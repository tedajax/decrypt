using System.Collections.Generic;

public class EventSystem : GameSystem
{
    Queue<GameEvent> eventQueue = new Queue<GameEvent>();

    class Listener
    {
        public System.Action<GameEvent> callback;

        public Listener(System.Action<GameEvent> callback)
        {
            this.callback = callback;
        }
    }

    Dictionary<System.Type, Dictionary<object, Listener>> listenersByEventType = new Dictionary<System.Type, Dictionary<object, Listener>>();

    public void Call(GameEvent gameEvent)
    {
        eventQueue.Enqueue(gameEvent);
    }

    public void Subscribe<TEvent>(object listener, System.Action<GameEvent> callback)
    {
        Dictionary<object, Listener> eventListeners;
        if (!listenersByEventType.TryGetValue(typeof(TEvent), out eventListeners))
        {
            eventListeners = new Dictionary<object, Listener>();
            listenersByEventType.Add(typeof(TEvent), eventListeners);
        }

        if (eventListeners.ContainsKey(listener))
        {
            UnityEngine.Debug.LogError("Listener has already subscribed to this event type.");
            return;
        }

        eventListeners.Add(listener, new Listener(callback));
    }

    public void Unsubscribe<TEvent>(object listener)
    {
        Dictionary<object, Listener> eventListeners;
        if (!listenersByEventType.TryGetValue(typeof(TEvent), out eventListeners))
        {
            UnityEngine.Debug.LogError("No listeners of event type " + typeof(TEvent).ToString() + " could be found.");
            return;
        }

        if (!eventListeners.ContainsKey(listener))
        {
            UnityEngine.Debug.LogError("Listener is not subscribed to event type " + typeof(TEvent).ToString() + ".");
        }

        eventListeners.Remove(listener);
    }

    public void Update()
    {
        while (eventQueue.Count > 0)
        {
            GameEvent gameEvent = eventQueue.Dequeue();
            System.Type eventType = gameEvent.GetType();
            Dictionary<object, Listener> listeners;
            if (listenersByEventType.TryGetValue(eventType, out listeners))
            {
                foreach (var kvp in listeners)
                {
                    kvp.Value.callback(gameEvent);
                }
            }
        }
    }
}
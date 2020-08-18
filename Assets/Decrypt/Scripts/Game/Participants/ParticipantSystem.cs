using System.Collections;
using System.Collections.Generic;

public class ParticipantSystem : GameSystem, IAppSystem
{
    public static readonly string SignalParticipantAdded = "ParticipantSystem.SignalParticipantAdded";
    public static readonly string SignalParticipantSpawned = "ParticipantSystem.SignalParticipantSpawned";

    private List<Participant> activeParticipants = new List<Participant>();

    public void OnAppLoad()
    {
        AddParticipant();
    }

    public IEnumerable ActiveParticipantIds()
    {
        foreach (var participant in activeParticipants)
        {
            yield return participant.Id;
        }
    }

    private Participant AddParticipant()
    {
        Participant result = new Participant(activeParticipants.Count);
        activeParticipants.Add(result);
        signalBus.GetSignal<Signal<Participant>>(SignalParticipantAdded).Dispatch(result);
        return result;
    }

    public Participant GetParticipant(int participantId)
    {
        return activeParticipants[participantId];
    }
}
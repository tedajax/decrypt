using System.Collections.Generic;
using UnityEngine;

public class SpawnSystem : GameSystem, IUpdateSystem, IMapSystem
{
    [Inject]
    public ParticipantSystem participantSystem { get; set; }

    List<SpawnPoint> spawnPointsInMap = new List<SpawnPoint>();

    // TODO: some kind of pending spawn data, just using a dummy int for now
    Queue<Participant> pendingSpawns = new Queue<Participant>();

    Dictionary<int, Participant> activeParticipants = new Dictionary<int, Participant>();

    private Signal<Participant> participantAddedSignal;
    private Signal<Participant> participantSpawnedSignal;

    public SpawnSystem() { }

    protected override void Initialize()
    {
        participantAddedSignal = signalBus.GetSignal<Signal<Participant>>(ParticipantSystem.SignalParticipantAdded);
        participantSpawnedSignal = signalBus.GetSignal<Signal<Participant>>(ParticipantSystem.SignalParticipantSpawned);
        participantAddedSignal.AddListener(OnParticipantAdded);
    }

    public void Dispose(bool disposing)
    {
        participantAddedSignal.RemoveListener(OnParticipantAdded);
    }

    public void OnMapLoad()
    {
        spawnPointsInMap.AddRange(Object.FindObjectsOfType<SpawnPoint>());
    }

    public void OnMapUnload()
    {
        spawnPointsInMap.Clear();
    }

    private void OnParticipantAdded(Participant participant)
    {
        pendingSpawns.Enqueue(participant);
    }

    public void OnUpdate()
    {
        while (pendingSpawns.Count > 0)
        {
            Participant participant = pendingSpawns.Dequeue();

            // Choose random spawner
            SpawnPoint chosenSpawnPoint = null;
            if (spawnPointsInMap.Count > 0)
            {
                chosenSpawnPoint = spawnPointsInMap[Random.Range(0, spawnPointsInMap.Count)];
            }

            if (chosenSpawnPoint == null)
            {
                Debug.LogError("Unable to find a spawn point, are any placed in the level?");
            }
            else
            {
                GameObject spawnedUnitObject = chosenSpawnPoint.SpawnPrefab(config.characterPrefab);
                IUnit unit = spawnedUnitObject.GetComponent<IUnit>();
                participant.OnSpawn(unit);

                GameObject cameraObject = Object.Instantiate(config.cameraPrefab,
                    unit.HeadPosition,
                    unit.LookRotation);

                cameraObject.GetComponent<FirstPersonUnitCamera>().attachedUnit = unit;

                participantSpawnedSignal.Dispatch(participant);
            }
        }
    }

}
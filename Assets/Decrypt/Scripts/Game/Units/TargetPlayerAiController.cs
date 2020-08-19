using UnityEngine;
using UnityEngine.AI;

public class TargetPlayerAiController : DecryptComponent
{
    private Signal<Participant> participantSpawnedSignal = null;
    private Transform targetTransform = null;
    private NavMeshPath path;

    protected override void Awake()
    {
        base.Awake();

        // NavMeshPath constructor cannot be called in MonoBehaviour constructor
        path = new NavMeshPath();

        participantSpawnedSignal = signalBus.GetSignal<Signal<Participant>>(ParticipantSystem.SignalParticipantSpawned);
        participantSpawnedSignal.AddListener(OnParticipantSpawned);
    }

    public override void Dispose()
    {
        participantSpawnedSignal.RemoveListener(OnParticipantSpawned);
    }

    private void OnParticipantSpawned(Participant participant)
    {
        targetTransform = participant.Unit.UnitTransform;
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            return;
        }

        if (NavMesh.CalculatePath(transform.position, targetTransform.position, 0x7FFFFFFF, path))
        {
            for (int i = 0; i < path.corners.Length - 1; ++i)
            {
                Vector3 a = path.corners[i];
                Vector3 b = path.corners[i + 1];
                Debug.DrawLine(a, b, Color.yellow, 0.1f);
            }
        }
    }
}
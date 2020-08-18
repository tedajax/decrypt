using UnityEngine;

public class TargetPlayerAiController : DecryptComponent
{
    private Signal<Participant> participantSpawnedSignal;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform targetTransform;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

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
        if (targetTransform != null)
        {
            agent.SetDestination(targetTransform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }
}
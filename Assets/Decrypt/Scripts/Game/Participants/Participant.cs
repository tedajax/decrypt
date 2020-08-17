public class Participant
{
    private int participantId = -1;
    private IUnit unit = null;

    public IUnit Unit => unit;

    public int Id => participantId;
    public bool HasUnit => unit != null;

    public Participant(int participantId)
    {
        this.participantId = participantId;
    }

    public void OnSpawn(IUnit unit)
    {
        this.unit = unit;
    }

    public void OnDeath()
    {
        unit = null;
    }
}
using UnityEngine;

public interface IUnit
{
    Vector3 Position { get; set; }
    Vector3 HeadPosition { get; }
    float LookPitch { get; set; }
    float LookHeading { get; set; }
    Quaternion LookRotation { get; }
    Transform HeadTransform { get; }
}

public class Unit : MonoBehaviour, IUnit
{
    [SerializeField]
    private Transform headTransform = null;
    private float lookPitch = 0f;
    private float lookHeading = 0f;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector3 HeadPosition => (headTransform != null) ? headTransform.position : Position;

    public float LookPitch
    {
        get => lookPitch;
        set => lookPitch = value;
    }

    public float LookHeading
    {
        get => lookHeading;
        set => lookHeading = value;
    }

    public Transform HeadTransform => headTransform;

    private Quaternion lookRotation = Quaternion.identity;
    public Quaternion LookRotation => lookRotation;

    void Update()
    {
        Quaternion pitchRotation = Quaternion.AngleAxis(LookPitch, Vector3.right);
        Quaternion yawRotation = Quaternion.AngleAxis(lookHeading, Vector3.up);
        lookRotation = yawRotation * pitchRotation;

        if (HeadTransform != null)
        {
            HeadTransform.localRotation = pitchRotation;
        }
    }
}
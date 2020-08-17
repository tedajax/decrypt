using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public Vector3 pivotVector = Vector3.right;
    public float minAngle = -Mathf.Infinity;
    public float maxAngle = Mathf.Infinity;

    public void SetAngle(float angle)
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Clamp(angle, minAngle, maxAngle), pivotVector);
    }
}
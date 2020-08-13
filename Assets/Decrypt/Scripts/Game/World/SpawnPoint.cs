using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 center = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawSphere(center, 0.5f);
        Gizmos.DrawLine(center, center + transform.forward);
    }
}
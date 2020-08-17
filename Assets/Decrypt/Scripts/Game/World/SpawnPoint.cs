using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject SpawnPrefab(GameObject prefab)
    {
        return Instantiate(prefab, transform.position, transform.rotation);
    }

    void OnDrawGizmos()
    {
        Vector3 center = transform.position + Vector3.up * 1f;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(center, 0.5f);
        Gizmos.DrawLine(transform.position, center);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(center, center + transform.forward);
    }
}
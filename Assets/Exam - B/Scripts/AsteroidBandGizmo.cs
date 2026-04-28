using UnityEngine;

public class AsteroidBandGizmo : MonoBehaviour
{
    [SerializeField] private float halfHeight = 3f;
    [SerializeField] private float width = 26f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.25f);
        Vector3 c = transform.position;
        Vector3 size = new Vector3(width, halfHeight * 2f, 0.1f);
        Gizmos.DrawCube(c, size);

        Gizmos.color = new Color(1f, 0.6f, 0.2f, 1f);
        Gizmos.DrawWireCube(c, size);
    }
}

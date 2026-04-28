using UnityEngine;

/// <summary>
/// Holds an AABB size for collision checks (axis-aligned).
/// </summary>
public class ObstacleAABB : MonoBehaviour
{
    [SerializeField] private Vector2 size = Vector2.one;
    public Vector2 Size => size;

    private void OnValidate()
    {
        size = new Vector2(Mathf.Max(0.1f, size.x), Mathf.Max(0.1f, size.y));
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }
}

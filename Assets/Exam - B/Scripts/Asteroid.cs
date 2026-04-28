using UnityEngine;

/// <summary>
/// A simple asteroid with manual motion and AABB size.
/// Students implement the AABB overlap test in Task 7.
///
/// Consumed by AsteroidCollisionManager at runtime — while the placeholder
/// is in place, asteroids never collide with each other. A warning is
/// printed to the Console on Play to make this visible.
/// </summary>
public class Asteroid : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 size = Vector2.one;

    public Vector2 Velocity => velocity;
    public Vector2 Size => size;

    public void SetVelocity(Vector2 v) => velocity = v;

    public void SetSize(Vector2 newSize)
    {
        size = new Vector2(Mathf.Max(0.1f, newSize.x), Mathf.Max(0.1f, newSize.y));
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Cleanup — prevents runaway asteroids.
        if (Mathf.Abs(transform.position.x) > 40f || Mathf.Abs(transform.position.y) > 40f)
            Destroy(gameObject);
    }

    /// <summary>
    /// Returns true when this asteroid overlaps the other asteroid.
    /// </summary>
    public bool OverlapsAABB(Asteroid other)
    {
        if (other == null) return false;

        // ---------------- TASK 7 ----------------
        // Goal: return true when this asteroid and `other` overlap as AABBs.
        //
        // Use:
        //   - transform.position         (Vector3 — centre of this box)
        //   - size                       (Vector2 — full size of this box)
        //   - other.transform.position
        //   - other.size  (accessible via other.Size)
        //   - Mathf.Abs(...)
        //
        // Half-extents:
        //   halfA = this.size * 0.5
        //   halfB = other.Size * 0.5
        //
        // Overlap condition (week 4 cheatsheet):
        //   abs(dx) <= (halfA.x + halfB.x)  AND  abs(dy) <= (halfA.y + halfB.y)
        //
        // Quick check:
        //   - two asteroids at the same position overlap
        //   - two asteroids far apart do not overlap
        //
        // Avoid:
        //   - comparing full sizes instead of half-extents
        //   - checking only one axis (both must overlap)
        //
        // Placeholder behaviour: always false — no collisions ever fire.
        return false; // TODO — implement the AABB overlap test.
        // -------------- END TASK 7 --------------
    }
}

using UnityEngine;

/// <summary>
/// Static helper for AABB overlap checks.
/// Students implement overlap logic in Task 7.
///
/// Consumed by DroneCollisionAABB at runtime — while the placeholder is
/// returned, Stage 3 collision detection is silently disabled. A warning
/// is printed to the Console on Play to make this visible.
/// </summary>
public static class AABB2D
{
    /// <summary>
    /// Returns true when the two axis-aligned boxes overlap.
    /// Each box is defined by its centre and its full size (width, height).
    /// </summary>
    public static bool Overlaps(Vector2 aCenter, Vector2 aSize, Vector2 bCenter, Vector2 bSize)
    {
        // ---------------- TASK 7 ----------------
        // Goal: return true when two axis-aligned boxes overlap, false otherwise.
        //
        // Use:
        //   - aCenter, aSize   (Vector2 centre and full size of box A)
        //   - bCenter, bSize   (Vector2 centre and full size of box B)
        //   - Mathf.Abs(...)
        //
        // Half-extents:
        //   halfA = aSize * 0.5
        //   halfB = bSize * 0.5
        //
        // Overlap condition (from the week 4 cheatsheet):
        //   Abs(aCenter.x - bCenter.x) <= (halfA.x + halfB.x)
        //   AND
        //   Abs(aCenter.y - bCenter.y) <= (halfA.y + halfB.y)
        //
        // Quick check:
        //   - two boxes with the same centre should overlap (return true)
        //   - two boxes far apart on x or y should not overlap (return false)
        //   - two boxes just touching edge-to-edge should overlap (<= not <)
        //
        // Avoid:
        //   - comparing full sizes instead of half-extents
        //   - using only one axis (both axes must overlap for AABBs to overlap)
        //
        // Placeholder behaviour: always returns false. While this is true,
        // the runtime collision system cannot detect anything and the drone
        // clips through obstacles in Stage 3.
        return false; // TODO — implement the AABB overlap test.
        // -------------- END TASK 7 --------------
    }
}

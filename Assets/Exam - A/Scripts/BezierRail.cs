using UnityEngine;

/// <summary>
/// A single cubic Bezier rail segment defined by 4 control points (P0..P3).
/// Students implement curve evaluation in Task 1.
/// </summary>
public class BezierRail : MonoBehaviour
{
    [Header("Control points (set by staff)")]
    [SerializeField] private Transform p0;
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;
    [SerializeField] private Transform p3;

    [Header("Routing (set by staff)")]
    [Tooltip("Lower is better. Used at junction choice.")]
    [SerializeField] private float costToGoal = 10f;

    public float CostToGoal => costToGoal;

    public Vector3 P0 => p0.position;
    public Vector3 P1 => p1.position;
    public Vector3 P2 => p2.position;
    public Vector3 P3 => p3.position;

    /// <summary>
    /// Returns the position on the cubic Bezier curve at parameter t in [0,1].
    /// </summary>
    public Vector3 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);   // t is the 'progress' along the curve, 0..1
        float u = 1f - t;       // convenient shorthand for (1 - t)

        // ---------------- TASK 1 ----------------
        // Goal: implement the cubic Bezier position formula.
        //
        // Use:
        //   - P0, P1, P2, P3   (Vector3 control points, already exposed above)
        //   - t                (float, clamped to [0,1] above)
        //   - u = 1 - t        (float, computed above)
        //
        // Formula (from the week 6 cheatsheet):
        //   B(t) = u^3 * P0
        //        + 3 * u^2 * t   * P1
        //        + 3 * u   * t^2 * P2
        //        + t^3 * P3
        //
        // Quick check:
        //   - Evaluate(0) should return P0 (start of the curve)
        //   - Evaluate(1) should return P3 (end of the curve)
        //   - values in between should produce a smooth curve through P1, P2
        //
        // Avoid:
        //   - returning Vector3.zero (the placeholder)
        //   - using a linear interpolation between P0 and P3 (that ignores P1, P2)
        //
        // Placeholder behaviour: returns origin, so the curve is meaningless.
        return Vector3.zero; // TODO — replace this line with the Bezier formula.
        // -------------- END TASK 1 --------------
    }

    private void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null || p3 == null)
            return;

        // Draw curve for staff + marking
        Gizmos.color = new Color(0.2f, 0.9f, 1f, 1f);

        Vector3 prev = p0.position;
        const int steps = 24;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector3 cur = EvaluateRuntimeSafe(t);
            Gizmos.DrawLine(prev, cur);
            prev = cur;
        }
    }

    // Runtime-safe fallback for gizmos before students complete Task 1.
    // This is NOT the student's answer — it exists so the rails still render
    // in the Scene view while Task 1 is unsolved.
    private Vector3 EvaluateRuntimeSafe(float t)
    {
        t = Mathf.Clamp01(t);
        float u = 1f - t;
        return (u * u * u) * P0 + (3f * u * u * t) * P1 + (3f * u * t * t) * P2 + (t * t * t) * P3;
    }
}

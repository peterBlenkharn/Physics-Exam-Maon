using UnityEngine;

/// <summary>
/// Moves the drone along the current rail using parameter t.
/// Students implement progression (Task 2) and tangent logic (Task 3).
/// </summary>
[DisallowMultipleComponent]
public class DroneRailFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DroneStageManager stageManager;
    [SerializeField] private BezierRail currentRail;

    [Header("Motion")]
    [SerializeField] private float speedAlongRail = 3.5f; // parameter-space speed (simplified)

    [Header("Runtime")]
    [SerializeField] private float t;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 forward = Vector2.up;

    public Vector2 Velocity => velocity;
    public float Speed => velocity.magnitude;
    public Vector2 Forward => forward;

    public void SetRail(BezierRail rail, bool resetT = true)
    {
        currentRail = rail;
        if (resetT) t = 0f;
    }

    private void Start()
    {
        // Ensure we start on a rail
        if (currentRail != null)
        {
            transform.position = currentRail.Evaluate(0f);
        }
    }

    private void Update()
    {
        // Early-out in Complete/Failed states. You do not need to edit this.
        if (stageManager != null && (stageManager.CurrentStage == RailExamStage.Complete || stageManager.CurrentStage == RailExamStage.Failed))
            return;

        if (currentRail == null) return;

        float dt = Time.deltaTime;
        Vector3 prevPos = transform.position;

        // `newPos` starts as the current position (no movement). Task 2's job
        // is to assign a new sampled position to `newPos` inside its block.
        Vector3 newPos = prevPos;

        // ---------------- TASK 2 ----------------
        // Goal: advance t along the current rail and sample the new position.
        //
        // Use:
        //   - dt                    (Time.deltaTime, computed above)
        //   - speedAlongRail         (parameter-space speed, field above)
        //   - t                     (float field, persists across frames)
        //   - Mathf.Clamp01(...)    (to keep t in [0,1])
        //   - currentRail.Evaluate(t)  (returns the position on the curve)
        //   - newPos                 (Vector3 local above — assign into this)
        //
        // Steps:
        //   1. Increase t by speedAlongRail * dt.
        //   2. Clamp t to [0,1] using Mathf.Clamp01.
        //   3. Assign newPos = currentRail.Evaluate(t).
        //
        // Quick check:
        //   - t increases smoothly and stops at 1
        //   - the drone follows the drawn Bezier curve from start to end
        //
        // Avoid:
        //   - treating t as world-space distance or arc length
        //   - reassigning newPos outside this block (leave the commit below alone)
        //
        // Placeholder behaviour: newPos stays equal to prevPos (no movement).
        // Suggested implementation shape (three lines):
        //     t += speedAlongRail * dt;
        //     t  = Mathf.Clamp01(t);
        //     newPos = currentRail.Evaluate(t);
        newPos = prevPos; // TODO — advance t and sample newPos from the rail.
        // -------------- END TASK 2 --------------

        // Commit motion — do NOT edit these two lines.
        transform.position = newPos;
        velocity = (Vector2)((newPos - prevPos) / Mathf.Max(dt, 0.0001f));

        // ---------------- TASK 3 ----------------
        // Goal: compute the drone's forward direction from a finite-difference
        // tangent along the current rail.
        //
        // Use:
        //   - currentRail.Evaluate(t)
        //   - currentRail.Evaluate(t + eps) with a small positive eps (e.g. 0.001f)
        //   - Vector3/Vector2 subtraction and .normalized
        //   - the `forward` field (Vector2)
        //
        // Finite-difference tangent:
        //     tangent approx= B(t + eps) - B(t)
        //     forward       = tangent.normalized
        //
        // Quick check:
        //   - on a straight rail, forward points along the rail
        //   - on a curved rail, the drone rotates as it moves
        //
        // Avoid:
        //   - using the direction to the dock or world origin
        //   - pointing the drone at a fixed world axis
        //   - using the drone's `velocity` vector directly (equivalent at
        //     non-zero speed, but zero on a stalled frame — gives a broken tangent)
        //
        // Placeholder behaviour: forward stuck pointing up.
        forward = Vector2.up; // TODO — replace with a finite-difference tangent.
        // -------------- END TASK 3 --------------

        // Commit rotation — do NOT edit this line.
        transform.up = new Vector3(forward.x, forward.y, 0f);
    }
}

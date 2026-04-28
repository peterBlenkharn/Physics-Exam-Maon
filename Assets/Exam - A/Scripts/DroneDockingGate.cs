using UnityEngine;

/// <summary>
/// Dock succeeds only in Stage 3 and only if the drone is both slow enough
/// AND aligned with the dock direction. Task 8 of the paper.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DroneDockingGate : MonoBehaviour
{
    [SerializeField] private DroneStageManager stageManager;
    [SerializeField] private DroneRailFollower drone;

    [Header("Docking constraints")]
    [Tooltip("The drone must be moving no faster than this to dock safely.")]
    [SerializeField] private float maxDockSpeed = 2.5f;

    [Tooltip("Dot product threshold: 1 = perfectly aligned, 0 = 90 degrees apart.")]
    [SerializeField] private float minAlignmentDot = 0.85f;

    [Tooltip("World-space dock direction (set by staff). Usually up or right.")]
    [SerializeField] private Vector2 dockDirectionWorld = Vector2.up;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (stageManager == null || drone == null) return;
        if (!stageManager.Stage3Active) return;

        // ---------------- TASK 8 ----------------
        // Goal: decide whether the dock attempt succeeds or fails.
        // Success requires BOTH: slow enough AND aligned with the dock direction.
        //
        // Use:
        //   - drone.Velocity              (Vector2 — current velocity)
        //   - drone.Speed                 (float — magnitude of Velocity)
        //   - drone.Forward                (Vector2 — drone's facing direction)
        //   - dockDirectionWorld           (Vector2 field above)
        //   - maxDockSpeed                  (float field above)
        //   - minAlignmentDot               (float field above)
        //   - stageManager.MarkComplete()  (call on success)
        //   - stageManager.MarkFailed()    (call on failure)
        //   - Vector2.Dot(a, b)
        //   - Vector2.normalized
        //
        // Steps:
        //   1. speed = drone.Speed
        //   2. alignment = Vector2.Dot(drone.Forward.normalized,
        //                              dockDirectionWorld.normalized)
        //   3. if speed <= maxDockSpeed AND alignment >= minAlignmentDot
        //        stageManager.MarkComplete();
        //      else
        //        stageManager.MarkFailed();
        //
        // Quick check:
        //   - slow AND aligned: run ends Complete (green state)
        //   - fast OR misaligned: run ends Failed (red state)
        //
        // Avoid:
        //   - using (dock position - drone position) for alignment — this
        //     task compares FACING DIRECTIONS, not positions
        //   - forgetting to normalise before taking the dot product
        //   - using OR where AND is needed (both conditions must hold)
        //
        // Depends on Task 3: if Task 3 is not implemented, drone.Forward is
        // stuck pointing up and this test will only succeed for upward docks.
        //
        // Placeholder behaviour: always fails, regardless of speed or alignment.
        stageManager.MarkFailed(); // TODO — replace with the pass/fail decision.
        // -------------- END TASK 8 --------------
    }

    private void OnValidate()
    {
        if (dockDirectionWorld.sqrMagnitude < 0.0001f)
            dockDirectionWorld = Vector2.up;
    }
}

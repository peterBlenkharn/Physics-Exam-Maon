using UnityEngine;

/// <summary>
/// When the rocket enters the station trigger during Stage 3,
/// the rocket must be under a maximum speed to succeed. Task 9.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DockingGate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private RocketPhysics rocketPhysics;

    [Header("Docking constraint")]
    [Tooltip("Maximum speed (magnitude of velocity) allowed for a safe dock.")]
    [SerializeField] private float maxDockSpeed = 3.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stageManager == null || rocketPhysics == null) return;
        if (!stageManager.Stage3Active) return;
        if (!other.CompareTag("Player")) return;

        // ---------------- TASK 9 ----------------
        // Goal: decide whether the dock succeeds or fails. Docking is safe
        // when the rocket's speed is no greater than maxDockSpeed.
        //
        // Use:
        //   - rocketPhysics.Speed         (float — magnitude of Velocity)
        //   - maxDockSpeed                (float field above)
        //   - stageManager.MarkComplete() (call on success)
        //   - stageManager.MarkFailed()   (call on failure)
        //
        // Steps:
        //   if rocketPhysics.Speed <= maxDockSpeed
        //       stageManager.MarkComplete();
        //   else
        //       stageManager.MarkFailed();
        //
        // Quick check:
        //   - Slow approach ends the run as Complete (green state)
        //   - Fast approach ends the run as Failed (red state)
        //
        // Avoid:
        //   - using velocity magnitude-squared without squaring maxDockSpeed
        //   - flipping the comparison (> instead of <=)
        //
        // Depends on Task 4:
        //   rocketPhysics.Speed is derived from the velocity that Task 4
        //   integrates. If Task 4 is not implemented, Speed is always 0 and
        //   this test always passes.
        //
        // Placeholder behaviour: always fails, regardless of approach speed.
        stageManager.MarkFailed(); // TODO — replace with the pass/fail decision.
        // -------------- END TASK 9 --------------
    }
}

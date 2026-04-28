using UnityEngine;

/// <summary>
/// Checks drone vs obstacle collisions using AABB.
/// If collision happens during Stage 3, fail immediately.
///
/// Not assessed — runtime scaffold only. It calls the student's
/// AABB2D.Overlaps each frame; if that is still the Task 7 placeholder
/// (always returns false), this script prints a Console warning once at
/// Start to make the silent failure visible.
/// </summary>
public class DroneCollisionAABB : MonoBehaviour
{
    [SerializeField] private DroneStageManager stageManager;

    [Tooltip("Drone AABB size (full size).")]
    [SerializeField] private Vector2 droneSize = new Vector2(1f, 1f);

    private void Start()
    {
        // Self-test: two boxes at the same centre with the same size MUST
        // overlap. If AABB2D.Overlaps returns false here, the student has
        // not yet implemented Task 7 and Stage 3 collisions will not fire.
        bool sameCentreOverlaps = AABB2D.Overlaps(
            Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.one);

        if (!sameCentreOverlaps)
        {
            Debug.LogWarning(
                "[Exam A] AABB2D.Overlaps is still the Task 7 placeholder (returns false). " +
                "Collision detection is disabled. Implement Task 7 in AABB2D.cs to enable " +
                "Stage 3 collisions.");
        }
    }

    private void Update()
    {
        if (stageManager == null || !stageManager.Stage3Active)
            return;

        // Simple: iterate obstacles under a known parent or by tag.
        ObstacleAABB[] obstacles = FindObjectsOfType<ObstacleAABB>();
        Vector2 droneCenter = transform.position;

        foreach (var obs in obstacles)
        {
            if (obs == null) continue;

            Vector2 obsCenter = obs.transform.position;
            Vector2 obsSize = obs.Size;

            bool hit = AABB2D.Overlaps(droneCenter, droneSize, obsCenter, obsSize);
            if (hit)
            {
                stageManager.MarkFailed();
                return;
            }
        }
    }
}

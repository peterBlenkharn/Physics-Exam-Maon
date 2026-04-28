using UnityEngine;

/// <summary>
/// At a junction, choose the outgoing rail with the lowest precomputed costToGoal.
/// Task 5 of the paper.
/// </summary>
public class JunctionChoiceTrigger : MonoBehaviour
{
    [SerializeField] private DroneStageManager stageManager;
    [SerializeField] private BezierRail[] outgoingRails;

    [Tooltip("After choosing a rail, only accept the choice during Stage 2.")]
    [SerializeField] private bool allowChoiceOnlyInStage2 = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (stageManager == null) return;

        if (allowChoiceOnlyInStage2 && stageManager.CurrentStage != RailExamStage.Stage2_JunctionChoice)
            return;

        DroneRailFollower follower = other.GetComponent<DroneRailFollower>();
        if (follower == null) return;
        if (outgoingRails == null || outgoingRails.Length == 0) return;

        // ---------------- TASK 5 ----------------
        // Goal: pick the outgoing rail with the LOWEST CostToGoal and
        // hand it to the drone via follower.SetRail(bestRail, true).
        //
        // Use:
        //   - outgoingRails           (BezierRail[], field above)
        //   - rail.CostToGoal          (float, smaller = better)
        //   - follower.SetRail(rail, resetT: true)
        //
        // Steps:
        //   1. Iterate outgoingRails and track the rail with the smallest
        //      CostToGoal seen so far.
        //   2. Call follower.SetRail on that rail, with resetT true.
        //
        // Quick check:
        //   - If one outgoing rail has a lower cost than the others, it is
        //     always the one picked (regardless of its position in the array).
        //
        // Avoid:
        //   - always picking outgoingRails[0] — that's the placeholder
        //   - picking the HIGHEST cost (this is minimum selection, not maximum)
        //
        // Placeholder behaviour: always chooses the first rail (WRONG if
        // the costs differ).
        follower.SetRail(outgoingRails[0], true); // TODO
        // -------------- END TASK 5 --------------
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.6f);
        Gizmos.DrawWireCube(transform.position, new Vector3(2f, 2f, 0.1f));
    }
}

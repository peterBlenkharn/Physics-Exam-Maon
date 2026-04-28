using UnityEngine;

public enum RailExamStage
{
    Stage1_EntryRail = 1,
    Stage2_JunctionChoice = 2,
    Stage3_CorridorAndDock = 3,
    Complete = 99,
    Failed = -1
}

/// <summary>
/// Simple stage progression + time pressure for the exam scene.
/// Students do not need to edit this file.
/// </summary>
public class DroneStageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform drone;

    [Header("Stage thresholds")]
    [SerializeField] private float yToEnterStage2 = 3.5f;
    [SerializeField] private float yToEnterStage3 = 7.0f;

    [Header("Stage 3 time limit")]
    [SerializeField] private float stage3TimeLimitSeconds = 25f;

    public RailExamStage CurrentStage { get; private set; } = RailExamStage.Stage1_EntryRail;
    public float Stage3TimeRemaining { get; private set; }

    public bool Stage3Active => CurrentStage == RailExamStage.Stage3_CorridorAndDock;
    public bool CanSpawnObstacles => Stage3Active;

    private void Start()
    {
        Stage3TimeRemaining = stage3TimeLimitSeconds;
    }

    private void Update()
    {
        if (CurrentStage == RailExamStage.Complete || CurrentStage == RailExamStage.Failed || drone == null)
            return;

        float y = drone.position.y;

        if (CurrentStage == RailExamStage.Stage1_EntryRail && y >= yToEnterStage2)
            CurrentStage = RailExamStage.Stage2_JunctionChoice;

        if ((CurrentStage == RailExamStage.Stage1_EntryRail || CurrentStage == RailExamStage.Stage2_JunctionChoice) && y >= yToEnterStage3)
            CurrentStage = RailExamStage.Stage3_CorridorAndDock;

        if (CurrentStage == RailExamStage.Stage3_CorridorAndDock)
        {
            Stage3TimeRemaining -= Time.deltaTime;
            if (Stage3TimeRemaining <= 0f)
                CurrentStage = RailExamStage.Failed;
        }
    }

    public void MarkComplete()
    {
        if (CurrentStage != RailExamStage.Failed)
            CurrentStage = RailExamStage.Complete;
    }

    public void MarkFailed()
    {
        CurrentStage = RailExamStage.Failed;
    }
}
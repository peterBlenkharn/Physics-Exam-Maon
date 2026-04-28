using UnityEngine;

public enum ExamStage
{
    Stage1_Atmosphere = 1,
    Stage2_StagingDecision = 2,
    Stage3_AsteroidBandAndDock = 3,
    Complete = 99,
    Failed = -1
}

/// <summary>
/// Stage progression + timers. Keeps the exam legible and markable.
/// Students are NOT expected to modify this file (unless you choose to add tasks here).
/// </summary>
public class StageManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rocket;

    [Header("Stage thresholds")]
    [SerializeField] private float altitudeToEnterStage2 = 4f;
    [SerializeField] private float altitudeToEnterStage3 = 9f;

    [Header("Stage 3 time pressure")]
    [SerializeField] private float stage3TimeLimitSeconds = 25f;

    public ExamStage CurrentStage { get; private set; } = ExamStage.Stage1_Atmosphere;
    public float Stage3TimeRemaining { get; private set; }

    public bool Stage3Active => CurrentStage == ExamStage.Stage3_AsteroidBandAndDock;
    public bool CanSpawnAsteroids => Stage3Active;

    private void Start()
    {
        Stage3TimeRemaining = stage3TimeLimitSeconds;
    }

    private void Update()
    {
        if (CurrentStage == ExamStage.Complete || CurrentStage == ExamStage.Failed)
            return;

        float y = rocket != null ? rocket.position.y : 0f;

        if (CurrentStage == ExamStage.Stage1_Atmosphere && y >= altitudeToEnterStage2)
        {
            CurrentStage = ExamStage.Stage2_StagingDecision;
        }

        if ((CurrentStage == ExamStage.Stage1_Atmosphere || CurrentStage == ExamStage.Stage2_StagingDecision) && y >= altitudeToEnterStage3)
        {
            CurrentStage = ExamStage.Stage3_AsteroidBandAndDock;
        }

        if (CurrentStage == ExamStage.Stage3_AsteroidBandAndDock)
        {
            Stage3TimeRemaining -= Time.deltaTime;
            if (Stage3TimeRemaining <= 0f)
            {
                CurrentStage = ExamStage.Failed;
            }
        }
    }

    public void MarkComplete()
    {
        if (CurrentStage != ExamStage.Failed)
            CurrentStage = ExamStage.Complete;
    }

    public void MarkFailed()
    {
        CurrentStage = ExamStage.Failed;
    }
}

using UnityEngine;

/// <summary>
/// Handles engine jettison.
/// After jettison: mass decreases AND max thrust decreases.
/// Jettison timing matters because Stage 3 manoeuvres will be weaker.
/// Task 5 of the paper.
/// </summary>
[DisallowMultipleComponent]
public class StagingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RocketController controller;
    [SerializeField] private StageManager stageManager;

    [Header("Stage parameters")]
    [SerializeField] private float preJettisonMassKg = 14f;
    [SerializeField] private float postJettisonMassKg = 9f;

    [Tooltip("Max thrust before jettison.")]
    [SerializeField] private float preJettisonMaxThrust = 60f;

    [Tooltip("Max thrust AFTER jettison (reduced).")]
    [SerializeField] private float postJettisonMaxThrust = 35f;

    [Header("Jettison gate")]
    [Tooltip("Jettison only allowed once above this altitude.")]
    [SerializeField] private float minAltitudeToJettison = 5f;

    [Header("Runtime (read-only)")]
    [SerializeField] private bool hasJettisoned = false;

    public float CurrentMassKg => hasJettisoned ? postJettisonMassKg : preJettisonMassKg;
    public float CurrentMaxThrust => hasJettisoned ? postJettisonMaxThrust : preJettisonMaxThrust;
    public bool HasJettisoned => hasJettisoned;

    private void Update()
    {
        // ---------------- TASK 5 ----------------
        // Goal: decide when to allow jettison. Jettison is a one-shot:
        // it may only happen ONCE, only when the J key is pressed, and only
        // when the rocket is above the minimum altitude.
        //
        // Use:
        //   - controller.JettisonPressedThisFrame   (bool)
        //   - transform.position.y                  (current altitude)
        //   - minAltitudeToJettison                 (float field above)
        //   - hasJettisoned                         (bool field — already checked below)
        //
        // Steps:
        //   Set canJettison = true only when ALL of these hold:
        //     - controller.JettisonPressedThisFrame is true
        //     - transform.position.y >= minAltitudeToJettison
        //
        //   (The "only once" check is done by the `!hasJettisoned` guard below.)
        //
        // Quick check:
        //   - Pressing J on the pad does nothing
        //   - Pressing J above min altitude jettisons once
        //   - Pressing J again after jettison does nothing (no second jettison)
        //
        // Avoid:
        //   - leaving canJettison = false (the placeholder)
        //   - using Input.GetKey(...) directly here — the controller already
        //     reads the key and exposes JettisonPressedThisFrame
        //
        // Placeholder behaviour: never jettisons.
        bool canJettison = false; // TODO

        if (!hasJettisoned && canJettison)
        {
            hasJettisoned = true;
        }
        // -------------- END TASK 5 --------------
    }
}

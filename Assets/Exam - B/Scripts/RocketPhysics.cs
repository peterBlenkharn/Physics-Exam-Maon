using UnityEngine;

/// <summary>
/// Manual physics: forces -> acceleration -> velocity -> position.
/// Students fill in Tasks 2 (thrust force), 3 (drag), and 4 (integration).
/// </summary>
[DisallowMultipleComponent]
public class RocketPhysics : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RocketController controller;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StagingSystem staging;

    [Header("Physical parameters")]
    [SerializeField] private float baseMassKg = 10f;

    [Tooltip("Max thrust force (N). Reduced after jettison by StagingSystem.")]
    [SerializeField] private float maxThrustNewton = 55f;

    [Tooltip("Gravity acceleration (units/s^2).")]
    [SerializeField] private float gravity = 9.81f;

    [Header("Atmosphere drag")]
    [Tooltip("Linear drag coefficient. F_drag = -k * v")]
    [SerializeField] private float linearDragK = 2.0f;

    [Header("Runtime (read-only)")]
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private float currentMassKg;

    public Vector2 Velocity => velocity;
    public float Speed => velocity.magnitude;

    public float CurrentMaxThrust => staging != null ? staging.CurrentMaxThrust : maxThrustNewton;
    public float CurrentMassKg => currentMassKg;

    private void Start()
    {
        currentMassKg = baseMassKg;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Mass may change after jettison (Task 5).
        currentMassKg = staging != null ? staging.CurrentMassKg : baseMassKg;

        // ---------------- TASK 2 ----------------
        // Goal: compute the thrust force vector from the controller output.
        //
        // Use:
        //   - controller.DesiredThrustDirection  (Vector2, unit-ish)
        //   - controller.DesiredThrustAmount01   (float, 0..1)
        //   - CurrentMaxThrust                   (float, scalar)
        //
        // Formula:
        //   thrustForce = direction * (amount * maxThrust)
        //
        // Quick check:
        //   - at full input, thrust force has magnitude ~= CurrentMaxThrust
        //   - at zero input, thrust force is Vector2.zero
        //
        // Avoid:
        //   - forgetting to multiply by amount (then half-input = full thrust)
        //
        // Placeholder behaviour: no thrust — rocket only feels gravity.
        Vector2 thrustForce = Vector2.zero; // TODO
        // -------------- END TASK 2 --------------

        // Gravity force — always downward, pre-multiplied by mass.
        Vector2 gravityForce = new Vector2(0f, -gravity * currentMassKg);

        // ---------------- TASK 3 ----------------
        // Goal: apply a linear drag force, but ONLY in atmosphere (Stage 1
        // or Stage 2). In Stage 3 (space) drag is zero.
        //
        // Use:
        //   - velocity       (Vector2 field)
        //   - linearDragK    (float field)
        //   - stageManager.CurrentStage
        //   - ExamStage.Stage1_Atmosphere, ExamStage.Stage2_StagingDecision
        //
        // Formula:
        //   F_drag = -linearDragK * velocity     (atmosphere only)
        //   F_drag = Vector2.zero                (space)
        //
        // Quick check:
        //   - In Stage 1 or 2 the rocket feels thick; top speed is limited
        //   - In Stage 3 the rocket feels floaty and coasts
        //
        // Avoid:
        //   - always applying drag (makes Stage 3 feel wrong)
        //   - never applying drag (the placeholder — Stage 1 feels weightless)
        //
        // Placeholder behaviour: drag is always zero.
        Vector2 dragForce = Vector2.zero; // TODO
        // -------------- END TASK 3 --------------

        // Net force on the rocket this frame.
        Vector2 netForce = thrustForce + gravityForce + dragForce;

        // ---------------- TASK 4 ----------------
        // Goal: implement the Euler integration pipeline so the rocket
        // actually moves. This is the core physics step for the whole exam.
        //
        // Use:
        //   - netForce              (Vector2, computed above)
        //   - currentMassKg         (float, updated above)
        //   - dt                    (float, Time.deltaTime)
        //   - velocity              (Vector2 field)
        //   - acceleration          (Vector2 field)
        //   - transform.position    (Vector3)
        //
        // Steps (in this order):
        //   1. acceleration = netForce / currentMassKg            (a = F / m)
        //   2. velocity     += acceleration * dt                   (v += a * dt)
        //   3. transform.position += (Vector3)(velocity * dt)      (p += v * dt)
        //
        // Quick check:
        //   - Lift-off arc looks smooth
        //   - Inertia is correct — cutting thrust doesn't teleport the rocket
        //   - No frame-rate jitter
        //
        // Avoid:
        //   - multiplying by dt twice (e.g. inside acceleration and velocity)
        //   - updating position before velocity (Euler is v then p)
        //   - forgetting the cast to Vector3 on the position update
        //
        // Placeholder behaviour: no movement at all; rocket sits on the pad.
        acceleration = Vector2.zero; // TODO
        // velocity           += ... ;
        // transform.position += ... ;
        // -------------- END TASK 4 --------------

        // Ground clamp — keeps the rocket from falling forever if the
        // integration is wrong. Not part of any task; do not edit.
        if (transform.position.y < -2.4f)
        {
            Vector3 p = transform.position;
            p.y = -2.4f;
            transform.position = p;
            velocity = Vector2.zero;
        }
    }
}

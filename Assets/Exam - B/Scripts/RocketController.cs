using UnityEngine;

/// <summary>
/// Reads player input and outputs a desired thrust direction + magnitude.
/// Students fill in Task 1 (input to thrust direction / amount).
/// </summary>
[DisallowMultipleComponent]
public class RocketController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private KeyCode jettisonKey = KeyCode.J;

    [Header("Control settings")]
    [Tooltip("Raw input magnitude below this value produces zero thrust.")]
    [SerializeField] private float inputDeadzone = 0.15f;

    // Output values consumed by RocketPhysics
    public Vector2 DesiredThrustDirection { get; private set; } = Vector2.up;
    public float DesiredThrustAmount01 { get; private set; } = 0f; // 0..1
    public bool JettisonPressedThisFrame { get; private set; } = false;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(x, y);

        // Deadzone short-circuit (already done for you). When input magnitude
        // is too small, there is nothing useful to derive — so we zero it out
        // and skip the block below.
        if (input.magnitude < inputDeadzone)
        {
            DesiredThrustDirection = Vector2.up;
            DesiredThrustAmount01 = 0f;
        }
        else
        {
            // ---------------- TASK 1 ----------------
            // Goal: convert `input` into a unit direction and an amount in [0,1].
            //
            // Use:
            //   - input                 (Vector2, raw axes, magnitude can be > 1)
            //   - DesiredThrustDirection (Vector2 property — set this)
            //   - DesiredThrustAmount01  (float property — set this)
            //   - Vector2.normalized
            //   - Mathf.Clamp01(...)
            //
            // Steps:
            //   1. DesiredThrustDirection = input.normalized
            //   2. DesiredThrustAmount01 = Mathf.Clamp01(input.magnitude)
            //
            // Quick check:
            //   - Pressing Right gives direction (1,0), amount ~1
            //   - Pressing Up+Right gives direction on the diagonal, amount ~1
            //   - Gamepad half-tilt gives amount < 1
            //
            // Avoid:
            //   - always outputting Vector2.up (the placeholder)
            //   - setting amount to a value > 1 (a Vector2 magnitude can exceed
            //     1 when both axes are pressed at full)
            //
            // Placeholder behaviour: always "up, full throttle" when outside
            // the deadzone — so the rocket can only thrust up.
            DesiredThrustDirection = Vector2.up; // TODO
            DesiredThrustAmount01 = 1f;          // TODO
            // -------------- END TASK 1 --------------
        }

        JettisonPressedThisFrame = Input.GetKeyDown(jettisonKey);
    }
}

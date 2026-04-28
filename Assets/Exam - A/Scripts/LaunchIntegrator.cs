using UnityEngine;

/// <summary>
/// A small physics sandbox. Simulates a single particle under a constant
/// thrust + gravity, using the Euler integration pipeline
/// (force -> acceleration -> velocity -> position).
///
/// This script is the host for TASK 4. It is deliberately separate from the
/// drone's rail motion so that the integration pipeline can be seen and
/// marked in isolation.
///
/// The GameObject, sprite, and initial position are all created at runtime
/// via [RuntimeInitializeOnLoadMethod], so no scene edits are required.
/// </summary>
[DisallowMultipleComponent]
public class LaunchIntegrator : MonoBehaviour
{
    [Header("Physical parameters (pre-set)")]
    [Tooltip("Mass of the simulated particle (kg).")]
    [SerializeField] private float massKg = 1.5f;

    [Tooltip("Downward gravity acceleration (units/s^2).")]
    [SerializeField] private float gravity = 9.81f;

    [Tooltip("Constant upward thrust force applied every frame (N).")]
    [SerializeField] private Vector2 constantThrust = new Vector2(0f, 22f);

    [Header("Auto-reset (to keep the demo looping)")]
    [Tooltip("If the particle falls below this Y, it teleports back to the start.")]
    [SerializeField] private float resetBelowY = -3f;

    [Tooltip("Position the particle respawns at when reset.")]
    [SerializeField] private Vector3 startPosition = new Vector3(-8f, -1.5f, 0f);

    [Header("Runtime (read-only — inspect these to debug)")]
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 acceleration;

    public Vector2 Velocity => velocity;
    public Vector2 Acceleration => acceleration;

    /// <summary>
    /// Auto-create a LaunchIntegrator GameObject with a visible yellow sprite
    /// if the scene does not already contain one. This means the test rig is
    /// visible on Play without any scene-file edits.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureExists()
    {
        // Avoid double-spawn if staff added one to the scene.
        if (Object.FindObjectOfType<LaunchIntegrator>() != null) return;

        GameObject go = new GameObject("LaunchIntegrator (auto)");
        LaunchIntegrator li = go.AddComponent<LaunchIntegrator>();
        go.transform.position = li.startPosition;
        go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        // Programmatic square sprite — keeps the asset database untouched.
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Texture2D tex = new Texture2D(8, 8);
        Color fill = new Color(1f, 0.85f, 0.1f, 1f);
        Color[] pixels = new Color[64];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = fill;
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
        sr.sortingOrder = 5;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Gravity force (acts downward; mass is already factored in here so
        // that you can add it directly to the net force below).
        Vector2 gravityForce = new Vector2(0f, -gravity * massKg);

        // Net force on the particle this frame.
        Vector2 netForce = constantThrust + gravityForce;

        // ---------------- TASK 4 ----------------
        // Goal: implement the Euler integration pipeline to move this
        // particle under thrust + gravity.
        //
        // Use:
        //   - netForce   (Vector2, already computed above)
        //   - massKg     (float, field)
        //   - dt         (float, Time.deltaTime, already computed above)
        //   - velocity   (Vector2, field, persists across frames)
        //   - transform.position (Vector3, this GameObject's position)
        //
        // Steps (in this order):
        //   1. acceleration = netForce / massKg                 (a = F / m)
        //   2. velocity     += acceleration * dt                (v += a * dt)
        //   3. transform.position += (Vector3)(velocity * dt)   (p += v * dt)
        //
        // Quick check:
        //   - the yellow square should rise from the bottom-left, slow down,
        //     and fall back down in a smooth arc
        //   - the particle should loop (auto-reset snaps it back to start)
        //
        // Avoid:
        //   - multiplying by dt twice (e.g. inside acceleration AND velocity)
        //   - updating position BEFORE velocity — Euler is v-then-p
        //   - forgetting the cast to Vector3 when updating transform.position
        //
        // Placeholder behaviour: no motion.
        acceleration = Vector2.zero; // TODO
        // velocity           += ... ;
        // transform.position += ... ;
        // -------------- END TASK 4 --------------

        // Keep the demo visible by resetting when the particle flies off.
        if (transform.position.y < resetBelowY || Mathf.Abs(transform.position.x) > 14f)
        {
            transform.position = startPosition;
            velocity = Vector2.zero;
        }
    }
}

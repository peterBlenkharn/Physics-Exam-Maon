using UnityEngine;

/// <summary>
/// Spawns asteroids in Stage 3 within a vertical band.
/// Students implement a non-uniform distribution for spawn height. Task 6.
/// </summary>
public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Transform bandTransform;
    [SerializeField] private Transform asteroidParent;
    [SerializeField] private Asteroid asteroidPrefab;

    [Header("Band settings")]
    [Tooltip("Half-height of the spawn band.")]
    [SerializeField] private float bandHalfHeight = 3f;

    [Tooltip("Width of the X range spawns are drawn from.")]
    [SerializeField] private float bandWidthX = 26f;

    [Header("Spawn timing")]
    [SerializeField] private float spawnIntervalSeconds = 0.7f;

    [Header("Asteroid motion")]
    [SerializeField] private float asteroidSpeedMin = 2.5f;
    [SerializeField] private float asteroidSpeedMax = 5.5f;

    private float _spawnTimer;

    private void Update()
    {
        if (stageManager == null || !stageManager.CanSpawnAsteroids)
            return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < spawnIntervalSeconds)
            return;

        _spawnTimer = 0f;
        SpawnOne();
    }

    private void SpawnOne()
    {
        if (asteroidPrefab == null || bandTransform == null || asteroidParent == null)
            return;

        float bandCenterY = bandTransform.position.y;

        // ---------------- TASK 6 ----------------
        // Goal: choose a NON-UNIFORM spawn height in
        // [bandCenterY - bandHalfHeight, bandCenterY + bandHalfHeight],
        // biased toward bandCenterY (the middle of the band). Extremes should
        // still appear, but less often than the middle.
        //
        // Use:
        //   - bandCenterY       (float, computed above)
        //   - bandHalfHeight    (float field above)
        //   - Random.value      (returns 0..1 uniform)
        //
        // Hint:
        //   - Averaging two Random.value samples gives a value in [0,1]
        //     biased toward 0.5. Remap that biased 0..1 into
        //     [bandCenterY - bandHalfHeight, bandCenterY + bandHalfHeight].
        //
        // Quick check:
        //   - spawns still cover the whole band (extremes do appear)
        //   - the middle is visibly more common than the edges
        //
        // Avoid:
        //   - leaving the uniform Random.Range below (the placeholder)
        //   - clamping everything to bandCenterY (removes the extremes)
        //
        // Placeholder behaviour: uniform distribution across the band (WRONG).
        float y = Random.Range(bandCenterY - bandHalfHeight, bandCenterY + bandHalfHeight); // TODO
        // -------------- END TASK 6 --------------

        float x = Random.Range(-bandWidthX * 0.5f, bandWidthX * 0.5f);

        Asteroid a = Instantiate(asteroidPrefab, asteroidParent);
        a.transform.position = new Vector3(x, y, 0f);

        // Give it a random velocity — not assessed.
        float spd = Random.Range(asteroidSpeedMin, asteroidSpeedMax);
        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
        a.SetVelocity(dir * spd);

        // Random size — not assessed.
        float s = Random.Range(0.7f, 1.6f);
        a.SetSize(new Vector2(s, s));
    }
}

using UnityEngine;

/// <summary>
/// Spawns obstacles only in Stage 3, with a non-uniform height distribution.
/// Task 6 of the paper.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DroneStageManager stageManager;
    [SerializeField] private Transform obstacleParent;
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Spawn band (corridor area)")]
    [Tooltip("Middle of the allowed Y band. Spawns should cluster here.")]
    [SerializeField] private float hotHeightY = 8f;

    [Tooltip("Half the total height of the allowed Y band.")]
    [SerializeField] private float bandHalfHeight = 3f;

    [Tooltip("Width of the X range spawns are drawn from.")]
    [SerializeField] private float spawnWidthX = 6f;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.8f;

    private float timer;

    private void Update()
    {
        if (stageManager == null || !stageManager.CanSpawnObstacles)
            return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        SpawnOne();
    }

    private void SpawnOne()
    {
        if (obstaclePrefab == null || obstacleParent == null)
            return;

        // X spawn is uniform — that's fine. Only Y is asked to be biased.
        float x = Random.Range(-spawnWidthX * 0.5f, spawnWidthX * 0.5f);

        // ---------------- TASK 6 ----------------
        // Goal: choose a NON-UNIFORM y spawn within the allowed band, biased
        // toward hotHeightY (the middle). Extremes should still spawn, but
        // less often than the middle.
        //
        // Use:
        //   - hotHeightY              (float field, centre of the bias)
        //   - bandHalfHeight          (float field, half the band height)
        //   - Random.value            (returns 0..1 uniform)
        //
        // Hint:
        //   - Averaging two Random.value samples gives a value in [0,1]
        //     biased toward 0.5. That biased 0..1 can then be remapped into
        //     [hotHeightY - bandHalfHeight, hotHeightY + bandHalfHeight].
        //
        // Quick check:
        //   - spawns still cover the whole band (extremes appear sometimes)
        //   - the middle is visibly more common than the edges
        //
        // Avoid:
        //   - leaving the uniform Random.Range below (the placeholder) —
        //     that produces a flat distribution, not a biased one
        //   - clamping everything to hotHeightY — that removes the extremes
        //
        // Placeholder behaviour: uniform distribution across the band (WRONG).
        float y = Random.Range(hotHeightY - bandHalfHeight, hotHeightY + bandHalfHeight); // TODO
        // -------------- END TASK 6 --------------

        GameObject obj = Instantiate(obstaclePrefab, obstacleParent);
        obj.transform.position = new Vector3(x, y, 0f);
    }

    private void OnDrawGizmos()
    {
        // Shaded band (full allowed Y range)
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.25f);
        Gizmos.DrawCube(new Vector3(0f, hotHeightY, 0f), new Vector3(spawnWidthX, bandHalfHeight * 2f, 0.1f));
        // Hot-height midline
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 1f);
        Gizmos.DrawLine(new Vector3(-spawnWidthX * 0.5f, hotHeightY, 0f), new Vector3(spawnWidthX * 0.5f, hotHeightY, 0f));
    }
}

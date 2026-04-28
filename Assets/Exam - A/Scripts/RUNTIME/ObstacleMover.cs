using UnityEngine;

/// <summary>
/// Simple obstacle motion to create collisions in the corridor.
/// Not assessed: keep deterministic.
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    [SerializeField] private Vector2 velocity = new Vector2(0f, -2f);

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);

        if (transform.position.y < -20f)
            Destroy(gameObject);
    }
}

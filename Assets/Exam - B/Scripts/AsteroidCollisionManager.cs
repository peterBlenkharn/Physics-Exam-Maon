using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks asteroid-asteroid collisions and destroys both on overlap.
/// Host for Task 8. Uses the student's Task 7 Asteroid.OverlapsAABB.
///
/// If Task 7 is still the placeholder (always false), asteroid-vs-asteroid
/// collisions never fire. A Console warning is printed on Play to flag this.
/// </summary>
public class AsteroidCollisionManager : MonoBehaviour
{
    [SerializeField] private Transform asteroidParent;

    private readonly List<Asteroid> _buffer = new();
    private bool _warnedAboutPlaceholder = false;

    private void Update()
    {
        if (asteroidParent == null)
            return;

        // Collect live asteroids.
        _buffer.Clear();
        for (int i = 0; i < asteroidParent.childCount; i++)
        {
            Asteroid a = asteroidParent.GetChild(i).GetComponent<Asteroid>();
            if (a != null) _buffer.Add(a);
        }

        // Pairwise check (small counts; exam-safe).
        for (int i = 0; i < _buffer.Count; i++)
        {
            for (int j = i + 1; j < _buffer.Count; j++)
            {
                Asteroid A = _buffer[i];
                Asteroid B = _buffer[j];
                if (A == null || B == null) continue;

                // ---------------- TASK 8 ----------------
                // Goal: when A and B overlap as AABBs, destroy both.
                //
                // Use:
                //   - A.OverlapsAABB(B)    (from Task 7)
                //   - Destroy(A.gameObject), Destroy(B.gameObject)
                //
                // Quick check:
                //   - two asteroids that visibly touch annihilate each other
                //   - if Task 7 is placeholder, nothing is ever destroyed
                //
                // Avoid:
                //   - destroying only one of the two
                //   - calling Destroy repeatedly on the same object — once
                //     destroyed the reference is null
                //
                // Placeholder behaviour: no destruction happens.
                bool hit = false; // TODO — set to A.OverlapsAABB(B)

                if (hit)
                {
                    Destroy(A.gameObject);
                    Destroy(B.gameObject);
                }
                // -------------- END TASK 8 --------------
            }
        }

        // One-shot self-test for Task 7: if two asteroids at the exact same
        // position don't return true, Task 7 is still the placeholder.
        if (!_warnedAboutPlaceholder && _buffer.Count >= 2)
        {
            Asteroid a = _buffer[0];
            Asteroid b = _buffer[1];
            if (a != null && b != null && Vector2.Distance(a.transform.position, b.transform.position) < 0.01f)
            {
                if (!a.OverlapsAABB(b))
                {
                    Debug.LogWarning(
                        "[Exam B] Asteroid.OverlapsAABB is still the Task 7 placeholder (returns false). " +
                        "Asteroid-asteroid collisions are disabled. Implement Task 7 in Asteroid.cs.");
                    _warnedAboutPlaceholder = true;
                }
            }
        }
    }
}

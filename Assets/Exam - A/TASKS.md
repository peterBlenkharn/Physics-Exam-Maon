# Exam A — Drone Rail Navigation and Docking

Read `README.md` in this folder before you start. It describes the scene,
the stage model, and what visible behaviour to expect as you complete each
task. This file lists the tasks in order.

You may consult the module cheatsheets for **weeks 3, 4, 5, 6, and 10**
during the exam. They contain the cubic Bezier formula, AABB overlap
condition, Newton's second law / Euler integration, dot-product facing
check, and non-uniform random technique.

Complete the numbered TASK blocks **in order**. Work only inside the
`// ---------------- TASK N ----------------` markers unless the task says
otherwise.

## Glossary

- **Direction** means a normalised vector (length 1).
- **Speed** means the magnitude of a velocity vector.
- **Alignment** means a dot product between normalised direction vectors.
- **AABB overlap** means overlap on both axes using centres and half-extents.

## Difficulty

Tasks are ordered easiest to hardest. Tasks 1, 2, 3 are intended to be
achievable for every student who has attended the module. The last three
tasks combine more concepts. If you get stuck on a task, skip it, attempt
the next one, and come back — most tasks have only weak dependencies on
earlier ones (see `README.md` for the dependency list).

---

## TASK 1 — `BezierRail.cs` — Evaluate B(t)

Goal:
Implement the cubic Bezier position formula in `BezierRail.Evaluate(float t)`.

Use:
- `P0`, `P1`, `P2`, `P3` (Vector3 control points)
- `t` (clamped to `[0,1]` for you)
- `u = 1 - t` (computed for you)

Formula (week 6 cheatsheet):
`B(t) = u^3 * P0 + 3 * u^2 * t * P1 + 3 * u * t^2 * P2 + t^3 * P3`

Quick check:
- `Evaluate(0)` returns `P0`
- `Evaluate(1)` returns `P3`

Avoid:
- linearly interpolating between `P0` and `P3` (that ignores `P1`, `P2`)

---

## TASK 2 — `DroneRailFollower.cs` — Move along the current rail

Goal:
Advance the drone along `currentRail` over time.

Use:
- `dt` (`Time.deltaTime`, already computed)
- `speedAlongRail` (field above; parameter-space speed)
- `t` (float field)
- `Mathf.Clamp01(t)`
- `currentRail.Evaluate(t)`
- `newPos` (local `Vector3` you assign into — committed to `transform.position`
  for you after the block)

Quick check:
- `t` increases smoothly and stops at `1`
- the drone follows the drawn curve

Avoid:
- treating `t` as world-space distance or arc length
- editing the two "commit motion" lines below the TASK 2 block

---

## TASK 3 — `DroneRailFollower.cs` — Tangent direction by finite difference

Goal:
Compute the drone's forward direction from the curve tangent.

Use:
- `currentRail.Evaluate(t)`
- `currentRail.Evaluate(t + eps)` with a small positive `eps` (for example `0.001f`)
- vector subtraction and `.normalized`
- the `forward` field (`Vector2`)

Formula:
`tangent ≈ B(t + eps) - B(t)`, then `forward = tangent.normalized`.

Quick check:
- on a straight rail, forward points along the rail
- the drone rotates to face along the curve

Avoid:
- using the direction to the dock or to world origin
- pointing the drone at a fixed world axis

---

## TASK 4 — `LaunchIntegrator.cs` — Force to acceleration to velocity to position

Goal:
Implement the full Euler integration pipeline for the yellow test particle
on the left of the scene. This task stands alone — its correctness is not
required for the drone to move on the rail, but it is what the whole rest
of physics-based motion rests on.

Use:
- `netForce` (`Vector2`, already computed above the TASK block)
- `massKg` (float field)
- `dt` (`Time.deltaTime`, already computed above)
- `velocity` (`Vector2` field — persists across frames)
- `transform.position` (`Vector3`)

Steps (in this order):
1. `acceleration = netForce / massKg` (`a = F / m`)
2. `velocity += acceleration * dt` (`v += a * dt`)
3. `transform.position += (Vector3)(velocity * dt)` (`p += v * dt`)

Quick check:
- the yellow square rises from the bottom-left, slows, falls back, and loops
- it is still reaching a reasonable apex — if it spikes off-screen, mass
  or dt ordering is wrong

Avoid:
- multiplying by `dt` twice (e.g. inside acceleration AND velocity)
- updating position **before** velocity — Euler integration is `v` then `p`
- forgetting the cast to `Vector3` on the position update

---

## TASK 5 — `JunctionChoiceTrigger.cs` — Choose the outgoing rail

Goal:
At the junction, pick the outgoing rail with the lowest `CostToGoal`.

Use:
- `outgoingRails` (`BezierRail[]`)
- each rail's `CostToGoal` (lower is better)
- `follower.SetRail(bestRail, true)`

Quick check:
- if one rail has the smallest cost, that rail is always chosen

Avoid:
- always picking `outgoingRails[0]` (that's the placeholder)
- picking the highest cost (this is minimum selection, not maximum)

---

## TASK 6 — `ObstacleSpawner.cs` — Non-uniform spawn height

Goal:
Choose a Y spawn value within the allowed band, **biased toward** `hotHeightY`
(the middle of the band). Extremes still spawn, but less often than the
middle.

Use:
- `hotHeightY` (middle of the band)
- `bandHalfHeight` (half the band height)
- `Random.value` (returns `0..1` uniform)

Hint:
- Averaging two `Random.value` samples gives a value in `[0,1]` biased toward
  `0.5`. Remap that biased 0..1 into
  `[hotHeightY - bandHalfHeight, hotHeightY + bandHalfHeight]`.

Quick check:
- spawns still cover the whole band
- the middle is visibly more common than the extremes

Avoid:
- leaving the uniform `Random.Range(...)` placeholder
- clamping everything to `hotHeightY` (removes the extremes entirely)

---

## TASK 7 — `AABB2D.cs` — AABB overlap

Goal:
Return `true` when two axis-aligned boxes overlap, otherwise `false`.

Use:
- `aCenter`, `aSize`, `bCenter`, `bSize`
- half-extents: `halfA = aSize * 0.5`, `halfB = bSize * 0.5`
- `Mathf.Abs(...)`

Overlap condition (week 4 cheatsheet):
`abs(aCenter.x - bCenter.x) <= (halfA.x + halfB.x)` AND
`abs(aCenter.y - bCenter.y) <= (halfA.y + halfB.y)`

Quick check:
- two boxes with the same centre overlap
- two boxes far apart on x or y do not overlap

Avoid:
- comparing full sizes instead of half-extents
- checking only one axis (both axes must overlap)

Why this matters at runtime:
This method is called every frame by `DroneCollisionAABB.cs` during Stage 3.
While the placeholder is in place, the drone clips through obstacles silently.
The Console prints a warning on Play to flag this.

---

## TASK 8 — `DroneDockingGate.cs` — Safe docking

Goal:
When the drone enters the dock trigger in Stage 3, decide whether the dock
succeeds or fails. Success requires **both**: slow enough **and** aligned
with the dock direction.

Use:
- `drone.Velocity`, `drone.Speed` (magnitude of `Velocity`)
- `drone.Forward`
- `dockDirectionWorld`, `maxDockSpeed`, `minAlignmentDot`
- `Vector2.Dot(a, b)`, `.normalized`
- `stageManager.MarkComplete()` on success
- `stageManager.MarkFailed()` on failure

Steps:
1. `speed = drone.Speed`
2. `alignment = Vector2.Dot(drone.Forward.normalized, dockDirectionWorld.normalized)`
3. if `speed <= maxDockSpeed` AND `alignment >= minAlignmentDot` → `MarkComplete()`, else → `MarkFailed()`

Quick check:
- slow + aligned succeeds (Complete)
- fast or misaligned fails (Failed)

Avoid:
- using `(dock position - drone position)` for alignment — this task
  compares **facing directions**, not positions
- forgetting to normalise before the dot product
- using OR where AND is needed

Depends on Task 3:
If Task 3 is not implemented, `drone.Forward` is stuck pointing up and this
test only succeeds for upward docks.

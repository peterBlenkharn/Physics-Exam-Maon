# Exam B — Rocket Launch and Asteroid Dock

Read `README.md` in this folder before you start. It describes the scene,
the stage model, controls, and what visible behaviour to expect as you
complete each task.

You may consult the module cheatsheets for **weeks 3, 4, 5, 6, and 10**
during the exam. They contain vector magnitude / normalisation / dot
product, Newton's second law / Euler integration, AABB overlap, and
non-uniform random techniques.

Complete the numbered TASK blocks **in order**. Work only inside the
`// ---------------- TASK N ----------------` markers unless the task says
otherwise.

## Glossary

- **Direction** means a normalised vector (length 1).
- **Speed** means the magnitude of a velocity vector.
- **AABB overlap** means overlap on both axes using centres and half-extents.

## Difficulty

Tasks are ordered easiest to hardest. Tasks 1, 2, 3 should be achievable
for every student who has attended the module — Task 1 is a small vector
normalisation, Task 2 is a one-line multiply, Task 3 is a stage-gated
formula substitution. Task 4 (Euler integration) is the core physics step
and is required for the rocket to move at all — but it is explicitly
scaffolded step-by-step.

---

## TASK 1 — `RocketController.cs` — Input to thrust direction and amount

Goal:
Convert the raw `input` vector into a unit direction and an amount in
`[0,1]`. The deadzone branch is already done for you.

Use:
- `input` (`Vector2`, raw input)
- `DesiredThrustDirection` (`Vector2` property — assign this)
- `DesiredThrustAmount01` (`float` property — assign this)
- `Vector2.normalized`, `Mathf.Clamp01(...)`

Steps:
1. `DesiredThrustDirection = input.normalized`
2. `DesiredThrustAmount01 = Mathf.Clamp01(input.magnitude)`

Quick check:
- Right on WASD gives direction `(1,0)`, amount ~1
- Up+Right gives a diagonal unit direction, amount ~1
- Gamepad half-tilt gives amount `< 1`

Avoid:
- always outputting `Vector2.up` (the placeholder)
- setting amount to a value `> 1` (diagonal raw input exceeds magnitude 1)

---

## TASK 2 — `RocketPhysics.cs` — Thrust force vector

Goal:
Compute the thrust force from the controller output.

Use:
- `controller.DesiredThrustDirection`
- `controller.DesiredThrustAmount01`
- `CurrentMaxThrust`

Formula:
`thrustForce = direction * (amount * maxThrust)`

Quick check:
- At full input, thrust force magnitude is about `CurrentMaxThrust`
- At zero input, thrust force is `Vector2.zero`

Avoid:
- forgetting to multiply by `amount` (then half-input = full thrust)

---

## TASK 3 — `RocketPhysics.cs` — Drag force (atmosphere only)

Goal:
Apply a linear drag force, but **only** in Stages 1 and 2. In Stage 3
(space) drag is zero.

Use:
- `velocity`
- `linearDragK`
- `stageManager.CurrentStage`
- `ExamStage.Stage1_Atmosphere`, `ExamStage.Stage2_StagingDecision`

Formula:
- Atmosphere: `F_drag = -linearDragK * velocity`
- Space: `F_drag = Vector2.zero`

Quick check:
- In Stages 1 and 2 the rocket feels thick; top speed is limited
- In Stage 3 the rocket coasts (no damping)

Avoid:
- always applying drag (breaks Stage 3)
- never applying drag (the placeholder — Stage 1 feels weightless)

---

## TASK 4 — `RocketPhysics.cs` — Acceleration and Euler integration

Goal:
Implement the full Euler integration pipeline. This is the core physics
step of the whole exam — without it the rocket cannot move at all.

Use:
- `netForce` (already computed)
- `currentMassKg`
- `dt` (`Time.deltaTime`, already computed)
- `velocity`, `acceleration` (fields)
- `transform.position`

Steps (in this order):
1. `acceleration = netForce / currentMassKg` (`a = F / m`)
2. `velocity += acceleration * dt` (`v += a * dt`)
3. `transform.position += (Vector3)(velocity * dt)` (`p += v * dt`)

Quick check:
- Lift-off arc looks smooth
- Cutting thrust doesn't teleport the rocket; inertia is correct
- No frame-rate jitter

Avoid:
- multiplying by `dt` twice
- updating position before velocity (Euler is `v` then `p`)
- forgetting the cast to `Vector3` on the position update

---

## TASK 5 — `StagingSystem.cs` — Jettison (one-shot + altitude gate)

Goal:
Allow jettison only when all three conditions hold: the J key was pressed
this frame, the rocket is above `minAltitudeToJettison`, and the rocket
hasn't already jettisoned (this third check is done for you).

Use:
- `controller.JettisonPressedThisFrame`
- `transform.position.y`
- `minAltitudeToJettison`

Steps:
Set `canJettison = true` only when:
- `controller.JettisonPressedThisFrame` is `true`, AND
- `transform.position.y >= minAltitudeToJettison`

Quick check:
- Pressing J on the pad does nothing
- Pressing J above min altitude jettisons once
- Pressing J again after jettison does nothing

Avoid:
- leaving `canJettison = false` (the placeholder)
- using `Input.GetKey(...)` directly — the controller already does this

---

## TASK 6 — `AsteroidSpawner.cs` — Non-uniform spawn height

Goal:
Choose a Y spawn value within the band, **biased toward** `bandCenterY`
(middle of the band). Extremes still appear, but less often.

Use:
- `bandCenterY`
- `bandHalfHeight`
- `Random.value`

Hint:
- Averaging two `Random.value` samples gives a value in `[0,1]` biased
  toward `0.5`. Remap into `[bandCenterY - bandHalfHeight, bandCenterY + bandHalfHeight]`.

Quick check:
- spawns cover the whole band
- the middle is visibly more common than the edges

Avoid:
- leaving the uniform `Random.Range(...)` placeholder
- clamping everything to `bandCenterY`

---

## TASK 7 — `Asteroid.cs` — AABB overlap

Goal:
Implement `OverlapsAABB(Asteroid other)` — return `true` when this asteroid
and `other` overlap as axis-aligned boxes.

Use:
- `transform.position`, `size`
- `other.transform.position`, `other.Size`
- `Mathf.Abs(...)`

Overlap condition (week 4 cheatsheet):
`abs(dx) <= (halfA.x + halfB.x)` AND `abs(dy) <= (halfA.y + halfB.y)`
where half-extents come from `size * 0.5`.

Quick check:
- two asteroids at the same position overlap
- two asteroids far apart do not overlap

Avoid:
- comparing full sizes instead of half-extents
- checking only one axis

Why this matters at runtime:
`AsteroidCollisionManager.cs` calls this every frame during Stage 3.
While the placeholder is in place, asteroids never annihilate each other.
The Console prints a warning on Play to flag this.

---

## TASK 8 — `AsteroidCollisionManager.cs` — Destroy on overlap

Goal:
Use Task 7's `OverlapsAABB` to destroy both asteroids when they overlap.

Use:
- `A.OverlapsAABB(B)`
- `Destroy(A.gameObject)`, `Destroy(B.gameObject)`

Quick check:
- two asteroids that visibly touch annihilate each other
- with the Task 7 placeholder, nothing is ever destroyed

Avoid:
- destroying only one of the two
- destroying objects that were already destroyed this frame

---

## TASK 9 — `DockingGate.cs` — Safe docking speed check

Goal:
When the rocket enters the dock trigger in Stage 3, mark the stage Complete
if the rocket is slow enough; otherwise mark it Failed.

Use:
- `rocketPhysics.Speed`
- `maxDockSpeed`
- `stageManager.MarkComplete()` on success
- `stageManager.MarkFailed()` on failure

Steps:
- if `rocketPhysics.Speed <= maxDockSpeed` → `MarkComplete()`
- else → `MarkFailed()`

Quick check:
- Slow approach ends the run as Complete
- Fast approach ends the run as Failed

Avoid:
- flipping the comparison (`>` instead of `<=`)
- comparing magnitude-squared without squaring `maxDockSpeed`

Depends on Task 4:
`rocketPhysics.Speed` is derived from the velocity Task 4 integrates. If
Task 4 is unimplemented, `Speed` is always `0` and this always passes.

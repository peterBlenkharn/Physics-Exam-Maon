# Exam A — Drone Rail Navigation and Docking

Read this before you open `TASKS.md`. It describes what the project is, what
you should expect to see on Play, what order to work in, and where each task
fits. Reading this first is the single cheapest thing you can do to avoid
getting stuck.

## What this project is

You are completing the physics and control logic for a small drone that
navigates a three-stage rail system and docks at a station at the far end of
the scene.

The scene is built around three stages managed by `DroneStageManager`:

1. **Stage 1 — Entry Rail.** The drone starts at the bottom of the scene and
   follows a single cubic Bezier rail upward. Stage 1 ends when the drone
   reaches a set height (`yToEnterStage2`, default `3.5`).
2. **Stage 2 — Junction Choice.** When the drone enters the junction trigger,
   it must pick one of several outgoing rails based on each rail's
   `CostToGoal`. Stage 2 is short — it only exists to make the choice.
3. **Stage 3 — Corridor and Dock.** Obstacles spawn in a height band and move
   through the corridor. The drone must survive them and dock safely at the
   station at the end. Stage 3 has a **25-second time limit** enforced by the
   stage manager.

There is also a small separate physics sandbox object — the `LaunchIntegrator`
yellow square on the left of the scene. It is not part of the drone's flight;
it exists only so you can demonstrate that your Euler integration pipeline is
correct (Task 4). The square auto-creates itself on Play.

## What you edit

You complete eight numbered TASK blocks, one per topic, across seven scripts.
The full list with goals is in `TASKS.md`.

**Do not edit outside the `// TASK N` markers unless a task explicitly says
otherwise.** Everything outside those markers is scaffold code you need to
compile; changing it is what tends to break the scene.

The task-bearing scripts are:

- `BezierRail.cs`
- `DroneRailFollower.cs` (two tasks live here)
- `LaunchIntegrator.cs`
- `JunctionChoiceTrigger.cs`
- `ObstacleSpawner.cs`
- `AABB2D.cs`
- `DroneDockingGate.cs`

The runtime-only scripts under `Scripts/RUNTIME/` (`DroneStageManager`,
`DroneCollisionAABB`, `ObstacleAABB`, `ObstacleMover`) are not assessed and
you do not need to read them in detail — but `DroneStageManager` is what
changes `CurrentStage` and what you will call `MarkComplete()` / `MarkFailed()`
on in Task 8.

## Visible behaviour on Play

When the exam scaffold is **completely unsolved** (no tasks done), pressing
Play should show:

- the drone sitting still at the start of the entry rail
- the rails drawn as cyan curves in the Scene view
- the junction trigger as a yellow wire cube
- the red obstacle-spawn band outline in the Scene view
- a yellow `LaunchIntegrator` square on the left — also sitting still
- no movement anywhere — this is expected when everything is unimplemented

As you complete tasks, visible behaviour should change as follows:

| Task done | What changes on Play |
|---|---|
| 1 — Bezier evaluation | Nothing visibly (enables everything below). |
| 2 — Advance t | Drone begins moving along the rail. |
| 3 — Tangent direction | Drone rotates to face along the curve. |
| 4 — Integration | Yellow square bobs up, slows, and falls back; loops. |
| 5 — Junction choice | Drone picks the lowest-cost rail at the junction. |
| 6 — Non-uniform spawn | Obstacles cluster visibly toward the middle of the band. |
| 7 — AABB overlap | Collisions in Stage 3 now end the run; before this, they don't. |
| 8 — Dock alignment | Approaching the dock slow + aligned ends the run as Complete. |

If a task is done but you can't see the expected change, the most common cause
is that an earlier task it depends on is not yet done. See next section.

## Tasks depend on each other

Some tasks use the output of earlier ones. If an earlier one is wrong, the
later one will look wrong even when your code is correct:

- **Task 2 depends on Task 1.** Advancing `t` only moves the drone if
  `Evaluate(t)` returns a real position.
- **Task 3 depends on Task 2.** A finite-difference tangent is only meaningful
  if `t` is actually increasing.
- **Task 7 is used by the runtime collision system.** If Task 7 returns the
  placeholder `false`, the drone will clip straight through obstacles in
  Stage 3. The Console will print a warning on Play when this is the case.
- **Task 8 depends on Task 3.** The docking alignment check reads
  `drone.Forward`. If Task 3 is not implemented, `drone.Forward` is stuck
  pointing up and the alignment will only succeed for dock directions that
  happen to point up.

**Work in order.** If you skip a task, come back to it before moving on.

## Time pressure

Stage 3 has a 25-second time limit. If the timer runs out, the run is marked
failed. This does not affect your marks — **you are marked on code
correctness, not on reaching the dock under timer**. It is useful to know the
timer exists so that you don't misread a "timer expired" outcome as "my code
is wrong".

## Controls and console

- Play / Stop in the Unity editor as normal.
- The Console will print a warning at Play-time if Task 7 is still the
  placeholder — this is a deliberate signal to remind you the AABB task is
  consumed by runtime collision.
- The inspector on the drone and stage manager shows live values of `t`,
  `speed`, stage, and `Stage3TimeRemaining` — check these when debugging.

## Cheatsheets

You should make use of the module cheatsheets, lab tasks, and lecture content during this open-book exam. The relevant content is:

- vector magnitude, normalisation, and the dot product (weeks 3, 7)
- AABB overlap condition using half-extents (week 4)
- Newton's second law and Euler integration (week 5)
- cubic Bezier formula in expanded form (week 6)
- non-uniform random techniques (week 10)

If you are stuck on a formula, check the cheatsheet **before** attempting to derive it from memory under time pressure.

## Start here

1. Read `TASKS.md`.
2. Skim the task-bearing scripts in order (they are short).
3. Implement the tasks in order. Run the scene after each task to verify the
   visible behaviour matches the table above.

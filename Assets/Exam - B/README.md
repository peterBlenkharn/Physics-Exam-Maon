# Exam B — Rocket Launch and Asteroid Dock

Read this before you open `TASKS.md`. It describes what the project is, what
to expect on Play, and the order in which tasks become visible.

## What this project is

You are completing the flight physics, staging, and docking logic for a
rocket that launches from the pad, decides when to jettison its first stage,
then navigates an asteroid belt to dock at a station.

The scene is built around three stages managed by `StageManager`:

1. **Stage 1 — Atmosphere.** The rocket launches from the pad. Atmospheric
   drag applies. Stage 1 ends when the rocket reaches `altitudeToEnterStage2`
   (default `4`).
2. **Stage 2 — Staging Decision.** The rocket is high but still in the
   atmosphere. Drag still applies. The player can jettison the first stage
   above `minAltitudeToJettison`. Stage 2 ends when the rocket reaches
   `altitudeToEnterStage3` (default `9`).
3. **Stage 3 — Asteroid Band and Dock.** Asteroids spawn in a band and move
   through the playfield. Drag is zero in space. The rocket must dock safely
   at the station. Stage 3 has a **25-second time limit**.

## What you edit

You complete nine numbered TASK blocks across eight scripts. `TASKS.md` has
the full list. Do not edit outside the `// TASK N` markers unless a task
says otherwise.

Task-bearing scripts:

- `RocketController.cs` (Task 1)
- `RocketPhysics.cs` (Tasks 2, 3, 4)
- `StagingSystem.cs` (Task 5)
- `AsteroidSpawner.cs` (Task 6)
- `Asteroid.cs` (Task 7)
- `AsteroidCollisionManager.cs` (Task 8)
- `DockingGate.cs` (Task 9)

Non-assessed runtime scripts: `StageManager.cs`, `AsteroidBandGizmo.cs`.
You do not need to edit these, but `StageManager` is where `MarkComplete()`
and `MarkFailed()` live — you call these from Task 9.

## Controls

- **WASD / Arrow keys** — thrust direction (input → Task 1)
- **J** (by default) — jettison (Task 5 checks this)

## Visible behaviour on Play

When the exam scaffold is **completely unsolved**:

- the rocket sits on the pad
- pressing keys does nothing (placeholder input returns zero amount)
- nothing spawns in the asteroid band

As you complete tasks:

| Task done | What changes on Play |
|---|---|
| 1 — Input to thrust | Pressing keys produces a directed thrust (amount goes to 1). |
| 2 — Thrust force | Thrust becomes a real force vector scaled by max thrust. |
| 3 — Drag | Rocket feels draggy in Stages 1 and 2, floaty in Stage 3. |
| 4 — Integration | Rocket actually moves: rises, arcs, falls under gravity. |
| 5 — Jettison | Pressing J above min altitude changes mass and max thrust. |
| 6 — Non-uniform spawn | Asteroids cluster toward band centre; fewer at edges. |
| 7 — AABB overlap | Task 7 return value enables Task 8 below. |
| 8 — Destroy on overlap | Overlapping asteroids annihilate each other. |
| 9 — Docking gate | Slow approach to the dock in Stage 3 ends the run as Complete. |

## Tasks depend on each other

- **Task 2 depends on Task 1.** Thrust force depends on the direction and
  amount output of the controller.
- **Task 4 depends on Tasks 2 and 3.** The rocket can only move if the net
  force is correct. If drag is wrong it looks wrong in Stage 1/2 only.
- **Task 8 depends on Task 7.** `OverlapsAABB` is called from the collision
  manager; while it returns the placeholder `false`, nothing ever collides.
- **Task 9 uses `rocketPhysics.Speed`** which is derived from Task 4's
  velocity. If integration is broken, the docking check gets a zero or
  garbage speed.

Work in order.

## Time pressure

Stage 3 has a 25-second time limit. Running out marks the run failed. You
are marked on **code correctness**, not on reaching the dock under the
timer, but the timer exists to make the scene finish cleanly on Play.

## Cheatsheets

You may consult the module cheatsheets for **weeks 3, 4, 5, 6, and 10**.
They contain vector magnitude / normalisation / dot product, AABB overlap,
Newton's second law / Euler integration, and non-uniform random techniques.

## Start here

1. Read `TASKS.md`.
2. Skim the task-bearing scripts in order.
3. Implement tasks in order, verifying visible behaviour after each.

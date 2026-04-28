# Exam A — Pedagogical Diagnosis & Refactor Plan

Author: Pedagogical review pass, April 2026
Scope: `Assets/Exam - A/` (drone / Bezier-rail exam) in `P&M 2026 Exam Main`
Cross-referenced against: `Physics_Maths/` (lectures, cheatsheets, labs, exam/ materials)
Cohort evidence: `mock_2026_review.md`, `mock_2026_marking.csv` (14 submissions)

---

## 0. Critical clarification before anything else

You described the paper as **"Exam A (the rocket ship)"**. The folder `Assets/Exam - A/` does not contain a rocket-ship exam. It contains a **drone-on-Bezier-rail docking** exam (scripts: `BezierRail`, `DroneRailFollower`, `JunctionChoiceTrigger`, `ObstacleSpawner`, `AABB2D`, `DroneDockingGate`, plus RUNTIME support).

The rocket-ship exam (scripts: `RocketController`, `RocketPhysics`, `StagingSystem`, `AsteroidSpawner`, `DockingGate`) lives in `Assets/Exam - B/`.

Your own prior review doc (`MAY_SPLINE_EXAM_SCAFFOLDING_REVIEW.md`) already flagged this: *"the spline paper is currently stored under `Assets/Exam - A` in the Unity project, even though it was described externally as the 'B' exam"*.

**This is not a cosmetic issue.** If you advertise to students that they will sit "Exam A: Rocket Ship" and they open `Assets/Exam - A/` to find a drone on rails, that alone is enough to trigger the panic response you are describing. Either:

- (a) You meant the spline paper and it has been colloquially called "the rocket ship" internally — in which case the rest of this report is targeted correctly. The visible deliverable label still needs tightening.
- (b) You meant the rocket-ship paper (`Assets/Exam - B/`) — in which case the *structural* findings in this report still apply, but the file-by-file refactor needs to be redone against the rocket scripts.

**Please confirm which paper you are sitting as "Exam A" before acting on section 5 below.** The diagnosis in sections 1–4 applies to both.

The rest of this document is written assuming **the spline/drone paper is the one being delivered**, because that is the most recently updated one in the repo, it is the one your prior reviews are about, and its `TASKS.md` has already been partially revised based on those reviews.

---

## 1. Executive summary

Mock cohort: mean **7.36 / 50** (14.7%). Highest 41/50, lowest 0/50. On the mock, tasks 5, 6 and 7 returned zero marks for 12, 13 and 12 of the 14 students respectively. This is not ordinary "exam is hard" failure. It is systemic scaffolding failure.

The single root cause is not "the questions are too hard". The maths being tested is directly taught. The problem is that **the entire path from student opens the project to student writes working C# is full of friction that compounds under time pressure**. Each of these items is individually survivable. Stacked together they produce the observed panic behaviour:

1. **There is no README or brief inside `Assets/Exam - A/`.** The only student-facing document is `TASKS.md`. Students open the project, see a Unity scene, and have no written explanation of what game they are looking at, what "complete" means, or what order stages happen in. Compare with `Exam - B/` which also has only `TASKS.md` — same problem there. The uploaded README/STAFF_SETUP you showed me belong to a different (earlier, salvage) exam variant.
2. **Task wording is scaffold-consistent in the `.md` but inconsistent with what the student actually sees in code.** The `TASKS.md` says "Task 2 — DroneRailFollower.cs — Move along the current rail", but the code block spans both Task 2 *and* Task 3 in the same `Update()` method with shared state, and Task 2's placeholder `Vector3 newPos = transform.position;` sits *outside* the `// -------------- END TASK 2 --------------` marker. A student following the rule "only edit inside TASK blocks" cannot actually complete Task 2 correctly.
3. **The exam tests at least one concept that was never taught.** Triangular / centre-biased random (Task 5) is not in any lecture, lab, or cheatsheet. The week-10 statistics lecture only covers weighted *table* selection via cumulative distribution, which is a different technique. The `TASKS.md` now includes a `Hint: averaging two Random.value samples gives a middle-biased value in 0..1`. That hint is the full technique. Adding it makes the task survivable, but the teaching gap remains.
4. **There are no pre-exam labs at all.** Every lab file in `Physics_Maths/labs/` is zero bytes. The mock was the first time any student in the cohort encountered the `// --- TASK N START ---` format. Reading that format under time pressure is itself a trained skill.
5. **Later tasks depend on earlier ones silently.** The student's AABB implementation (Task 6) is consumed by `DroneCollisionAABB.cs` (runtime, not a task). If Task 6 is left as placeholder (returns `false`), the drone can clip through obstacles in Stage 3 and the student may conclude "my collision is working because I'm still alive". There is no warning about this in `TASKS.md` or the code.
6. **The release-shaped scaffold still contains several small bugs and noise items** that leak into student confusion: an unused `maxDockSpeed` field in `DroneRailFollower.cs`, the Task 2 placeholder line leaking outside the task markers, and a placeholder that literally no-ops (`Vector3 newPos = transform.position;` → `velocity = (newPos - prevPos) / dt` → `velocity = 0`). These are exactly the sort of things that make students doubt their own code rather than the exam.

The good news: everything in this list is fixable without redesigning the assessment. The maths stays the same. The task allocation stays the same. The mark weighting stays the same. Most of the work is in three files (`TASKS.md`, `DroneRailFollower.cs`, and a new `README.md`), plus a handful of surgical comment/code tweaks in the others.

The single highest-leverage change is **adding a student-facing `README.md` / `EXAM_BRIEF.md` to `Assets/Exam - A/` that describes the scene, the stages, what "success" looks like, and where each task fits**. This costs you roughly one page of prose and should move the bottom half of the cohort meaningfully above zero on the late tasks.

---

## 2. Evidence base

Every claim in this document is anchored to one or more of the following sources. I list them up front so the argument is auditable.

### 2.1 Mock cohort data (primary)

`mock_2026_review.md` and `mock_2026_marking.csv`. 14 submissions. Task-level pattern:

| Task | Topic | Avg / max | Zero-credit | Completion status |
|---|---|---|---|---|
| 1 | Thrust vector input | 1.21 / 4 | 5 / 14 | Attempted by most; finesse issues |
| 2 | Euler integration (F→a→v→p) | 1.14 / 8 | 7 / 14 | Partially attempted; pipeline confusion |
| 3 | Drag force | 0.86 / 5 | 10 / 14 | Mostly skipped |
| 4 | One-shot jettison | 1.50 / 5 | 7 / 14 | Mixed; state-machine errors |
| 5 | Triangular random spawn | 0.50 / 6 | **12 / 14** | Not attempted / left uniform |
| 6 | AABB overlap | 0.50 / 7 | **13 / 14** | Not attempted / returns false |
| 7 | Dock alignment (dot product) | 0.36 / 10 | **12 / 14** | Not attempted / used distance instead |

Critical interpretive note: these are **mock** tasks (submarine context). The current `Exam - A` paper is the spline variant, which tests different specific tasks but the same underlying concept set (Bezier evaluation, parametric motion, tangent, minimum-cost selection, triangular random, AABB overlap, dot-product alignment). The risk signature transfers directly.

### 2.2 Teaching coverage (Physics_Maths)

From a survey of `Physics_Maths/slides/`, `cheatsheets/`, `labs/`, `docs/`:

Taught and exam-relevant:

- Vectors, magnitude, normalisation, dot product as facing check — week 3 lectures + `week03_cheatsheet.tex`.
- AABB overlap using half-extents — week 4, `slides/04_Geometry/04_09_static_aabb_collision.tex`, + week 4 cheatsheet.
- Forces, Newton's second law, Euler integration — week 5 slides + cheatsheet.
- Cubic Bezier evaluation — week 6 slides, `06_03_cubic_bezier.tex`, + week 6 cheatsheet. Formula is in the cheatsheet, in both de-Casteljau and expanded forms.
- Parametric motion (advancing `t`) — week 6, `06_01_parametric_basics.tex`.
- Dijkstra / minimum cost in a graph context — week 9.

Not taught (or taught only in adjacent form):

- **Triangular / middle-biased random using averaged uniform samples.** Week 10 teaches *weighted table selection via cumulative distribution*, which is a structurally different technique.
- **Finite-difference numerical tangent** (tangent ≈ normalise(B(t+ε) − B(t))). Week 6 teaches the *analytical* derivative (3(P₃−P₂) at endpoints), not the numerical approximation. Students can infer the finite-difference form from "two points close together", but it is not directly in the slides or cheatsheet.
- **Minimum-of-array as a standalone selection pattern.** Students have seen Dijkstra, which embeds minimum selection inside a graph frontier. They have not been drilled on "iterate outgoingRails, keep the one with the smallest CostToGoal".

Also structurally missing:

- **All `labs/lab_*.tex` files are empty.** There are no scaffolded practice labs. The mock was the cohort's first experience reading a `// --- TASK N START ---` block. Reading those blocks is a trained skill, and this cohort did not receive the training.
- **Weeks 11 and 12 lecture material is a stub.** Not directly relevant to Exam A, but it matters for the wider picture of how much of the module the students actually saw.

### 2.3 Current Exam A materials (`Assets/Exam - A/`)

- `TASKS.md` — 135 lines; already rewritten in the format recommended by `MAY_SPLINE_PEDAGOGICAL_DETAIL_AND_TASKS_REWRITE.md`. Strong improvement on the pre-rewrite version, but still has several specific problems (see section 3).
- 7 task-bearing scripts. Each has a marked TASK block with a goal, formula / approach, Quick check, and Avoid note. Structure is consistent. Several individual issues remain (see section 3).
- 5 RUNTIME scripts (`DroneStageManager`, `DroneCollisionAABB`, `ObstacleAABB`, `ObstacleMover`, plus the Obstacle prefab).
- `Main Scene - Exam A.unity` — scene is constructed. The drone's `currentRail` **is assigned** (fileID: 141287746), so the scaffold-breaking issue flagged in `MAY_SPLINE_EXAM_SCAFFOLDING_REVIEW.md` has been addressed.
- **No `README.md`, no `EXAM_BRIEF.md`, no student-facing scene explanation in `Assets/Exam - A/`.** The only student-visible document is `TASKS.md`.

### 2.4 Prior review docs (uploaded)

- `MAY_SPLINE_EXAM_SCAFFOLDING_REVIEW.md` — recommendations summary; most P1 items have been actioned in the current `TASKS.md`. The runtime-observability and labeling items have not.
- `MAY_SPLINE_PEDAGOGICAL_DETAIL_AND_TASKS_REWRITE.md` — prescribed the `file / goal / use / check / avoid` structure now in use. Good.
- `MARKING_METHODOLOGY.md` / `MARKING_SCHEME.md` / `README-b87d4ba4.md` / `STAFF_SETUP.md` — these describe the Salvage Drone variant of the exam, not the currently shipping Exam A. They are instructive but not directly applicable; do not ship these docs with the live paper.

---

## 3. Diagnosis — why students panic

This section is the heart of the review. I group findings by the *student experience*, not by file, because the failure mode you are describing — panic, overwhelm, lost orientation — is a cross-file problem.

### 3.1 Orientation failure: the student has no map

**What the student sees when they open the project:**

They get a Unity project with a scene they have not seen before, a scripts folder with 11 C# files, and one document (`TASKS.md`) that tells them to edit numbered TASK blocks in seven specific files. There is no explanation anywhere of:

- What the scene is a model of (drone flying through a scrapyard? Between asteroids? Delivering cargo?). The scripts say "drone" and "rail" and "dock", but the narrative context is unstated.
- What the three `RailExamStage` values mean and how they affect behaviour (stage 1 is entry rail, stage 2 is junction choice, stage 3 is corridor + dock, plus a 25-second timer on stage 3 that silently fails the run).
- What visible behaviour constitutes "progress". A student completing Task 1 correctly will see… nothing different, because Task 2 is unimplemented and `newPos` is still just `transform.position`. The drone does not move until Tasks 1 + 2 are both done, and even then `forward` is stuck at `Vector2.up` until Task 3 is done.
- What debugging affordances exist. There is no on-screen HUD, no debug log guidance, no gizmo key for students.

**Why this causes panic:** a first-year student under time pressure who doesn't see movement after ten minutes of coding assumes they are catastrophically wrong. They start rewriting things that were already correct. The mock data supports this: `PM_MOCK_26_001465287` got 4/4 on Task 1 but 0/8 on Task 2 — Task 1 was solved, Task 2 was abandoned before anything moved.

**The fix is not to show them the answer.** It is to give them a one-page map of the scene and a one-sentence expectation of visible behaviour at each stage. See refactor P0-1 in section 4.

### 3.2 Scaffold leakage: task blocks don't contain the task

**Specific example, `DroneRailFollower.cs`, lines 52–72 (verbatim from current file):**

```csharp
// ---------------- TASK 2 ----------------
// Advance (increment) t over time and move the drone along the rail.
// Use:
// - dt = Time.deltaTime
// - speedAlongRail as speed in parameter space
// - t clamped to [0,1]
// - currentRail.Evaluate(t)
//
// Quick check:
// - t should increase smoothly and stop at 1
// - the drone should follow the drawn curve
//
// Avoid:
// - treating t as world-space distance or arc length
//
// Placeholder: no movement.
// TODO: Implement.
// -------------- END TASK 2 --------------

Vector3 newPos = transform.position; // REPLACE THIS!
```

The line `Vector3 newPos = transform.position;` is **outside** the TASK 2 END marker. The `TASKS.md` rules tell students to *"Only edit the numbered TASK blocks unless a task explicitly says otherwise"*. Following that rule, students cannot complete Task 2, because the thing they need to change is not inside the block. The comment `// REPLACE THIS!` is the only signal, and it contradicts the global rule.

A student reading the block literally will do one of two things:
- Write `t += speedAlongRail * dt; t = Mathf.Clamp01(t); currentRail.Evaluate(t);` inside the block, then nothing happens because the evaluated position is thrown away — their assignment didn't reach `newPos`. They conclude they are wrong.
- Re-read the TASK block three times trying to find the right answer. Waste time. Panic.

**Analogous issue in Task 3:** the placeholder line `forward = Vector2.up;` and the follow-up `transform.up = new Vector3(forward.x, forward.y, 0f);` are both inside the TASK 3 block, but the block is structurally welded to Task 2 via shared state (`prevPos`, `newPos`, `velocity`). A student who hasn't finished Task 2 cannot see whether their Task 3 implementation is correct, because `forward` computed from a non-moving drone gives undefined-direction behaviour.

This is the highest-leverage code fix in the paper. See P0-2 in section 4.

### 3.3 Task wording: precise where it matters, vague where it kills

`TASKS.md` is mostly good. It has a consistent shape, a glossary at the top, clear file/goal/use/check lines. But three specific wordings carry disproportionate damage:

**Task 3.** Current `Use:` block contains `B(t + eps) - B(t)` and `eps means epsilon in this context - just a very small number!`. The second line reads as apologetic. Worse, it tells the student `eps` is a variable name. The task does not say *where* to get `B(t+eps)` from — a student unfamiliar with function objects might write `B(t + eps)` literally as if `B` were defined. The assumption is that the student reads `currentRail.Evaluate(t+eps)` from context, but nothing in the `Use:` block points at that. Compare to Task 2 which explicitly names `currentRail.Evaluate(t)` in its `Use:` list.

Fix: make Task 3 `Use:` list reference `currentRail.Evaluate(t)` and `currentRail.Evaluate(t + eps)` by name, and drop the apologetic "just a very small number" phrasing.

**Task 5.** Current wording:

> *Spawn obstacles across the allowed height band with more spawns near the 'hot height' / middle.*
> *This means*
> *- there should be more stuff spawning at the middle than at the edges of the band*
> *- nothing should be spawning outside the band.*

The `This means` block reads as a correction of the goal, not as a clarification. Students under time pressure can read this as "the goal I just read isn't quite right; the real goal is below". Combine that with the fact that this is the task most-likely-to-be-skipped (12/14 zeros in the mock), and you lose students before they attempt it.

Also: `bandHalfHeight - this is half the height of the band` reads as slightly patronising. The variable name is self-describing.

Fix: compress into one `Goal:` line and let the `Check:` and `Hint:` carry the rest. See P0-4 in section 4 for proposed rewording.

**Task 7.** The current `Avoid:` is good — it explicitly says "do not use (dock position - drone position) for alignment". That directly addresses the mock failure mode. Keep it. But the `Use:` lists five items (`drone.Velocity`, `drone.Forward`, `dockDirectionWorld`, `maxDockSpeed`, `minAlignmentDot`) and does not mention `stageManager.MarkComplete()` or `stageManager.MarkFailed()`. A student may get the speed + alignment logic right but not know how to signal success or failure. The code block has `stageManager.MarkFailed(); // TODO` but `TASKS.md` doesn't surface the completion API.

Fix: add `stageManager.MarkComplete()` and `stageManager.MarkFailed()` to the Task 7 `Use:` list.

### 3.4 Silent dependency chains

Two dependency traps that cost students marks they *could* earn but don't realise they're losing:

**Trap 1: AABB → runtime collision.** `DroneCollisionAABB.cs` calls `AABB2D.Overlaps(...)` every frame in Stage 3. While Task 6's placeholder is `return false`, collisions never fire, the drone clips obstacles, and nothing appears broken to the student. A student who runs out of time and doesn't attempt Task 6 has no visible signal that Task 6 is doing real runtime work.

Consequence: a student who survives Stage 3 without dying may conclude "my collision is working fine" and skip Task 6 as not-urgent. It is urgent. They just never saw the feedback.

Fix options:
- (a) Make `DroneCollisionAABB` log a one-line warning at Start if `AABB2D.Overlaps` is still the placeholder (trivially detectable by calling `AABB2D.Overlaps(Vector2.zero, Vector2.one, Vector2.zero, Vector2.one)` and asserting the result is true; if false, warn).
- (b) Add to the student-facing README a note that Task 6 is consumed by runtime collision detection, so its correctness is *silently* testable.
- (c) Leave a very visible on-screen "⚠ AABB not implemented — collisions disabled" banner while Task 6 returns the placeholder.

Recommendation: (a) + (b). See P1-2.

**Trap 2: Task 3 → Task 7.** The docking alignment test in Task 7 uses `drone.Forward`. That property is populated by Task 3. A student who skipped Task 3 (or left `forward = Vector2.up`) will see Task 7 pass whenever they fly *upwards*, regardless of the dock direction. They may implement Task 7 correctly and still get behaviour that looks wrong, or implement it incorrectly and get behaviour that looks right.

Fix: add one sentence to Task 7 — `Task 7 depends on Task 3. If Task 3 is incomplete, the drone's Forward vector is stuck pointing up, which will make this test behave unexpectedly.`

### 3.5 Code-comment style drift between files

The task-block comments across the seven scripts are not consistently shaped. Representative sample:

| File | Has `Use:` list | Has `Quick check:` | Has `Avoid:` | Reference to `TASKS.md` |
|---|---|---|---|---|
| BezierRail | No (has formula) | Yes | No | No |
| DroneRailFollower (Task 2) | Yes | Yes | Yes | No |
| DroneRailFollower (Task 3) | Partial | Yes | No | No |
| JunctionChoiceTrigger | No (has "Requirements") | Yes | No | No |
| ObstacleSpawner | Partial (has "One acceptable approach") | Yes | No | No |
| AABB2D | Yes (has `Inputs:` + formula) | Yes | No | No |
| DroneDockingGate | Yes (has numbered steps) | Yes | Yes | No |

Individually each block is fine. Collectively, students see six different shapes across seven tasks. When they switch files, their reading strategy has to reset. Under time pressure this is real friction.

Fix: standardise every task block on the same five-part shape (Goal / Use / Hint-if-needed / Quick check / Avoid-if-needed / Placeholder-warning). Match the shape used in `TASKS.md` exactly. See P1-3 for the template.

### 3.6 Noise and dead code

The `MAY_SPLINE_PEDAGOGICAL_DETAIL_AND_TASKS_REWRITE.md` flagged `private float maxDockSpeed = 2.5f;` in `DroneRailFollower.cs` as unused. I checked — it is not in the current file. That was addressed. Good.

Still present and worth cleaning:

- `DroneRailFollower.cs`: `stageManager` field is declared and the `Update()` early-out uses `stageManager.CurrentStage`. Fine. But the student reading the file sees this guard as one more thing to understand. A one-line comment — `// Early-out in complete/failed states; you don't need to edit this.` — removes that cognitive load.
- `DroneStageManager.cs`: student-facing `Students do not need to edit this file` comment exists. Good. But nothing in `TASKS.md` tells students this file exists and what it does. Students discovering `RailExamStage` enum values (Stage1_EntryRail, Stage2_JunctionChoice, Stage3_CorridorAndDock, Complete, Failed) by themselves must reconstruct the scene flow. This should be in the new README.
- `ObstacleSpawner.cs`: scope of the `hotHeightY` vs `bandCenterY` distinction is not motivated. They are literally the same value (both `8f`) in the current scene serialization. A student reading this sees two variables that seem interchangeable and wonders which one they are "supposed" to use. The `TASKS.md` Task 5 lists both, which doesn't help.

Fix: in code and `TASKS.md`, pick one (`hotHeightY`) as the centre-of-bias and drop the other from the task's `Use:` list; keep `bandCenterY` only inside the runtime band computation if needed.

### 3.7 The labelling / branding layer

Minor but real. The current materials have:

- `Assets/Exam - A/` — folder name matches "Exam A" (good).
- `Main Scene - Exam A.unity` — matches (good).
- `TASKS.md` title: `# Physics and Maths for Games - Exam Tasks`. Does not say "Exam A" or "Drone Rail". A student running both A and B during the exam window in the same Unity editor could open the wrong scene.
- `DroneStageManager.cs` enum: `RailExamStage.Stage1_EntryRail`, etc. No residual `Resit` labels (earlier concern resolved). Good.
- The Physics_Maths cheatsheets: week 10 is referenced as `week10_cheatsheet.tex` but the practical availability of "which cheatsheets can I take in" during the exam is not communicated. If students don't know they have the cheatsheets, they don't benefit from them.

Fix: rename the `TASKS.md` H1 to `# Exam A — Drone Rail Navigation and Docking`. Add a one-line opener: `You may consult the module cheatsheets (week 3, 4, 5, 6, 10) during this exam.`

---

## 4. Prioritised refactor plan

Everything below is scoped by impact × effort. P0 items are "I would not ship without these". P1 items move the expected median by several marks. P2 items are nice-to-have.

### P0-1. Add `Assets/Exam - A/README.md` (single biggest impact)

**Why.** The cohort cannot orient itself. No scene description, no stage model, no "what working code looks like". This document closes that gap in one page and costs you nothing structurally.

**What it contains.** See section 5.1 for a full draft.

**Effort.** ~45 minutes to write, no code changes.

**Risk.** Zero. It's additive.

### P0-2. Restructure `DroneRailFollower.cs` so Task 2 and Task 3 are self-contained

**Why.** The current file has the Task 2 placeholder (`Vector3 newPos = transform.position;`) outside the TASK 2 markers, and Task 3 shares state with Task 2 in a way that makes either task uncheckable without the other. This is the single most common "scaffold violation" in the paper.

**What.** See section 5.2 for the proposed rewrite. The assessment stays identical. Only the comment markers and one variable placement change.

**Effort.** ~20 minutes. One file.

**Risk.** Low. Mark scheme unchanged. Need a staff re-test that the refactored scaffold still compiles and produces the same visible behaviour when solved correctly.

### P0-3. Rewrite Task 5 in both `TASKS.md` and `ObstacleSpawner.cs`

**Why.** 12/14 of the mock cohort scored zero on the triangular-random task, and triangular random is not explicitly taught. The current `Hint:` rescues this, but the wording is still friction-heavy and the `Use:` list has an unnecessary duplicate (`hotHeightY` vs `bandCenterY`).

**What.** See section 5.3. Tighten the wording, collapse the `Use:` list, and add a one-sentence explanation of the concept in the in-file comment.

**Effort.** ~15 minutes. Two files.

**Risk.** Low. You might consider promoting the triangular-random technique into a week-10 lecture next year; for the current cohort, the hint is enough.

### P0-4. Add the stage model and completion API to Task 7

**Why.** Students know "slow + aligned = success" but not how to *signal* success. `stageManager.MarkComplete()` and `stageManager.MarkFailed()` are not surfaced in the `TASKS.md` `Use:` list. The code placeholder does call `MarkFailed()`, but a student who doesn't read the placeholder line will miss it.

**What.** Add `stageManager.MarkComplete()` and `stageManager.MarkFailed()` to Task 7's `Use:` list in `TASKS.md`. Ditto in the code comment. See section 5.5.

**Effort.** ~5 minutes. Two files.

**Risk.** None.

### P0-5. Confirm which paper is actually "Exam A"

See section 0. You must resolve this before delivery. If it turns out the rocket-ship paper is "Exam A", re-run this refactor against `Assets/Exam - B/`.

---

### P1-1. Standardise the in-code task-block shape across all seven scripts

**Why.** Six inconsistent block shapes across seven tasks. Students context-switch every time they change file. Under time pressure that costs real minutes.

**What.** Use the same five-line shape everywhere: `Goal / Use / Hint / Quick check / Avoid`. Match the `TASKS.md` shape. See section 5.6 for the canonical template.

**Effort.** ~45 minutes across seven files.

**Risk.** None. Comments only.

### P1-2. Add a runtime self-test for AABB placeholder

**Why.** Task 6 is silent-failing. A student who doesn't implement it sees no visible consequence until the Stage 3 timer runs out or they clip through obstacles. They never learn that Task 6 is on the critical path.

**What.** In `DroneCollisionAABB.cs` `Start()`:

```csharp
private void Start()
{
    bool sameCentreOverlaps = AABB2D.Overlaps(Vector2.zero, Vector2.one, Vector2.zero, Vector2.one);
    if (!sameCentreOverlaps)
    {
        Debug.LogWarning(
            "[Exam A] AABB2D.Overlaps is still the placeholder (Task 6 not implemented). " +
            "Collision detection is disabled. Implement Task 6 in AABB2D.cs.");
    }
}
```

This is non-assessed scaffold — explicit, harmless, and visible in the Console.

**Effort.** ~10 minutes. One file.

**Risk.** Low. Make sure the warning is informational, not error-level, so the scene still runs.

### P1-3. Add a minimal read-only debug HUD

**Why.** Students have no way to see `t`, current stage, or `Stage3TimeRemaining` without opening the inspector. Under exam pressure they won't know the stage 3 timer is ticking silently, or that `t` is stuck at 0 because they didn't update it.

**What.** Add a `DebugHUD.cs` component (non-assessed, in `RUNTIME/`) that uses `OnGUI()` to draw a fixed top-left block: `Stage: Stage1_EntryRail`, `t: 0.23`, `Speed: 3.40`, `Stage3 time: 18.5`. Visible for everyone. Takes ~20 lines.

**Effort.** ~25 minutes. One new file, wire into scene.

**Risk.** Low. Do not log anything students are assessed on (correct answers, collision pass/fail, dock pass/fail). Strict rule: display only observable quantities.

Example content:

```csharp
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [SerializeField] private DroneStageManager stageManager;
    [SerializeField] private DroneRailFollower drone;

    private void OnGUI()
    {
        if (stageManager == null || drone == null) return;
        GUI.Label(new Rect(10, 10, 300, 20), $"Stage: {stageManager.CurrentStage}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Speed: {drone.Speed:F2}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Forward: {drone.Forward}");
        if (stageManager.Stage3Active)
            GUI.Label(new Rect(10, 70, 300, 20), $"Stage 3 time: {stageManager.Stage3TimeRemaining:F1}s");
    }
}
```

### P1-4. Add a visible cross-dependency note to Task 7

**Why.** Task 7 silently depends on Task 3 via `drone.Forward`. See section 3.4, trap 2.

**What.** One line in `TASKS.md` under Task 7's `Check:` section:

> *Depends on Task 3: if Task 3 is not implemented, `drone.Forward` will be stuck pointing up and this test will not behave as expected.*

**Effort.** ~2 minutes.

**Risk.** None.

### P1-5. Fix `ObstacleSpawner.cs` hot-height vs band-centre redundancy

**Why.** Two variables with overlapping meaning; students don't know which to use.

**What.** Decide on one as the bias point. Recommend using `hotHeightY` only, removing `bandCenterY` from the Task 5 `Use:` list, and keeping `bandCenterY` in code only as the midpoint of the physical band for the gizmo.

**Effort.** ~5 minutes.

**Risk.** None.

### P1-6. Add an explicit "cheatsheets are available" line to `TASKS.md`

**Why.** If students forget they can consult week-6 and week-4 cheatsheets, they don't use them.

**What.** One-line opener: *"You may consult the module cheatsheets for weeks 3, 4, 5, 6, and 10 during this exam. The cheatsheets contain the Bezier formula, AABB overlap condition, dot-product facing check, and weighted random formula."*

**Effort.** 1 minute.

**Risk.** None, as long as this matches your actual invigilation policy. Check before shipping.

---

### P2-1. Rebrand `TASKS.md` title and scene title

**Why.** Cosmetic but reduces "am I in the right file?" panic. See section 3.7.

**What.** `# Exam A — Drone Rail Navigation and Docking` as the H1 of `TASKS.md`.

**Effort.** 30 seconds.

### P2-2. Pre-draft a worked lab for next year's cohort

**Why.** The biggest long-term finding in this review is that all `labs/lab_*.tex` files are empty. The mock was the cohort's *first* experience of the task-block format. That's the single biggest structural reason the cohort is underprepared.

**What.** Outside the scope of this refactor. Flagged for next year's module redesign.

**Effort.** Not attempted here.

### P2-3. Decide triangular-random's long-term status

**Why.** It's being tested without being taught. The hint rescues it, but next year you should either add it to week 10 or replace Task 5 with something week 10 does cover.

**What.** Outside the scope of this refactor.

### P2-4. Tighten the marking-scheme doc (not shipped to students)

The uploaded `MARKING_SCHEME.md` is for the Salvage Drone mock variant. You will need the equivalent for the spline exam before release. Structurally it can mirror the submarine one: band descriptors, exemplar code, partial-credit rules.

---

## 5. Before / after — concrete deliverable-shaped diffs

Below are the actual text changes to make. Each is drop-in. Cumulative effort: roughly 2–3 hours.

### 5.1 `Assets/Exam - A/README.md` (new file)

```md
# Exam A — Drone Rail Navigation and Docking

## What this project is

You are completing the physics and control logic for a drone that navigates
a three-stage rail system and docks at a station at the end.

The scene is built around three stages:

1. **Stage 1 — Entry Rail.** The drone starts at the bottom of the scene and
   follows a single rail upward. Stage 1 ends when the drone reaches a set
   height.
2. **Stage 2 — Junction Choice.** When the drone enters the junction trigger,
   it must choose between several outgoing rails based on `CostToGoal`.
   Stage 2 is short — you only need to pick the rail.
3. **Stage 3 — Corridor and Dock.** Obstacles spawn in a height band. The
   drone must survive collisions and dock safely at the station. Stage 3 has
   a 25-second time limit.

## What you edit

You complete seven numbered TASK blocks, one per topic, across seven scripts.
The full list with goals is in `TASKS.md`. Do not edit outside the TASK blocks
unless a task says otherwise.

## Visible behaviour on Play

When the exam scaffold is unsolved (no tasks complete), pressing Play should
show:

- the drone sitting at the start of the entry rail
- three rail curves drawn as blue gizmos in the Scene view
- the junction trigger as a yellow wire cube
- the station and dock trigger at the far end
- the red obstacle-spawn band outline (visible in Scene view)
- the on-screen debug HUD showing the current stage, speed, and forward vector

As you complete tasks, you should see:

- **Task 1 done:** the drone's Evaluate(0) returns the correct start point.
  Not visible on its own yet.
- **Task 2 done:** the drone begins moving along the rail.
- **Task 3 done:** the drone rotates to face along the curve.
- **Task 4 done:** at the junction, the drone picks the lowest-cost rail.
- **Task 5 done:** obstacles spawn with a visible centre-biased distribution.
- **Task 6 done:** collisions now fail the run (MarkFailed). Before Task 6 is
  implemented, collisions are silently disabled.
- **Task 7 done:** when the drone enters the dock trigger in Stage 3 at low
  speed and aligned with the dock direction, the stage completes.

## Tasks depend on each other

Some tasks silently depend on earlier ones:

- Task 3 depends on Task 2 (no movement = no meaningful tangent).
- Task 6 is consumed by the runtime collision system; if Task 6 returns the
  placeholder, Stage 3 collisions are disabled.
- Task 7 uses `drone.Forward` from Task 3; if Task 3 is incomplete, the dock
  alignment test will look wrong.

Work in order.

## Time pressure

Stage 3 has a 25-second time limit enforced by `DroneStageManager`. If the
timer runs out, the run fails. This does not affect your marks — the marks
are on code quality and correctness, not on reaching the dock — but it does
mean you should verify earlier tasks in Stage 1 and Stage 2 without needing
to beat the timer first.

## Controls and console

- Play and Stop in the Unity editor.
- The Console will print a warning if Task 6 is still the placeholder.
- The debug HUD is read-only.

## Cheatsheets

You may consult the module cheatsheets for weeks 3, 4, 5, 6, and 10 during
this exam. They contain the cubic Bezier formula, AABB overlap condition,
dot-product facing check, and weighted random technique.

## Start here

1. Read `TASKS.md`.
2. Skim the seven task-bearing scripts.
3. Implement the tasks in order.
```

### 5.2 `DroneRailFollower.cs` — proposed refactor of Tasks 2 and 3

```csharp
private void Update()
{
    if (stageManager != null && (stageManager.CurrentStage == RailExamStage.Complete || stageManager.CurrentStage == RailExamStage.Failed))
        return;

    if (currentRail == null) return;

    float dt = Time.deltaTime;
    Vector3 prevPos = transform.position;
    Vector3 newPos = prevPos; // default: no movement. Task 2 replaces this.

    // ---------------- TASK 2 ----------------
    // Goal: advance t along the current rail and sample the new position.
    //
    // Use:
    //   - dt (Time.deltaTime, already computed above)
    //   - speedAlongRail (parameter-space speed)
    //   - t (already declared and serialized)
    //   - currentRail.Evaluate(t)
    //
    // Quick check:
    //   - t should stay in [0,1] and stop increasing at 1
    //   - the drone should follow the drawn Bezier curve
    //
    // Avoid:
    //   - treating t as world distance or arc length
    //
    // When you have implemented this correctly, assign the sampled position
    // to `newPos`. Example shape:
    //     t += /* ... */;
    //     newPos = currentRail.Evaluate(t);
    //
    // Placeholder behaviour: newPos stays equal to prevPos (no movement).
    // -------------- END TASK 2 --------------

    transform.position = newPos;
    velocity = (Vector2)((newPos - prevPos) / Mathf.Max(dt, 0.0001f));

    // ---------------- TASK 3 ----------------
    // Goal: compute the drone's forward direction from a finite-difference
    // tangent along the current rail.
    //
    // Use:
    //   - currentRail.Evaluate(t)
    //   - currentRail.Evaluate(t + eps) where eps is a small positive number
    //   - Vector3/Vector2 subtraction and .normalized
    //
    // Quick check:
    //   - on a straight segment, forward should point along the rail
    //   - the drone should rotate to face along the curve
    //
    // Avoid:
    //   - pointing the drone at the dock, the world origin, or a fixed axis
    //   - using the velocity vector directly (it is derived from the same t
    //     you just updated, so it's equivalent at non-zero speed, but on a
    //     stalled frame it will be zero and give you a broken tangent)
    //
    // Placeholder behaviour: forward stuck pointing up.
    forward = Vector2.up; // TODO replace inside this block
    // -------------- END TASK 3 --------------

    transform.up = new Vector3(forward.x, forward.y, 0f);
}
```

Key changes from the current file:

- `Vector3 newPos = prevPos;` is now *inside* the method above the TASK 2 block. The student's job inside the block is to assign a new value to `newPos`. No scaffold-violating edits required.
- The velocity and position commit (`transform.position = newPos; velocity = ...;`) happens *after* Task 2 closes. This is clean: the student never edits the commit line.
- Task 3 is isolated: its only input/output is `forward`. `transform.up` assignment is outside the block, below, just like `velocity` below Task 2.
- Both Quick check blocks now explicitly say what visible behaviour to expect.

### 5.3 `TASKS.md` — Task 5 rewrite

Replace the current Task 5 block with:

```md
## TASK 5 — ObstacleSpawner.cs — Non-uniform spawn height

Goal:
Choose a Y spawn value within the allowed band, biased toward the middle of
the band (`hotHeightY`). Extremes should still spawn, but less often.

Use:
- `hotHeightY` (middle of the band; the bias target)
- `bandHalfHeight` (half the band's total height)
- a non-uniform random value in [0,1]

Hint:
- `(Random.value + Random.value) * 0.5f` returns a value in [0,1] biased
  toward 0.5. Remap that into `[hotHeightY - bandHalfHeight, hotHeightY + bandHalfHeight]`.

Quick check:
- spawns still cover the whole band (extremes do still appear)
- the middle is visibly more common than the edges

Avoid:
- leaving `Random.Range(bandCenterY - bandHalfHeight, bandCenterY + bandHalfHeight)`
  unchanged (this is the placeholder — uniform)
```

And the matching `ObstacleSpawner.cs` task block:

```csharp
// ---------------- TASK 5 ----------------
// Goal: choose a NON-UNIFORM y spawn in [hotHeightY - bandHalfHeight,
//       hotHeightY + bandHalfHeight], biased toward hotHeightY.
//
// Use:
//   - hotHeightY
//   - bandHalfHeight
//   - Random.value (returns 0..1 uniform)
//
// Hint:
//   averaging two Random.value samples gives a 0..1 value biased toward 0.5.
//   Remap that biased 0..1 into the allowed Y band.
//
// Quick check:
//   - spawns cover the whole band
//   - middle is more common than edges
//
// Avoid:
//   - leaving the uniform Random.Range line below unchanged
//
// Placeholder (uniform — WRONG):
float y = Random.Range(hotHeightY - bandHalfHeight, hotHeightY + bandHalfHeight); // TODO
// -------------- END TASK 5 --------------
```

Note: swap `bandCenterY` → `hotHeightY` throughout this block. Keep `bandCenterY` only where it controls the gizmo drawing. This removes the "which variable am I meant to use?" ambiguity.

### 5.4 `TASKS.md` — Task 3 refinement

Replace Task 3's `Use:` block with:

```md
Use:
- `currentRail.Evaluate(t)`
- `currentRail.Evaluate(t + eps)` with a small positive `eps` (for example 0.001f)
- subtraction and `.normalized`
```

And the `Avoid:` block with:

```md
Avoid:
- using the direction to the dock or world origin — this is the curve's tangent
- pointing the drone at a fixed world axis
```

### 5.5 `TASKS.md` — Task 7 `Use:` block

Add the completion API:

```md
Use:
- `drone.Velocity` (or `drone.Speed` for the magnitude)
- `drone.Forward`
- `dockDirectionWorld`
- `maxDockSpeed`
- `minAlignmentDot`
- `stageManager.MarkComplete()` when the dock succeeds
- `stageManager.MarkFailed()` when the dock fails

Check:
- slow + aligned: MarkComplete() is called
- fast or misaligned: MarkFailed() is called
- this task depends on Task 3 — if Task 3 is unimplemented, drone.Forward is
  stuck pointing up
```

### 5.6 Canonical code-comment template for all seven tasks

```csharp
// ---------------- TASK N ----------------
// Goal: <one sentence>.
//
// Use:
//   - <variable 1>
//   - <variable 2>
//   - <API call or constant>
//
// Hint (only if the concept was not fully taught):
//   <one sentence>
//
// Quick check:
//   - <one sentence describing visible behaviour>
//   - <second sentence if useful>
//
// Avoid (only if a specific misconception is likely):
//   - <one sentence>
//
// Placeholder (WRONG):
<one line of placeholder code with // TODO on the same line>
// -------------- END TASK N --------------
```

Apply to all seven tasks. Use existing copy where it matches; reshape the rest to fit. Section 3.5 above lists which tasks are currently non-conforming.

---

## 6. Risk and effort summary

| Item | Effort | Mark-scheme risk | Visible-behaviour risk | Student-confusion risk if not done |
|---|---|---|---|---|
| P0-1 Student README | 45 min | None | None (additive) | High |
| P0-2 DroneRailFollower Task 2/3 restructure | 20 min | None (same assessment) | Low (verify same behaviour on solved scaffold) | High |
| P0-3 Task 5 rewrite | 15 min | None | None | High (12/14 zero in mock) |
| P0-4 Task 7 completion API | 5 min | None | None | Medium |
| P0-5 Confirm paper identity | — | Potentially very high | — | Very high |
| P1-1 Standardise code blocks | 45 min | None | None | Medium |
| P1-2 AABB placeholder warning | 10 min | None | None | Medium |
| P1-3 Debug HUD | 25 min | None | Low (verify in scene) | Medium |
| P1-4 Task 7 dep note | 2 min | None | None | Low-medium |
| P1-5 Hot/band cleanup | 5 min | None | None | Low |
| P1-6 Cheatsheets line | 1 min | Low (verify policy) | None | Low |
| P2-1 Rebrand | 30 s | None | None | Low |
| P2-2/3/4 | Next-year | — | — | — |

Total shipping-critical effort: roughly 2–3 hours. If you can only do one thing in that time, do **P0-1 (the README)**. If you can do two, add **P0-2 (DroneRailFollower restructure)**. Together those two changes address the largest failure vectors in the mock cohort.

---

## 7. Ship checklist

Run this before distributing to students.

1. P0-5 resolved: you have confirmed which paper is Exam A and which is Exam B.
2. `Assets/Exam - A/README.md` exists and matches section 5.1.
3. `Assets/Exam - A/TASKS.md` H1 is `Exam A — Drone Rail Navigation and Docking`.
4. `Assets/Exam - A/TASKS.md` contains the cheatsheet availability line.
5. `DroneRailFollower.cs` Task 2/3 restructure applied; solved scaffold still produces correct drone motion and facing.
6. `ObstacleSpawner.cs` uses `hotHeightY` in the task block, not `bandCenterY`.
7. `DroneDockingGate.cs` and `TASKS.md` Task 7 both surface `MarkComplete()` / `MarkFailed()`.
8. All seven task blocks conform to the canonical template from section 5.6.
9. `DroneCollisionAABB.cs` prints the placeholder-detection warning at Start when Task 6 is unsolved.
10. `DebugHUD.cs` is attached to a scene object and shows stage / speed / forward / stage-3 timer.
11. Scene still opens cleanly on a fresh clone: no missing script references, `currentRail` is assigned on the drone.
12. Unsolved scaffold still compiles and still runs with a visible Play experience (drone sits on entry rail, rails drawn, no runtime null-ref exceptions).
13. Solved reference implementation compiles and completes the run end-to-end.
14. Marking scheme (separate doc) matches the task breakdown used here.

---

## 8. Honest notes on what this review does not do

1. **I did not watch a student attempt the live paper.** The mock evidence is from the submarine variant. The failure *pattern* transfers (same cohort, same module, overlapping concept set), but task-specific friction may differ. A single 10-minute observation of one student attempting the current Exam A would validate or invalidate specific wordings in a way that this paper review cannot.

2. **I did not re-mark any submission.** I took `mock_2026_review.md`'s marks as given. The marking methodology is reasonable and auditable.

3. **I did not rewrite the marking scheme.** The uploaded `MARKING_SCHEME.md` is for the Salvage Drone variant. You need the spline-equivalent separately. Structurally it can mirror the submarine one.

4. **I did not evaluate the difficulty of the paper relative to other first-year games-dev modules.** The question "is this a reasonable exam for a first-year" is outside the scope of this doc. The question I answered is "given this exam and this cohort, why are they failing". On that: the exam is reasonable; the path to solving it is not.

5. **Triangular random is still being tested without being taught.** My recommendation (keep the hint, plan to teach it next year) is a workaround, not a fix. A stricter reviewer would say the task should be replaced with a weighted-cumulative distribution task that matches week 10 exactly.

6. **No labs exist.** This is the single biggest structural problem in the wider module. It is out of scope for an exam refactor but it is worth naming: under current conditions, the exam itself is the first place students see a TASK block. That is not a failing of the exam — it is a failing of the preceding weeks. Labs for next year are the best investment.

---

End of document.

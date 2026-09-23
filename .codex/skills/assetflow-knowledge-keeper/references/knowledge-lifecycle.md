# AssetFlow Knowledge Lifecycle Reference

Open this reference for milestone work, large issues, planned-state validation,
start gates, previous milestone closure, or knowledge updates after meaningful
architecture/product changes.

## What to Capture

Capture durable facts that reduce repeated discovery:

- system overview, architecture, services, bounded contexts, and responsibilities
- current milestone, completed major work, active major work, and known gaps when
  they affect engineering context
- important directories, abstractions, domain terminology, and conventions
- database ownership, PostgreSQL schemas, important tables, migrations, and query
  patterns
- Kafka topics, event contracts, producers, consumers, event flows, and
  idempotency rules
- gRPC services, REST APIs, SignalR hubs, blob storage usage, background workers,
  channel pipelines
- event-sourced aggregates, projections, infrastructure, testing strategy,
  constraints, and ADR links
- planned behavior from specs, issues, and milestones that future code checks
  must compare against actual implementation

## Lifecycle

Before meaningful work:

1. Read the relevant files in `docs/knowledge/`.
2. Use that knowledge to guide code inspection.
3. Verify critical facts against the repository and GitHub before relying on
   them.

After meaningful changes:

1. Determine whether project knowledge changed.
2. Update affected knowledge files.
3. Remove or correct stale information.
4. Add important architectural decisions or ADR links.
5. Run planned-state validation when the work is a milestone, big issue, or
   feature.
6. Keep documentation synchronized with code.

## Planned-State Validation

After finishing a milestone, large issue, or feature, perform planned-state
validation before calling the work complete:

- identify the original plan, GitHub issue, milestone, acceptance criteria, and
  any local plan/spec docs
- inspect the implemented code, tests, configuration, docs, and runtime behavior
  where practical
- verify the current app state matches exactly what was planned, or record every
  deliberate deviation and follow-up
- verify tests and other checks cover the planned behavior at an appropriate risk
  level
- update `docs/knowledge/` with the final state, decisions, and known gaps
- update GitHub status when access is available

Do not skip this for milestones, big issues, or features.

## Previous Milestone Closure Hook

Before planning a new milestone or creating its GitHub issues, verify the
previous milestone is closed well enough for future agents to start from
maintained docs:

- an implementation report exists for the previous milestone, unless there is no
  previous milestone
- `docs/knowledge/` captures durable facts from the completed milestone and does
  not contradict code, tests, configuration, or GitHub state
- local status docs, plans, reports, and GitHub tracking match the completed
  implementation

If those artifacts already exist and are current, record that the closure hook
passed and avoid duplicating them. If any are missing, stale, or too thin, update
them before the next milestone plan or issues are created.

## Milestone Start Gate

Before starting milestone implementation:

- verify the previous milestone closure hook passed
- identify the milestone plan in `docs/plans/`, the GitHub milestone, related
  issues, and any contract/spec docs
- verify the plan defines the milestone goal, scope, non-goals, deliverables,
  acceptance criteria, task breakdown, dependencies, risks, testing strategy,
  and tracking
- verify every implementation-blocking decision is decided, or explicitly
  deferred out of scope
- verify the plan has no unresolved placeholders such as `TBD`, `TODO`,
  `unknown`, `undecided`, or blocking open questions
- update `docs/knowledge/` with durable planned behavior future work must compare
  against
- update GitHub status when access is available

If this start gate fails, do not start implementation. Continue planning until
the milestone is ready.

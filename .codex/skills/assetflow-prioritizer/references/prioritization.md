# AssetFlow Prioritization Reference

Open this reference for milestone, multi-phase, risky, or dependency-heavy task
sequencing.

## Criteria

Build an order that accounts for:

- dependencies and blockers
- architectural foundations
- business value and user-visible progress
- uncertainty and technical risk
- feedback speed and testing ability
- integration, migration, and infrastructure requirements

## Sequencing Heuristics

- Establish foundations before vertical slices that depend on them.
- Prove risky persistence, messaging, realtime, or integration boundaries early.
- Prefer small slices that produce testable feedback.
- Keep cleanup after the behavior it protects unless cleanup is a prerequisite.
- Separate unrelated GitHub-tracked tasks into separate branches and PRs unless
  the user explicitly asks to combine them.

# M9 Agentic OS Memory And Security Model

## Decision

M9 keeps AssetFlow memory local-first and Git-reviewable.

The adopted memory layer is:

- `docs/knowledge/` for curated durable engineering memory
- `docs/plans/` for milestone and task intent
- `docs/specs/` for product, workflow, security, telemetry, and integration specs
- milestone reports for planned-state validation and handoff notes
- the generated Agentic OS dashboard for read-only navigation and status

M9 does not adopt Obsidian, a vector store, external drive synchronization, personal notebook sync, or hosted memory infrastructure. Those options stay deferred until there is a concrete retrieval, collaboration, or portability need that outweighs their security and maintenance cost.

## Evaluated Options

| Option | Value | Risk | M9 Decision |
| --- | --- | --- | --- |
| Curated repo docs | Reviewable, portable, visible in PRs, already used by agents | Can become stale if not maintained | Adopt |
| Obsidian or local note vault | Strong personal knowledge workflow | Can mix private notes with repo memory; sync boundaries unclear | Defer |
| Vector store / embeddings | Useful retrieval for large corpora | Adds infrastructure, retention, prompt-leak, and rebuild questions | Defer |
| Google Drive / Docs | Collaboration and sharing | Requires connector access and external data governance | Plan only |
| Research notebook service | Good for long-running investigations | Connector and export boundaries unclear | Plan only |

## Data Classification

- Public/repo-safe: roadmap, plans, specs, reports, issue references, PR links, verification commands, generated dashboard metadata.
- Internal project context: architecture notes, operational decisions, known gaps, local workflow runbooks.
- Restricted: credentials, tokens, private account data, personal notes, raw AI prompts, raw AI transcripts, customer data, provider billing exports.

Restricted data must not be committed to Git or embedded in generated dashboard output.

## Storage Rules

- Curated facts belong in `docs/knowledge/overview.md` unless a narrower knowledge file becomes justified.
- Plans belong in `docs/plans/`.
- M9 operating-layer contracts belong in `docs/specs/`.
- Generated dashboard output belongs under `tools/agent-dashboard/dist/` and remains ignored by Git.
- Local scratch exports should stay outside the repository unless they have been sanitized and intentionally converted into a reviewed spec/report.

## Access And Retention

- Git history is the retention mechanism for repo-safe memory.
- Sensitive material must be kept out of Git so it is not retained accidentally.
- External connector data must be summarized into repo-safe Markdown before it becomes project memory.
- Future agents should read repo memory first, then verify important claims against code, GitHub, and current docs.

## Review Checklist

- Does the change introduce any credential, token, account identifier, raw prompt, raw transcript, private note, or customer data?
- Is the source of each durable claim clear enough to verify?
- Is generated output ignored or excluded from the commit?
- Are unavailable telemetry fields shown as unavailable rather than estimated?
- Does the dashboard present mutating operations only as prompts, links, or command text?

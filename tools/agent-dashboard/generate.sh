#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/../.." && pwd)"
output_dir="${1:-${script_dir}/dist}"
output_file="${output_dir}/index.html"

mkdir -p "${output_dir}"

run_git() {
  git -C "${repo_root}" "$@" 2>/dev/null || true
}

html_escape() {
  local value="${1:-}"
  value="${value//&/&amp;}"
  value="${value//</&lt;}"
  value="${value//>/&gt;}"
  value="${value//\"/&quot;}"
  value="${value//\'/&#39;}"
  printf '%s' "${value}"
}

file_state() {
  local path="$1"
  if [[ -e "${repo_root}/${path}" ]]; then
    printf 'Available'
  else
    printf 'Missing'
  fi
}

branch="$(run_git branch --show-current)"
if [[ -z "${branch}" ]]; then
  branch="detached"
fi

head_sha="$(run_git rev-parse --short HEAD)"
if [[ -z "${head_sha}" ]]; then
  head_sha="unavailable"
fi

remote_url="$(run_git remote get-url origin)"
if [[ -z "${remote_url}" ]]; then
  remote_url="unavailable"
fi

status_output="$(run_git status --short)"
if [[ -z "${status_output}" ]]; then
  working_tree_state="Clean"
else
  working_tree_state="Has local changes"
fi

generated_at="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"

cat > "${output_file}" <<HTML
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>AssetFlow Agentic Control Dashboard</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f7f8fa;
      --surface: #ffffff;
      --ink: #1d2430;
      --muted: #5d6878;
      --line: #d9dee7;
      --accent: #0b6f6a;
      --accent-strong: #084f4b;
      --warn-bg: #fff5d7;
      --warn-ink: #6b4a00;
      --ok-bg: #e8f5ee;
      --ok-ink: #145c35;
    }

    * {
      box-sizing: border-box;
    }

    body {
      margin: 0;
      font-family: ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      background: var(--bg);
      color: var(--ink);
      line-height: 1.5;
    }

    a {
      color: var(--accent-strong);
      text-decoration-thickness: 1px;
      text-underline-offset: 3px;
    }

    .shell {
      width: min(1180px, calc(100% - 32px));
      margin: 0 auto;
      padding: 28px 0 44px;
    }

    header {
      display: grid;
      gap: 16px;
      margin-bottom: 24px;
    }

    h1 {
      margin: 0;
      font-size: clamp(2rem, 6vw, 3.6rem);
      line-height: 1;
      letter-spacing: 0;
    }

    h2,
    h3 {
      margin: 0;
      letter-spacing: 0;
    }

    p {
      margin: 0;
    }

    .summary {
      max-width: 780px;
      color: var(--muted);
      font-size: 1.05rem;
    }

    .nav {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 4px;
    }

    .nav a,
    .chip {
      display: inline-flex;
      align-items: center;
      min-height: 36px;
      padding: 7px 10px;
      border: 1px solid var(--line);
      border-radius: 6px;
      background: var(--surface);
      color: var(--ink);
      text-decoration: none;
      font-size: 0.92rem;
    }

    .grid {
      display: grid;
      grid-template-columns: repeat(12, 1fr);
      gap: 14px;
    }

    section,
    .panel {
      background: var(--surface);
      border: 1px solid var(--line);
      border-radius: 8px;
      padding: 18px;
    }

    .span-4 {
      grid-column: span 4;
    }

    .span-6 {
      grid-column: span 6;
    }

    .span-8 {
      grid-column: span 8;
    }

    .span-12 {
      grid-column: 1 / -1;
    }

    .metric-row,
    .link-list,
    .action-list {
      display: grid;
      gap: 10px;
      margin-top: 14px;
    }

    .metric {
      display: grid;
      gap: 2px;
      padding: 10px 0;
      border-top: 1px solid var(--line);
    }

    .metric:first-child {
      border-top: 0;
      padding-top: 0;
    }

    .label {
      color: var(--muted);
      font-size: 0.82rem;
      text-transform: uppercase;
    }

    .value {
      overflow-wrap: anywhere;
      font-weight: 650;
    }

    .badge {
      width: fit-content;
      max-width: 100%;
      padding: 4px 8px;
      border-radius: 6px;
      font-size: 0.82rem;
      font-weight: 650;
    }

    .badge-ok {
      background: var(--ok-bg);
      color: var(--ok-ink);
    }

    .badge-warn {
      background: var(--warn-bg);
      color: var(--warn-ink);
    }

    .card {
      display: grid;
      gap: 6px;
      padding: 12px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: #fbfcfd;
    }

    .card strong {
      overflow-wrap: anywhere;
    }

    .card span,
    .help {
      color: var(--muted);
      font-size: 0.92rem;
    }

    code {
      overflow-wrap: anywhere;
      font-family: ui-monospace, "SFMono-Regular", Consolas, monospace;
      font-size: 0.9em;
    }

    @media (max-width: 860px) {
      .shell {
        width: min(100% - 20px, 760px);
        padding-top: 18px;
      }

      .grid {
        grid-template-columns: 1fr;
      }

      .span-4,
      .span-6,
      .span-8,
      .span-12 {
        grid-column: 1;
      }
    }
  </style>
</head>
<body>
  <main class="shell">
    <header>
      <div class="chip">Generated $(html_escape "${generated_at}")</div>
      <h1>AssetFlow Agentic Control Dashboard</h1>
      <p class="summary">A repo-local control surface for milestone status, durable planning docs, workflow agents, reusable skills, and explicit AI-assisted development launch points.</p>
      <nav class="nav" aria-label="Dashboard sections">
        <a href="#overview">Overview</a>
        <a href="#tracking">Tracking</a>
        <a href="#docs">Docs</a>
        <a href="#agents">Agents</a>
        <a href="#workflow">Workflow</a>
        <a href="#activity">Activity</a>
      </nav>
    </header>

    <div class="grid">
      <section id="overview" class="span-8">
        <h2>Overview</h2>
        <div class="metric-row">
          <div class="metric">
            <span class="label">Current milestone</span>
            <span class="value">M4 - Agentic Control Dashboard MVP</span>
          </div>
          <div class="metric">
            <span class="label">Active task</span>
            <span class="value"><a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/50">#50 Build repo-local dashboard foundation</a></span>
          </div>
          <div class="metric">
            <span class="label">Next planned task</span>
            <span class="value"><a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/51">#51 Add GitHub and local status views</a></span>
          </div>
        </div>
      </section>

      <section class="span-4">
        <h2>Local Checkout</h2>
        <div class="metric-row">
          <div class="metric">
            <span class="label">Repository path</span>
            <span class="value"><code>$(html_escape "${repo_root}")</code></span>
          </div>
          <div class="metric">
            <span class="label">Branch</span>
            <span class="value"><code>$(html_escape "${branch}")</code></span>
          </div>
          <div class="metric">
            <span class="label">HEAD</span>
            <span class="value"><code>$(html_escape "${head_sha}")</code></span>
          </div>
          <div class="metric">
            <span class="label">Working tree</span>
            <span class="value"><span class="badge $(if [[ "${working_tree_state}" == "Clean" ]]; then printf 'badge-ok'; else printf 'badge-warn'; fi)">$(html_escape "${working_tree_state}")</span></span>
          </div>
          <div class="metric">
            <span class="label">Remote</span>
            <span class="value"><code>$(html_escape "${remote_url}")</code></span>
          </div>
        </div>
      </section>

      <section id="tracking" class="span-12">
        <h2>Tracking</h2>
        <p class="help">GitHub remains the source of truth. This foundation links to current M4 tracking; issue #51 will add richer read-only status mapping.</p>
        <div class="link-list">
          <div class="card">
            <strong><a href="https://github.com/JavadEslamibabaheidari/AssetFlow/milestone/5">M4 GitHub milestone</a></strong>
            <span>Open implementation milestone for the dashboard MVP.</span>
          </div>
          <div class="card">
            <strong>M4 issue sequence</strong>
            <span><a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/49">#49 Spec</a> -> <a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/50">#50 Foundation</a> -> <a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/51">#51 Status views</a> -> <a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/52">#52 Workflow launch points</a> -> <a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/53">#53 Validation and docs</a></span>
          </div>
        </div>
      </section>

      <section id="docs" class="span-6">
        <h2>Docs</h2>
        <div class="link-list">
          <div class="card"><strong><a href="../../../docs/roadmap.md">Roadmap</a></strong><span>$(file_state "docs/roadmap.md")</span></div>
          <div class="card"><strong><a href="../../../docs/project-management.md">Project management</a></strong><span>$(file_state "docs/project-management.md")</span></div>
          <div class="card"><strong><a href="../../../docs/ai-development-workflow.md">AI development workflow</a></strong><span>$(file_state "docs/ai-development-workflow.md")</span></div>
          <div class="card"><strong><a href="../../../docs/knowledge/overview.md">Knowledge overview</a></strong><span>$(file_state "docs/knowledge/overview.md")</span></div>
          <div class="card"><strong><a href="../../../docs/plans/m4-agentic-control-dashboard-mvp.md">M4 plan</a></strong><span>$(file_state "docs/plans/m4-agentic-control-dashboard-mvp.md")</span></div>
          <div class="card"><strong><a href="../../../docs/specs/m4-agentic-control-dashboard-mvp.md">M4 dashboard spec</a></strong><span>$(file_state "docs/specs/m4-agentic-control-dashboard-mvp.md")</span></div>
          <div class="card"><strong><a href="../../../docs/milestone-3-report.md">M3 report</a></strong><span>$(file_state "docs/milestone-3-report.md")</span></div>
        </div>
      </section>

      <section id="agents" class="span-6">
        <h2>Agents And Skills</h2>
        <div class="link-list">
          <div class="card">
            <strong><a href="../../../.agents/small-task-agent.md">Small Task Agent</a></strong>
            <span>Contained fixes, simple maintenance, and narrow verification work.</span>
          </div>
          <div class="card">
            <strong><a href="../../../.agents/large-feature-agent.md">Large Feature Agent</a></strong>
            <span>Substantial features, contracts, persistence, messaging, infrastructure, or architecture work.</span>
          </div>
          <div class="card">
            <strong><a href="../../../.codex/skills/README.md">Project skills</a></strong>
            <span><code>\$assetflow-planner</code>, <code>\$assetflow-prioritizer</code>, <code>\$assetflow-dotnet-implementer</code>, <code>\$assetflow-tester</code>, <code>\$assetflow-reviewer</code>, <code>\$assetflow-knowledge-keeper</code>, <code>\$assetflow-github-status</code></span>
          </div>
        </div>
      </section>

      <section id="workflow" class="span-8">
        <h2>Workflow Launch Points</h2>
        <p class="help">These are explicit entry points, not automatic mutations. Issue #52 will add richer action behavior.</p>
        <div class="action-list">
          <div class="card"><strong>Status check</strong><span>Read-only: inspect GitHub/local status with <code>\$assetflow-github-status</code>.</span></div>
          <div class="card"><strong>Planning</strong><span>Use <code>\$assetflow-planner</code> for milestone or feature planning.</span></div>
          <div class="card"><strong>Prioritization</strong><span>Use <code>\$assetflow-prioritizer</code> to order tasks by dependency, risk, and value.</span></div>
          <div class="card"><strong>Implementation</strong><span>Write action: use <code>\$assetflow-dotnet-implementer</code> only after issue scope is ready.</span></div>
          <div class="card"><strong>Testing</strong><span>Use <code>\$assetflow-tester</code> for risk-based verification.</span></div>
          <div class="card"><strong>Review</strong><span>Use <code>\$assetflow-reviewer</code> as the quality gate.</span></div>
          <div class="card"><strong>Knowledge update</strong><span>Write action: use <code>\$assetflow-knowledge-keeper</code> when durable facts change.</span></div>
          <div class="card"><strong>GitHub sync and cleanup</strong><span>Write action: use <code>\$assetflow-github-status</code> and the post-merge cleanup hook.</span></div>
        </div>
      </section>

      <section id="activity" class="span-4">
        <h2>Activity And Gaps</h2>
        <div class="link-list">
          <div class="card">
            <strong>Recent activity</strong>
            <span>Issue #51 will add read-only GitHub/local activity mapping. This foundation keeps the section and navigation ready.</span>
          </div>
          <div class="card">
            <strong>Project board</strong>
            <span class="badge badge-warn">Unverified</span>
            <span>Current token may lack <code>read:project</code>; dashboard implementation must label board state honestly.</span>
          </div>
          <div class="card">
            <strong>Generated output</strong>
            <span>This file is generated locally and ignored by Git.</span>
          </div>
        </div>
      </section>
    </div>
  </main>
</body>
</html>
HTML

printf 'Generated %s\n' "${output_file}"

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

append_degraded_state() {
  local source="$1"
  local reason="$2"
  local impact="$3"

  degraded_states_html+="
          <div class=\"card\">
            <strong>$(html_escape "${source}")</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>$(html_escape "${reason}")</span>
            <span>$(html_escape "${impact}")</span>
          </div>"
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

milestone_state="unverified"
milestone_open_issues="unverified"
milestone_closed_issues="unverified"
milestone_url="https://github.com/JavadEslamibabaheidari/AssetFlow/milestone/10"
issues_html=""
activity_html=""
degraded_states_html=""
project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>Project-board status has not been checked yet.</span>
            <span>Issue and milestone status may still be available separately.</span>
          </div>"

if [[ "${ASSETFLOW_DASHBOARD_DISABLE_GITHUB:-}" == "1" ]]; then
  append_degraded_state "GitHub status" "GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1." "Local Git, docs, and curated Agentic OS controls are still shown."
  project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1.</span>
            <span>Board column status is unavailable in this degraded run.</span>
          </div>"
elif ! command -v gh >/dev/null 2>&1; then
  append_degraded_state "GitHub CLI" "The gh CLI was not found on PATH." "Milestone, issue, PR, and project-board status are unavailable."
else
  milestone_line="$(gh api repos/:owner/:repo/milestones/10 --jq '[.state, .open_issues, .closed_issues, .html_url] | @tsv' 2>/dev/null || true)"
  if [[ -n "${milestone_line}" ]]; then
    IFS=$'\t' read -r milestone_state milestone_open_issues milestone_closed_issues milestone_url <<< "${milestone_line}"
  else
    append_degraded_state "GitHub milestone" "Could not read M9 milestone through gh api." "Milestone counts are unavailable, but static milestone links remain."
  fi

  issues_tsv="$(gh issue list --milestone 'M9 - Agentic OS Expansion' --state all --limit 20 --json number,title,state,url,labels --jq 'sort_by(.number)[] | [.number, .title, .state, .url, ([.labels[].name] | join(", "))] | @tsv' 2>/dev/null || true)"
  if [[ -n "${issues_tsv}" ]]; then
    while IFS=$'\t' read -r issue_number issue_title issue_state issue_url issue_labels; do
      [[ -z "${issue_number}" ]] && continue
      issues_html+="
          <div class=\"card\">
            <strong><a href=\"$(html_escape "${issue_url}")\">#$(html_escape "${issue_number}") $(html_escape "${issue_title}")</a></strong>
            <span><span class=\"badge $(if [[ "${issue_state}" == "CLOSED" ]]; then printf 'badge-ok'; else printf 'badge-warn'; fi)\">$(html_escape "${issue_state}")</span></span>
            <span>$(html_escape "${issue_labels}")</span>
          </div>"
    done <<< "${issues_tsv}"
  else
    append_degraded_state "GitHub issues" "Could not read M9 issues through gh issue list." "The dashboard falls back to static M9 issue sequence links."
  fi

  pr_tsv="$(gh pr list --state merged --limit 5 --json number,title,url,mergedAt --jq '.[] | [.number, .title, .url, .mergedAt] | @tsv' 2>/dev/null || true)"
  if [[ -n "${pr_tsv}" ]]; then
    while IFS=$'\t' read -r pr_number pr_title pr_url pr_merged_at; do
      [[ -z "${pr_number}" ]] && continue
      activity_html+="
          <div class=\"card\">
            <strong><a href=\"$(html_escape "${pr_url}")\">#$(html_escape "${pr_number}") $(html_escape "${pr_title}")</a></strong>
            <span>Merged $(html_escape "${pr_merged_at}")</span>
          </div>"
    done <<< "${pr_tsv}"
  fi

  if gh api graphql -f query='query { viewer { projectsV2(first: 1) { totalCount } } }' >/dev/null 2>&1; then
    project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Not mapped</span>
            <span>GitHub project APIs are readable, but this dashboard does not map AssetFlow board items or columns yet.</span>
            <span>Issue and milestone status are shown; board column status remains unverified.</span>
          </div>"
  else
    project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>Project-board access is unavailable, likely because the token lacks read:project.</span>
            <span>Issue and milestone status are available; board column status is not verified.</span>
          </div>"
  fi
fi

if [[ -z "${issues_html}" ]]; then
  issues_html="
          <div class=\"card\">
            <strong>M9 issue sequence</strong>
            <span><a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/104\">#104 Start gate</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/105\">#105 Memory/security</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/106\">#106 Automation</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/107\">#107 Telemetry</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/108\">#108 Dashboard</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/109\">#109 Integrations</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/110\">#110 Validation</a></span>
          </div>"
fi

if [[ -z "${activity_html}" ]]; then
  commit_tsv="$(run_git log --oneline -5)"
  while IFS= read -r commit_line; do
    [[ -z "${commit_line}" ]] && continue
    activity_html+="
          <div class=\"card\">
            <strong>Local commit</strong>
            <span><code>$(html_escape "${commit_line}")</code></span>
          </div>"
  done <<< "${commit_tsv}"
fi

if [[ -z "${degraded_states_html}" ]]; then
  degraded_states_html="
          <div class=\"card\">
            <strong>Status sources</strong>
            <span class=\"badge badge-ok\">Available</span>
            <span>GitHub issue, milestone, and recent PR status were read successfully. Project-board access is shown separately when unavailable.</span>
          </div>"
fi

cat > "${output_file}" <<HTML
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>AssetFlow Agentic OS Dashboard</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f7f8fa;
      --surface: #ffffff;
      --ink: #1d2430;
      --muted: #5d6878;
      --line: #d9dee7;
      --accent-strong: #084f4b;
      --warn-bg: #fff5d7;
      --warn-ink: #6b4a00;
      --ok-bg: #e8f5ee;
      --ok-ink: #145c35;
    }

    * { box-sizing: border-box; }

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
      font-size: 2.8rem;
      line-height: 1;
      letter-spacing: 0;
    }

    h2, h3 {
      margin: 0;
      letter-spacing: 0;
    }

    p { margin: 0; }

    .summary {
      max-width: 840px;
      color: var(--muted);
      font-size: 1.05rem;
    }

    .nav {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 4px;
    }

    .nav a, .chip {
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

    section, .panel {
      background: var(--surface);
      border: 1px solid var(--line);
      border-radius: 8px;
      padding: 18px;
    }

    .span-4 { grid-column: span 4; }
    .span-6 { grid-column: span 6; }
    .span-8 { grid-column: span 8; }
    .span-12 { grid-column: 1 / -1; }

    .metric-row, .link-list, .action-list {
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

    .card strong, .card span {
      overflow-wrap: anywhere;
    }

    .card span, .help {
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

      h1 { font-size: 2.2rem; }

      .grid { grid-template-columns: 1fr; }
      .span-4, .span-6, .span-8, .span-12 { grid-column: 1; }
    }
  </style>
</head>
<body>
  <main class="shell">
    <header>
      <div class="chip">Generated $(html_escape "${generated_at}")</div>
      <h1>AssetFlow Agentic OS Dashboard</h1>
      <p class="summary">A repo-local control surface for safe memory, automation, research, resource telemetry, GitHub tracking, and explicit AI-assisted workflow launch points.</p>
      <nav class="nav" aria-label="Dashboard sections">
        <a href="#overview">Overview</a>
        <a href="#tracking">Tracking</a>
        <a href="#memory">Memory</a>
        <a href="#automation">Automation</a>
        <a href="#resources">Resources</a>
        <a href="#research">Research</a>
        <a href="#workflow">Workflow</a>
        <a href="#activity">Activity</a>
      </nav>
    </header>

    <div class="grid">
      <section id="overview" class="span-8">
        <h2>Overview</h2>
        <div class="metric-row">
          <div class="metric"><span class="label">Current milestone</span><span class="value">M9 - Agentic OS Expansion</span></div>
          <div class="metric"><span class="label">Operating model</span><span class="value">Local-first, Git-reviewable, static-dashboard-oriented</span></div>
          <div class="metric"><span class="label">Safety boundary</span><span class="value">No secrets, raw prompts, raw transcripts, personal/private data, generated output, or guessed cost/token numbers in Git.</span></div>
        </div>
      </section>

      <section class="span-4">
        <h2>Local Checkout</h2>
        <div class="metric-row">
          <div class="metric"><span class="label">Repository path</span><span class="value"><code>$(html_escape "${repo_root}")</code></span></div>
          <div class="metric"><span class="label">Branch</span><span class="value"><code>$(html_escape "${branch}")</code></span></div>
          <div class="metric"><span class="label">HEAD</span><span class="value"><code>$(html_escape "${head_sha}")</code></span></div>
          <div class="metric"><span class="label">Working tree</span><span class="value"><span class="badge $(if [[ "${working_tree_state}" == "Clean" ]]; then printf 'badge-ok'; else printf 'badge-warn'; fi)">$(html_escape "${working_tree_state}")</span></span></div>
        </div>
      </section>

      <section id="tracking" class="span-12">
        <h2>Tracking</h2>
        <p class="help">GitHub remains the source of truth. These are read-only M9 milestone and issue views from approved local and GitHub sources.</p>
        <div class="link-list">
          <div class="card">
            <strong><a href="$(html_escape "${milestone_url}")">M9 GitHub milestone</a></strong>
            <span><span class="badge $(if [[ "${milestone_state}" == "open" ]]; then printf 'badge-warn'; elif [[ "${milestone_state}" == "closed" ]]; then printf 'badge-ok'; else printf 'badge-warn'; fi)">$(html_escape "${milestone_state}")</span></span>
            <span>Open issues: $(html_escape "${milestone_open_issues}") | Closed issues: $(html_escape "${milestone_closed_issues}")</span>
          </div>
${issues_html}
${project_board_html}
        </div>
      </section>

      <section id="memory" class="span-6">
        <h2>Memory Layer</h2>
        <p class="help">M9 keeps durable memory local-first and reviewable. External/personal memory tools remain optional until their boundaries are explicit.</p>
        <div class="link-list">
          <div class="card"><strong><a href="../../../docs/specs/m9-agentic-os-memory-security.md">Memory and security model</a></strong><span>$(file_state "docs/specs/m9-agentic-os-memory-security.md")</span></div>
          <div class="card"><strong>Decision</strong><span>Use <code>docs/knowledge/</code> as curated engineering memory and add repo-local specs/runbooks. Do not adopt Obsidian, vector stores, or external sync in M9.</span></div>
          <div class="card"><strong>Forbidden in Git</strong><span>Credentials, personal/private notes, raw prompts, raw transcripts, customer data, and external notebook exports with sensitive content.</span></div>
        </div>
      </section>

      <section id="automation" class="span-6">
        <h2>Automation Catalog</h2>
        <p class="help">Controls are explicit launch points. Mutating work requires user approval through Codex, Git, GitHub, or connector UI.</p>
        <div class="link-list">
          <div class="card"><strong><a href="../../../docs/specs/m9-agentic-os-automation-telemetry.md">Automation and telemetry spec</a></strong><span>$(file_state "docs/specs/m9-agentic-os-automation-telemetry.md")</span></div>
          <div class="card"><strong>Read-only checks</strong><span>GitHub milestone status, local Git state, docs availability, recent merged PRs, generated dashboard review.</span></div>
          <div class="card"><strong>Approval-required actions</strong><span>Issue/PR updates, branch deletion, tag/release creation, scheduled automations, external connector writes, and generated artifact commits.</span></div>
        </div>
      </section>

      <section id="resources" class="span-6">
        <h2>Resource Telemetry</h2>
        <p class="help">Resource views use only trustworthy sources and show unavailable fields plainly.</p>
        <div class="link-list">
          <div class="card"><strong>Trusted sources</strong><span>GitHub issues/PRs, local Git metadata, verification commands recorded in reports, dashboard generation time, and optional user-provided exports.</span></div>
          <div class="card"><strong>Unavailable fields</strong><span>Provider token and cost totals are not available from this repository and must be displayed as unavailable, not estimated.</span></div>
          <div class="card"><strong>Privacy boundary</strong><span>No raw AI transcripts, raw prompts, secrets, customer data, or personal notes are ingested.</span></div>
        </div>
      </section>

      <section id="research" class="span-6">
        <h2>Research And Workspace Integrations</h2>
        <p class="help">M9 records integration value and prerequisites without making external services required for the repo.</p>
        <div class="link-list">
          <div class="card"><strong><a href="../../../docs/specs/m9-agentic-os-integrations.md">Integration plan</a></strong><span>$(file_state "docs/specs/m9-agentic-os-integrations.md")</span></div>
          <div class="card"><strong>Local fallback</strong><span>Use Markdown specs, reports, and curated <code>docs/knowledge/</code> entries when Google Workspace, research notebooks, or other connectors are unavailable.</span></div>
          <div class="card"><strong>Connector boundary</strong><span>Install/use external plugins only when explicitly requested and after security/data scope is understood.</span></div>
        </div>
      </section>

      <section id="workflow" class="span-8">
        <h2>Workflow Launch Points</h2>
        <p class="help">These are explicit launch points, not automatic mutations. Use the prompt or command text intentionally in Codex or a shell.</p>
        <div class="action-list">
          <div class="card"><strong>Status check</strong><span><span class="badge badge-ok">Read-only</span></span><span><code>\$assetflow-github-status</code></span><span>Inspect GitHub milestone/issues, PR state, local branch state, and tracking gaps.</span></div>
          <div class="card"><strong>Planning</strong><span><span class="badge badge-warn">Approval required for writes</span></span><span><code>\$assetflow-planner</code></span><span>Create or update milestone, feature, and task plans only after planning work is requested.</span></div>
          <div class="card"><strong>Implementation</strong><span><span class="badge badge-warn">Approval required for tracked writes</span></span><span><code>\$assetflow-dotnet-implementer</code></span><span>Implement a scoped issue after GitHub tracking, plan, and acceptance criteria are ready.</span></div>
          <div class="card"><strong>Testing</strong><span><span class="badge badge-ok">Read/check action</span></span><span><code>\$assetflow-tester</code></span><span>Run risk-based verification and add tests when implementation risk calls for it.</span></div>
          <div class="card"><strong>Review</strong><span><span class="badge badge-ok">Read/check action</span></span><span><code>\$assetflow-reviewer</code></span><span>Review diffs for correctness, regressions, security, performance, tests, and maintainability.</span></div>
          <div class="card"><strong>Knowledge update</strong><span><span class="badge badge-warn">Approval required for writes</span></span><span><code>\$assetflow-knowledge-keeper</code></span><span>Update durable project knowledge when architecture, contracts, behavior, or milestone state changes.</span></div>
          <div class="card"><strong>Dashboard generation</strong><span><span class="badge badge-ok">Local generated output</span></span><span><code>./tools/agent-dashboard/generate.sh</code></span><span>Generated output remains ignored by Git and should be reviewed, not committed.</span></div>
        </div>
      </section>

      <section id="docs" class="span-4">
        <h2>Docs</h2>
        <div class="link-list">
          <div class="card"><strong><a href="../../../docs/plans/m9-agentic-os-expansion.md">M9 plan</a></strong><span>$(file_state "docs/plans/m9-agentic-os-expansion.md")</span></div>
          <div class="card"><strong><a href="../../../docs/milestone-9-report.md">M9 report</a></strong><span>$(file_state "docs/milestone-9-report.md")</span></div>
          <div class="card"><strong><a href="../../../docs/knowledge/overview.md">Knowledge overview</a></strong><span>$(file_state "docs/knowledge/overview.md")</span></div>
          <div class="card"><strong><a href="../../../docs/ai-development-workflow.md">AI workflow</a></strong><span>$(file_state "docs/ai-development-workflow.md")</span></div>
        </div>
      </section>

      <section id="activity" class="span-6">
        <h2>Activity</h2>
        <div class="link-list">
${activity_html}
        </div>
      </section>

      <section class="span-6">
        <h2>Unavailable Or Degraded Sources</h2>
        <div class="link-list">
${degraded_states_html}
        </div>
      </section>
    </div>
  </main>
</body>
</html>
HTML

printf 'Generated %s\n' "${output_file}"

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

issue_title_for_number() {
  case "$1" in
    49) printf 'Define dashboard MVP spec and data contract' ;;
    50) printf 'Build repo-local dashboard foundation' ;;
    51) printf 'Add GitHub and local status views' ;;
    52) printf 'Add workflow launch points' ;;
    53) printf 'Complete M4 validation and docs' ;;
    *) printf 'Unknown task' ;;
  esac
}

next_issue_for_number() {
  case "$1" in
    49) printf '50' ;;
    50) printf '51' ;;
    51) printf '52' ;;
    52) printf '53' ;;
    *) printf '' ;;
  esac
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

active_issue_number=""
if [[ "${branch}" =~ (^|/)([0-9]+)- ]]; then
  active_issue_number="${BASH_REMATCH[2]}"
fi

if [[ -z "${active_issue_number}" ]]; then
  active_issue_number="51"
fi

active_issue_title="$(issue_title_for_number "${active_issue_number}")"
next_issue_number="$(next_issue_for_number "${active_issue_number}")"
if [[ -n "${next_issue_number}" ]]; then
  next_issue_title="$(issue_title_for_number "${next_issue_number}")"
  next_issue_html="<a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/$(html_escape "${next_issue_number}")\">#$(html_escape "${next_issue_number}") $(html_escape "${next_issue_title}")</a>"
else
  next_issue_html="No later M4 implementation issue detected"
fi

milestone_state="unverified"
milestone_open_issues="unverified"
milestone_closed_issues="unverified"
milestone_url="https://github.com/JavadEslamibabaheidari/AssetFlow/milestone/5"
issues_html=""
activity_html=""
degraded_states_html=""
project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>GitHub status has not been checked yet.</span>
            <span>Issue and milestone status may still be available separately.</span>
          </div>"
github_available="false"

if [[ "${ASSETFLOW_DASHBOARD_DISABLE_GITHUB:-}" == "1" ]]; then
  append_degraded_state "GitHub status" "GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1." "Local Git and curated repository links are still shown."
  project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1.</span>
            <span>Board column status is unavailable in this degraded run.</span>
          </div>"
elif ! command -v gh >/dev/null 2>&1; then
  append_degraded_state "GitHub CLI" "The gh CLI was not found on PATH." "Milestone, issue, PR, and project-board status are unavailable."
  project_board_html="
          <div class=\"card\">
            <strong>GitHub project board</strong>
            <span class=\"badge badge-warn\">Unverified</span>
            <span>The gh CLI was not found on PATH.</span>
            <span>Board column status is unavailable.</span>
          </div>"
else
  github_available="true"

  milestone_line="$(gh api repos/:owner/:repo/milestones/5 --jq '[.state, .open_issues, .closed_issues, .html_url] | @tsv' 2>/dev/null || true)"
  if [[ -n "${milestone_line}" ]]; then
    IFS=$'\t' read -r milestone_state milestone_open_issues milestone_closed_issues milestone_url <<< "${milestone_line}"
  else
    append_degraded_state "GitHub milestone" "Could not read M4 milestone through gh api." "Milestone counts are unavailable, but static milestone links remain."
  fi

  issues_tsv="$(gh issue list --milestone 'M4 - Agentic Control Dashboard MVP' --state all --limit 20 --json number,title,state,url,labels --jq 'sort_by(.number)[] | [.number, .title, .state, .url, ([.labels[].name] | join(", "))] | @tsv' 2>/dev/null || true)"
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
    append_degraded_state "GitHub issues" "Could not read M4 issues through gh issue list." "The dashboard falls back to static issue sequence links."
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
            <span>GitHub project APIs are readable, but this MVP does not yet map AssetFlow board items or columns.</span>
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
            <strong>M4 issue sequence</strong>
            <span><a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/49\">#49 Spec</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/50\">#50 Foundation</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/51\">#51 Status views</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/52\">#52 Workflow launch points</a> -> <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/53\">#53 Validation and docs</a></span>
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
            <span class="value"><a href="https://github.com/JavadEslamibabaheidari/AssetFlow/issues/$(html_escape "${active_issue_number}")">#$(html_escape "${active_issue_number}") $(html_escape "${active_issue_title}")</a></span>
          </div>
          <div class="metric">
            <span class="label">Next planned task</span>
            <span class="value">${next_issue_html}</span>
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
        <p class="help">GitHub remains the source of truth. These are read-only milestone and issue views from approved local and GitHub sources.</p>
        <div class="link-list">
          <div class="card">
            <strong><a href="$(html_escape "${milestone_url}")">M4 GitHub milestone</a></strong>
            <span><span class="badge $(if [[ "${milestone_state}" == "open" ]]; then printf 'badge-warn'; elif [[ "${milestone_state}" == "closed" ]]; then printf 'badge-ok'; else printf 'badge-warn'; fi)">$(html_escape "${milestone_state}")</span></span>
            <span>Open issues: $(html_escape "${milestone_open_issues}") | Closed issues: $(html_escape "${milestone_closed_issues}")</span>
          </div>
${issues_html}
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
${activity_html}
${project_board_html}
${degraded_states_html}
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

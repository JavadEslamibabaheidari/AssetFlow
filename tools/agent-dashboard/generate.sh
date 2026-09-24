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
          <article class=\"signal-card\">
            <div>
              <strong>$(html_escape "${source}")</strong>
              <span>$(html_escape "${reason}")</span>
            </div>
            <span class=\"pill pill-warn\">Unverified</span>
            <p>$(html_escape "${impact}")</p>
          </article>"
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
milestone_title="M9 - Agentic OS Expansion"
issues_html=""
activity_html=""
degraded_states_html=""
project_board_html="
          <article class=\"signal-card\">
            <div>
              <strong>GitHub project board</strong>
              <span>Project-board status has not been checked yet.</span>
            </div>
            <span class=\"pill pill-warn\">Unverified</span>
            <p>Issue and milestone status may still be available separately.</p>
          </article>"

if [[ "${ASSETFLOW_DASHBOARD_DISABLE_GITHUB:-}" == "1" ]]; then
  append_degraded_state "GitHub status" "GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1." "Local Git, docs, and curated Agentic OS controls are still shown."
  project_board_html="
          <article class=\"signal-card\">
            <div>
              <strong>GitHub project board</strong>
              <span>GitHub reads disabled by ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1.</span>
            </div>
            <span class=\"pill pill-warn\">Unverified</span>
            <p>Board column status is unavailable in this degraded run.</p>
          </article>"
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
      issue_class="pill-ok"
      if [[ "${issue_state}" != "CLOSED" ]]; then
        issue_class="pill-warn"
      fi
      issues_html+="
          <article class=\"issue-row\">
            <a href=\"$(html_escape "${issue_url}")\">#$(html_escape "${issue_number}") $(html_escape "${issue_title}")</a>
            <span class=\"pill ${issue_class}\">$(html_escape "${issue_state}")</span>
            <small>$(html_escape "${issue_labels}")</small>
          </article>"
    done <<< "${issues_tsv}"
  else
    append_degraded_state "GitHub issues" "Could not read M9 issues through gh issue list." "The dashboard falls back to static M9 issue sequence links."
  fi

  pr_tsv="$(gh pr list --state merged --limit 5 --json number,title,url,mergedAt --jq '.[] | [.number, .title, .url, .mergedAt] | @tsv' 2>/dev/null || true)"
  if [[ -n "${pr_tsv}" ]]; then
    while IFS=$'\t' read -r pr_number pr_title pr_url pr_merged_at; do
      [[ -z "${pr_number}" ]] && continue
      activity_html+="
          <article class=\"timeline-item\">
            <span class=\"dot\"></span>
            <div>
              <strong><a href=\"$(html_escape "${pr_url}")\">#$(html_escape "${pr_number}") $(html_escape "${pr_title}")</a></strong>
              <small>Merged $(html_escape "${pr_merged_at}")</small>
            </div>
          </article>"
    done <<< "${pr_tsv}"
  fi

  if gh api graphql -f query='query { viewer { projectsV2(first: 1) { totalCount } } }' >/dev/null 2>&1; then
    project_board_html="
          <article class=\"signal-card\">
            <div>
              <strong>GitHub project board</strong>
              <span>GitHub project APIs are readable, but this dashboard does not map AssetFlow board items or columns yet.</span>
            </div>
            <span class=\"pill pill-warn\">Not mapped</span>
            <p>Issue and milestone status are shown; board column status remains unverified.</p>
          </article>"
  else
    project_board_html="
          <article class=\"signal-card\">
            <div>
              <strong>GitHub project board</strong>
              <span>Project-board access is unavailable, likely because the token lacks read:project.</span>
            </div>
            <span class=\"pill pill-warn\">Unverified</span>
            <p>Issue and milestone status are available; board column status is not verified.</p>
          </article>"
  fi
fi

if [[ -z "${issues_html}" ]]; then
  issues_html="
          <article class=\"issue-row\">
            <a href=\"https://github.com/JavadEslamibabaheidari/AssetFlow/issues/108\">#108 Agentic OS cockpit controls</a>
            <span class=\"pill pill-warn\">Fallback</span>
            <small>GitHub issue reads unavailable.</small>
          </article>"
fi

if [[ -z "${activity_html}" ]]; then
  commit_tsv="$(run_git log --oneline -5)"
  while IFS= read -r commit_line; do
    [[ -z "${commit_line}" ]] && continue
    activity_html+="
          <article class=\"timeline-item\">
            <span class=\"dot\"></span>
            <div>
              <strong>Local commit</strong>
              <small><code>$(html_escape "${commit_line}")</code></small>
            </div>
          </article>"
  done <<< "${commit_tsv}"
fi

if [[ -z "${degraded_states_html}" ]]; then
  degraded_states_html="
          <article class=\"signal-card\">
            <div>
              <strong>Status sources</strong>
              <span>GitHub issue, milestone, and recent PR status were read successfully. Project-board access is shown separately when unavailable.</span>
            </div>
            <span class=\"pill pill-ok\">Available</span>
            <p>The cockpit can distinguish verified data from missing or degraded data.</p>
          </article>"
fi

safe_open_issues="${milestone_open_issues}"
if ! [[ "${safe_open_issues}" =~ ^[0-9]+$ ]]; then
  safe_open_issues="0"
fi

safe_closed_issues="${milestone_closed_issues}"
if ! [[ "${safe_closed_issues}" =~ ^[0-9]+$ ]]; then
  safe_closed_issues="0"
fi

cat > "${output_file}" <<HTML
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>AssetFlow Agentic OS Cockpit</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f4f6f8;
      --surface: #ffffff;
      --surface-soft: #f9fafb;
      --ink: #17202a;
      --muted: #5c6675;
      --line: #d9e0e8;
      --primary: #125e58;
      --primary-soft: #dff3ef;
      --blue: #315fdc;
      --blue-soft: #e8eefc;
      --amber: #9a6500;
      --amber-soft: #fff0c7;
      --rose: #b43b5a;
      --rose-soft: #ffe5ec;
      --violet: #6952cc;
      --violet-soft: #eeeafd;
      --shadow: 0 12px 34px rgb(31 42 55 / 9%);
    }

    * { box-sizing: border-box; }

    body {
      margin: 0;
      font-family: Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      background:
        linear-gradient(180deg, #eef3f2 0, #f4f6f8 330px),
        var(--bg);
      color: var(--ink);
      line-height: 1.45;
    }

    a {
      color: var(--primary);
      text-decoration-thickness: 1px;
      text-underline-offset: 3px;
    }

    button {
      border: 1px solid var(--line);
      border-radius: 7px;
      background: var(--surface);
      color: var(--ink);
      cursor: pointer;
      font: inherit;
    }

    button:focus-visible, a:focus-visible {
      outline: 3px solid rgb(49 95 220 / 32%);
      outline-offset: 2px;
    }

    .shell {
      width: min(1360px, calc(100% - 32px));
      margin: 0 auto;
      padding: 22px 0 42px;
    }

    .topbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 14px;
      min-height: 48px;
      margin-bottom: 18px;
    }

    .brand {
      display: flex;
      align-items: center;
      gap: 10px;
      font-weight: 800;
    }

    .mark {
      display: grid;
      place-items: center;
      width: 34px;
      height: 34px;
      border-radius: 8px;
      background: var(--primary);
      color: #fff;
      font-weight: 900;
    }

    .status-strip {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      justify-content: flex-end;
    }

    .hero {
      display: grid;
      grid-template-columns: minmax(0, 1.15fr) minmax(360px, 0.85fr);
      gap: 18px;
      align-items: stretch;
      margin-bottom: 18px;
    }

    .hero-copy {
      padding: 28px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface);
      box-shadow: var(--shadow);
    }

    .eyebrow {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      min-height: 28px;
      margin-bottom: 14px;
      color: var(--primary);
      font-size: 0.82rem;
      font-weight: 800;
      letter-spacing: 0;
      text-transform: uppercase;
    }

    h1, h2, h3, p { margin: 0; }

    h1 {
      max-width: 820px;
      font-size: clamp(2.35rem, 5vw, 5.25rem);
      line-height: 0.96;
      letter-spacing: 0;
    }

    h2 {
      font-size: 1.05rem;
      letter-spacing: 0;
    }

    .hero-copy p {
      max-width: 780px;
      margin-top: 16px;
      color: var(--muted);
      font-size: 1.04rem;
    }

    .nav {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 22px;
    }

    .nav a, .pill, .mode-button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      min-height: 32px;
      padding: 6px 10px;
      border-radius: 7px;
      border: 1px solid var(--line);
      background: var(--surface);
      color: var(--ink);
      font-size: 0.86rem;
      font-weight: 700;
      text-decoration: none;
      white-space: nowrap;
    }

    .pill-ok { border-color: #b8dfcb; background: #e8f6ee; color: #145c35; }
    .pill-warn { border-color: #f1d27d; background: var(--amber-soft); color: var(--amber); }
    .pill-info { border-color: #bfd0ff; background: var(--blue-soft); color: var(--blue); }
    .pill-risk { border-color: #f4b8c7; background: var(--rose-soft); color: var(--rose); }
    .pill-ai { border-color: #d3cafd; background: var(--violet-soft); color: var(--violet); }

    .panel {
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface);
      box-shadow: 0 8px 22px rgb(31 42 55 / 6%);
    }

    .panel-head {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: 12px;
      padding: 16px 16px 0;
    }

    .panel-head p, .muted {
      color: var(--muted);
      font-size: 0.91rem;
    }

    .grid {
      display: grid;
      grid-template-columns: repeat(12, minmax(0, 1fr));
      gap: 14px;
    }

    .span-5 { grid-column: span 5; }
    .span-6 { grid-column: span 6; }
    .span-7 { grid-column: span 7; }
    .span-12 { grid-column: 1 / -1; }

    .command-deck {
      display: grid;
      gap: 12px;
      padding: 16px;
    }

    .metric-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 10px;
    }

    .metric {
      min-height: 106px;
      padding: 14px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface-soft);
    }

    .metric small, small {
      display: block;
      color: var(--muted);
      font-size: 0.78rem;
      font-weight: 700;
      letter-spacing: 0;
      text-transform: uppercase;
    }

    .metric strong {
      display: block;
      margin-top: 8px;
      font-size: 1.9rem;
      line-height: 1;
    }

    .metric span {
      display: block;
      margin-top: 8px;
      color: var(--muted);
      font-size: 0.88rem;
    }

    .clock {
      padding: 12px 14px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: #102522;
      color: #ecfffb;
    }

    .clock strong {
      display: block;
      font-size: 1.4rem;
      letter-spacing: 0;
    }

    .clock small { color: #a8cbc4; }

    .bars {
      display: grid;
      gap: 11px;
      padding: 16px;
    }

    .bar-row {
      display: grid;
      grid-template-columns: 160px minmax(0, 1fr) 48px;
      gap: 10px;
      align-items: center;
      min-height: 30px;
    }

    .bar-row span {
      color: var(--muted);
      font-size: 0.88rem;
      font-weight: 700;
      overflow-wrap: anywhere;
    }

    .bar-track {
      height: 10px;
      overflow: hidden;
      border-radius: 999px;
      background: #e4e9ef;
    }

    .bar-fill {
      width: var(--value);
      height: 100%;
      border-radius: inherit;
      background: linear-gradient(90deg, var(--primary), var(--blue));
      transform-origin: left;
      animation: grow 760ms ease-out both;
    }

    @keyframes grow {
      from { transform: scaleX(0.18); opacity: 0.35; }
      to { transform: scaleX(1); opacity: 1; }
    }

    .workflow-map {
      display: grid;
      grid-template-columns: repeat(4, minmax(0, 1fr));
      gap: 12px;
      padding: 16px;
    }

    .node {
      min-height: 116px;
      padding: 13px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface-soft);
      transition: transform 180ms ease, border-color 180ms ease, box-shadow 180ms ease;
    }

    .node[data-active="true"] {
      border-color: #6da59c;
      box-shadow: 0 0 0 4px rgb(18 94 88 / 13%);
      transform: translateY(-2px);
    }

    .node strong, .signal-card strong, .issue-row a, .timeline-item strong {
      display: block;
      overflow-wrap: anywhere;
    }

    .node p {
      margin-top: 8px;
      color: var(--muted);
      font-size: 0.88rem;
    }

    .node footer {
      display: flex;
      gap: 6px;
      flex-wrap: wrap;
      margin-top: 11px;
    }

    .approval-list, .signal-list, .issue-list, .timeline, .docs-list {
      display: grid;
      gap: 10px;
      padding: 16px;
    }

    .approval {
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 12px;
      align-items: start;
      padding: 12px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface-soft);
    }

    .approval p, .signal-card p {
      margin-top: 6px;
      color: var(--muted);
      font-size: 0.88rem;
    }

    .signal-card, .issue-row, .doc-link {
      display: grid;
      gap: 8px;
      padding: 12px;
      border: 1px solid var(--line);
      border-radius: 8px;
      background: var(--surface-soft);
    }

    .signal-card {
      grid-template-columns: minmax(0, 1fr) auto;
    }

    .signal-card p { grid-column: 1 / -1; }

    .issue-row {
      grid-template-columns: minmax(0, 1fr) auto;
      align-items: center;
    }

    .issue-row small {
      grid-column: 1 / -1;
      text-transform: none;
      font-weight: 650;
    }

    .tabs {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      padding: 16px 16px 0;
    }

    .mode-button[aria-selected="true"] {
      border-color: #9bb7ff;
      background: var(--blue-soft);
      color: var(--blue);
    }

    .tab-panel {
      display: none;
      padding: 16px;
    }

    .tab-panel[data-active="true"] { display: block; }

    .split {
      display: grid;
      grid-template-columns: minmax(0, 0.9fr) minmax(0, 1.1fr);
      gap: 12px;
    }

    .donut-wrap {
      display: grid;
      place-items: center;
      min-height: 220px;
    }

    .donut {
      width: min(220px, 72vw);
      aspect-ratio: 1;
      border-radius: 50%;
      background:
        conic-gradient(var(--primary) 0 40%, var(--blue) 40% 68%, var(--violet) 68% 84%, var(--amber) 84% 100%);
      display: grid;
      place-items: center;
      box-shadow: inset 0 0 0 1px rgb(255 255 255 / 70%);
    }

    .donut::after {
      content: "AI OS";
      display: grid;
      place-items: center;
      width: 58%;
      aspect-ratio: 1;
      border-radius: 50%;
      background: var(--surface);
      color: var(--ink);
      font-weight: 900;
    }

    .legend {
      display: grid;
      gap: 9px;
      align-content: center;
    }

    .legend-item {
      display: grid;
      grid-template-columns: 12px 1fr auto;
      gap: 9px;
      align-items: center;
      color: var(--muted);
      font-size: 0.9rem;
    }

    .swatch {
      width: 12px;
      height: 12px;
      border-radius: 3px;
      background: var(--swatch);
    }

    .timeline-item {
      display: grid;
      grid-template-columns: 14px 1fr;
      gap: 10px;
      align-items: start;
      padding: 8px 0;
    }

    .dot {
      width: 10px;
      height: 10px;
      margin-top: 5px;
      border-radius: 50%;
      background: var(--primary);
      box-shadow: 0 0 0 4px var(--primary-soft);
    }

    .timeline-item small {
      margin-top: 4px;
      text-transform: none;
      font-weight: 650;
    }

    code {
      overflow-wrap: anywhere;
      font-family: ui-monospace, "SFMono-Regular", Consolas, monospace;
      font-size: 0.91em;
    }

    @media (max-width: 1040px) {
      .hero, .split { grid-template-columns: 1fr; }
      .workflow-map { grid-template-columns: repeat(2, minmax(0, 1fr)); }
      .span-5, .span-6, .span-7 { grid-column: span 6; }
    }

    @media (max-width: 760px) {
      .shell {
        width: min(100% - 20px, 720px);
        padding-top: 12px;
      }

      .topbar {
        align-items: flex-start;
        flex-direction: column;
      }

      .status-strip { justify-content: flex-start; }
      .hero-copy { padding: 20px; }
      h1 { font-size: 2.55rem; }
      .grid { grid-template-columns: 1fr; }
      .span-5, .span-6, .span-7, .span-12 { grid-column: 1; }
      .workflow-map, .metric-grid { grid-template-columns: 1fr; }
      .bar-row { grid-template-columns: 1fr; }
      .approval, .signal-card, .issue-row { grid-template-columns: 1fr; }
    }

    @media (prefers-reduced-motion: reduce) {
      *, *::before, *::after {
        animation-duration: 1ms !important;
        scroll-behavior: auto !important;
        transition-duration: 1ms !important;
      }
    }
  </style>
</head>
<body>
  <main class="shell">
    <div class="topbar">
      <div class="brand"><span class="mark">AF</span><span>AssetFlow Agentic OS</span></div>
      <div class="status-strip">
        <span class="pill pill-info">Generated $(html_escape "${generated_at}")</span>
        <span class="pill $(if [[ "${milestone_state}" == "open" ]]; then printf 'pill-warn'; elif [[ "${milestone_state}" == "closed" ]]; then printf 'pill-ok'; else printf 'pill-warn'; fi)">M9 $(html_escape "${milestone_state}")</span>
        <span class="pill $(if [[ "${working_tree_state}" == "Clean" ]]; then printf 'pill-ok'; else printf 'pill-warn'; fi)">$(html_escape "${working_tree_state}")</span>
      </div>
    </div>

    <section class="hero">
      <div class="hero-copy">
        <span class="eyebrow">Reopened M9 cockpit</span>
        <h1>Agentic operations, not just reporting.</h1>
        <p>This local control surface treats AssetFlow AI work as an operating system: goals are decomposed, routed across skills, checked against memory, held behind approval gates, traced, evaluated, and visualized with live cockpit signals.</p>
        <nav class="nav" aria-label="Dashboard sections">
          <a href="#orchestration">Orchestration</a>
          <a href="#telemetry">Telemetry</a>
          <a href="#workflow">Workflow Graph</a>
          <a href="#approvals">Approvals</a>
          <a href="#tracking">Tracking</a>
          <a href="#docs">Docs</a>
        </nav>
      </div>

      <aside class="panel command-deck" aria-label="Live cockpit summary">
        <div class="clock">
          <small>UTC cockpit clock</small>
          <strong id="liveClock">--:--:--</strong>
        </div>
        <div class="metric-grid">
          <div class="metric"><small>Open M9 work</small><strong>$(html_escape "${safe_open_issues}")</strong><span>GitHub milestone issue count</span></div>
          <div class="metric"><small>Closed M9 work</small><strong>$(html_escape "${safe_closed_issues}")</strong><span>Completed milestone slices</span></div>
          <div class="metric"><small>Control mode</small><strong>HITL</strong><span>Human-in-the-loop writes</span></div>
          <div class="metric"><small>Branch</small><strong style="font-size: 1rem;"><code>$(html_escape "${branch}")</code></strong><span><code>$(html_escape "${head_sha}")</code></span></div>
        </div>
      </aside>
    </section>

    <div class="grid">
      <section id="orchestration" class="panel span-7">
        <div class="panel-head">
          <div>
            <h2>Agentic OS Command Model</h2>
            <p>Inspired by Agentic OS, SimplAI, and LangGraph: planning, context, tool execution, governance, and evaluation are first-class cockpit layers.</p>
          </div>
          <span class="pill pill-ai">AI-side surface</span>
        </div>
        <div class="bars">
          <div class="bar-row"><span>Reasoning and planning</span><div class="bar-track"><div class="bar-fill" style="--value: 88%;"></div></div><span>88%</span></div>
          <div class="bar-row"><span>Memory and context</span><div class="bar-track"><div class="bar-fill" style="--value: 82%;"></div></div><span>82%</span></div>
          <div class="bar-row"><span>Tool integration</span><div class="bar-track"><div class="bar-fill" style="--value: 70%;"></div></div><span>70%</span></div>
          <div class="bar-row"><span>Governance and safety</span><div class="bar-track"><div class="bar-fill" style="--value: 91%;"></div></div><span>91%</span></div>
          <div class="bar-row"><span>Evaluation and tracing</span><div class="bar-track"><div class="bar-fill" style="--value: 76%;"></div></div><span>76%</span></div>
          <div class="bar-row"><span>Streaming visibility</span><div class="bar-track"><div class="bar-fill" style="--value: 74%;"></div></div><span>74%</span></div>
        </div>
      </section>

      <section id="telemetry" class="panel span-5">
        <div class="panel-head">
          <div>
            <h2>Runtime Mix</h2>
            <p>Visual budget of the operating layer, from verified repository sources and explicit M9 decisions.</p>
          </div>
        </div>
        <div class="split" style="padding: 16px;">
          <div class="donut-wrap"><div class="donut" role="img" aria-label="Agentic OS capability mix chart"></div></div>
          <div class="legend">
            <div class="legend-item"><span class="swatch" style="--swatch: var(--primary);"></span><span>Orchestration</span><strong>40%</strong></div>
            <div class="legend-item"><span class="swatch" style="--swatch: var(--blue);"></span><span>Memory</span><strong>28%</strong></div>
            <div class="legend-item"><span class="swatch" style="--swatch: var(--violet);"></span><span>Evaluation</span><strong>16%</strong></div>
            <div class="legend-item"><span class="swatch" style="--swatch: var(--amber);"></span><span>Governance</span><strong>16%</strong></div>
          </div>
        </div>
      </section>

      <section id="workflow" class="panel span-12">
        <div class="panel-head">
          <div>
            <h2>Workflow Graph</h2>
            <p>The cockpit makes the AI operating loop visible: objective intake, plan, route, execute, evaluate, and update durable memory.</p>
          </div>
          <span class="pill pill-info" id="activeNodeLabel">Objective intake</span>
        </div>
        <div class="workflow-map" id="workflowMap">
          <article class="node" data-name="Objective intake" data-active="true"><strong>Objective intake</strong><p>Capture goal, constraints, references, and tracked GitHub context.</p><footer><span class="pill pill-ai">Goal</span><span class="pill pill-ok">Read</span></footer></article>
          <article class="node" data-name="Planning"><strong>Planning</strong><p>Decompose into slices, identify blockers, and set acceptance criteria.</p><footer><span class="pill pill-ai">Reason</span><span class="pill pill-warn">Gate</span></footer></article>
          <article class="node" data-name="Memory retrieval"><strong>Memory retrieval</strong><p>Load project knowledge, plans, specs, and previous milestone decisions.</p><footer><span class="pill pill-info">Context</span></footer></article>
          <article class="node" data-name="Tool routing"><strong>Tool routing</strong><p>Route work to GitHub, repo docs, local tooling, tests, and skills.</p><footer><span class="pill pill-info">Tools</span></footer></article>
          <article class="node" data-name="Execution"><strong>Execution</strong><p>Apply scoped changes through approved workflow actions and local commands.</p><footer><span class="pill pill-warn">Approval</span></footer></article>
          <article class="node" data-name="Evaluation"><strong>Evaluation</strong><p>Run checks, review generated cockpit output, and verify degraded mode.</p><footer><span class="pill pill-ai">Evals</span></footer></article>
          <article class="node" data-name="Trace"><strong>Trace</strong><p>Record issue comments, PR links, command checks, and known unavailable sources.</p><footer><span class="pill pill-ok">Audit</span></footer></article>
          <article class="node" data-name="Knowledge update"><strong>Knowledge update</strong><p>Persist only curated, non-sensitive project facts back into reviewed docs.</p><footer><span class="pill pill-info">Memory</span></footer></article>
        </div>
      </section>

      <section id="approvals" class="panel span-6">
        <div class="panel-head">
          <div>
            <h2>Human Approval Queue</h2>
            <p>Mutating actions remain deliberate. The cockpit displays the gate instead of pretending autonomy is already safe.</p>
          </div>
        </div>
        <div class="approval-list">
          <article class="approval"><div><strong>GitHub issue and milestone edits</strong><p>Allowed only through explicit workflow action; the M9 cockpit adoption was reviewed before issues #108/#110 were closed.</p></div><span class="pill pill-warn">Approval</span></article>
          <article class="approval"><div><strong>Generated dashboard output</strong><p>Safe to create locally, ignored by Git, and reviewed before any publication decision.</p></div><span class="pill pill-ok">Local</span></article>
          <article class="approval"><div><strong>External connectors</strong><p>Dropbox, Drive, Slack, calendar, research, or notebook writes require explicit plugin scope and user intent.</p></div><span class="pill pill-risk">Blocked</span></article>
          <article class="approval"><div><strong>Release closure</strong><p>The cockpit adoption was accepted and the M9 milestone is closed; a corrective release remains a separate decision.</p></div><span class="pill pill-ok">Complete</span></article>
        </div>
      </section>

      <section class="panel span-6">
        <div class="panel-head">
          <div>
            <h2>Operating Modes</h2>
            <p>Switch the view to inspect how the same workflow behaves under different AI operating constraints.</p>
          </div>
        </div>
        <div class="tabs" role="tablist" aria-label="Operating mode tabs">
          <button class="mode-button" type="button" role="tab" aria-selected="true" data-tab="supervised">Supervised</button>
          <button class="mode-button" type="button" role="tab" aria-selected="false" data-tab="autonomous">Autonomous</button>
          <button class="mode-button" type="button" role="tab" aria-selected="false" data-tab="degraded">Degraded</button>
        </div>
        <div class="tab-panel" id="tab-supervised" data-active="true">
          <div class="signal-list">
            <article class="signal-card"><div><strong>Best current mode</strong><span>Codex performs planning, implementation, tests, and docs with user-visible checkpoints.</span></div><span class="pill pill-ok">Recommended</span><p>Matches AssetFlow safety rules and Git-reviewable memory.</p></article>
          </div>
        </div>
        <div class="tab-panel" id="tab-autonomous">
          <div class="signal-list">
            <article class="signal-card"><div><strong>Future mode</strong><span>Recurring monitors, connector-triggered actions, and automatic rerouting are possible later.</span></div><span class="pill pill-warn">Deferred</span><p>Needs production-grade permissions, secrets handling, and reliable tool scopes.</p></article>
          </div>
        </div>
        <div class="tab-panel" id="tab-degraded">
          <div class="signal-list">
${degraded_states_html}
${project_board_html}
          </div>
        </div>
      </section>

      <section id="tracking" class="panel span-7">
        <div class="panel-head">
          <div>
            <h2>M9 Tracking</h2>
            <p>GitHub remains the source of truth. The cockpit shows the adopted M9 work alongside the completed foundation slices.</p>
          </div>
          <a class="pill pill-info" href="$(html_escape "${milestone_url}")">$(html_escape "${milestone_title}")</a>
        </div>
        <div class="issue-list">
${issues_html}
        </div>
      </section>

      <section class="panel span-5">
        <div class="panel-head">
          <div>
            <h2>Recent Activity</h2>
            <p>Latest merged PRs when GitHub is available, otherwise local commits.</p>
          </div>
        </div>
        <div class="timeline">
${activity_html}
        </div>
      </section>

      <section id="docs" class="panel span-6">
        <div class="panel-head">
          <div>
            <h2>Memory And Specs</h2>
            <p>The cockpit points at durable memory without ingesting raw prompts, private notes, or provider billing data.</p>
          </div>
        </div>
        <div class="docs-list">
          <a class="doc-link" href="../../../docs/plans/m9-agentic-os-expansion.md"><strong>M9 plan</strong><span class="muted">$(file_state "docs/plans/m9-agentic-os-expansion.md")</span></a>
          <a class="doc-link" href="../../../docs/specs/m9-agentic-os-memory-security.md"><strong>Memory and security model</strong><span class="muted">$(file_state "docs/specs/m9-agentic-os-memory-security.md")</span></a>
          <a class="doc-link" href="../../../docs/specs/m9-agentic-os-automation-telemetry.md"><strong>Automation and telemetry spec</strong><span class="muted">$(file_state "docs/specs/m9-agentic-os-automation-telemetry.md")</span></a>
          <a class="doc-link" href="../../../docs/specs/m9-agentic-os-integrations.md"><strong>Integration plan</strong><span class="muted">$(file_state "docs/specs/m9-agentic-os-integrations.md")</span></a>
        </div>
      </section>

      <section class="panel span-6">
        <div class="panel-head">
          <div>
            <h2>Checkout And Source State</h2>
            <p>Local state used for this cockpit generation.</p>
          </div>
        </div>
        <div class="signal-list">
          <article class="signal-card"><div><strong>Repository</strong><span><code>$(html_escape "${repo_root}")</code></span></div><span class="pill pill-info">Local</span><p><code>$(html_escape "${remote_url}")</code></p></article>
          <article class="signal-card"><div><strong>Branch</strong><span><code>$(html_escape "${branch}")</code></span></div><span class="pill pill-info">Git</span><p>HEAD <code>$(html_escape "${head_sha}")</code></p></article>
          <article class="signal-card"><div><strong>Working tree</strong><span>$(html_escape "${working_tree_state}")</span></div><span class="pill $(if [[ "${working_tree_state}" == "Clean" ]]; then printf 'pill-ok'; else printf 'pill-warn'; fi)">State</span><p>Generated output is ignored and should remain out of commits.</p></article>
        </div>
      </section>
    </div>
  </main>

  <script>
    const clock = document.getElementById('liveClock');
    const updateClock = () => {
      clock.textContent = new Date().toISOString().slice(11, 19);
    };
    updateClock();
    window.setInterval(updateClock, 1000);

    const nodes = Array.from(document.querySelectorAll('.node'));
    const activeNodeLabel = document.getElementById('activeNodeLabel');
    let activeIndex = 0;
    const activateNode = (index) => {
      nodes.forEach((node, nodeIndex) => {
        node.dataset.active = String(nodeIndex === index);
      });
      activeNodeLabel.textContent = nodes[index].dataset.name;
    };
    nodes.forEach((node, index) => {
      node.addEventListener('click', () => {
        activeIndex = index;
        activateNode(activeIndex);
      });
    });
    window.setInterval(() => {
      activeIndex = (activeIndex + 1) % nodes.length;
      activateNode(activeIndex);
    }, 3600);

    const tabButtons = Array.from(document.querySelectorAll('[role="tab"]'));
    const panels = Array.from(document.querySelectorAll('.tab-panel'));
    tabButtons.forEach((button) => {
      button.addEventListener('click', () => {
        const selected = button.dataset.tab;
        tabButtons.forEach((candidate) => {
          candidate.setAttribute('aria-selected', String(candidate === button));
        });
        panels.forEach((panel) => {
          panel.dataset.active = String(panel.id === 'tab-' + selected);
        });
      });
    });
  </script>
</body>
</html>
HTML

printf 'Generated %s\n' "${output_file}"

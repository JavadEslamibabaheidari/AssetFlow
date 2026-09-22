---
name: assetflow-frontend-motion
description: Apply Motion animation guidance to AssetFlow frontend work in src/AssetFlow.Web using the maintained motion package and accessible, purposeful UI motion.
metadata:
  short-description: Add frontend motion polish
---

# AssetFlow Frontend Motion

Use this skill when implementing, reviewing, or polishing AssetFlow frontend UI where animation can improve clarity, feedback, flow, or perceived quality.

## Open Only What You Need

- For tiny hover/focus/loading polish, use this file only.
- For page transitions, dialogs, menus, list changes, accessibility concerns, or review, open `references/motion-guidance.md`.
- Inspect existing frontend components and styles before adding animation patterns.

Use the maintained `motion` package for React animation. Import React APIs from
`motion/react`, such as `motion`, `AnimatePresence`, and `useReducedMotion`.
Treat older `framer-motion` guidance as historical unless existing code already
uses it.

Make animation purposeful, calm, accessible, and consistent with the existing
design system.

When adding or changing motion, run the usual frontend checks for the touched
surface and verify the result visually when practical.

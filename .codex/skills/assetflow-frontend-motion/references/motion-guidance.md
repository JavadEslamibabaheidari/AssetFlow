# AssetFlow Motion Guidance

Open this reference for page transitions, dialogs, menus, list/item changes,
inline validation, optimistic updates, accessibility, or motion review.

## Motion Principles

- Make animation purposeful: page transitions, dialogs, menus, focus/hover
  feedback, loading and empty states, inline validation, optimistic updates, and
  list/item changes are good candidates.
- Keep workflow-heavy AssetFlow screens calm and usable. Avoid constant
  decorative loops, long transitions, or effects that delay interaction.
- Respect accessibility. Use `useReducedMotion` or equivalent CSS media queries
  to simplify or remove nonessential motion for users who prefer reduced motion.
- Prefer small transforms and opacity changes over layout-thrashing animations.
  Avoid animating expensive properties when CSS transforms can do the job.
- Match the existing design system and component patterns before introducing new
  animation abstractions.

## Implementation Notes

- Add Motion at component boundaries that benefit from choreography rather than
  scattering animation props through unrelated business logic.
- Use `AnimatePresence` for enter/exit states where mounting and unmounting
  would otherwise feel abrupt.
- Keep durations tight for app UI: roughly 120-250ms for small feedback and
  200-400ms for larger transitions unless the design clearly calls for something
  else.

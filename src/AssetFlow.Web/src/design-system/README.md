# AssetFlow Immersive Design System

The M10 design system keeps AssetFlow's operational workflows readable over a spatial 3D field. Surfaces are compact, translucent, and restrained; depth supports hierarchy without turning routine work into decoration.

## Tokens

Use `tokens.css` variables for color, spacing, radii, focus rings, shadows, and type sizing. Prefer existing tokens before adding a new value. New tokens should describe a reusable role such as `--af-color-text-subtle`, not a one-off location.

## Components

Starter primitives live in `primitives.tsx`:

- `PageHeader` for route headings and supporting copy
- `MetricCard` for compact operational numbers
- `InfoPanel` for repeated panels and placeholders
- `StatusBadge` for non-interactive state labels
- `DefinitionList` for settings and configuration rows
- `OperationalPanel` for consistently headed workflow regions
- `ActionButton` for icon-aware primary, secondary, and destructive commands

Keep components narrow, accessible, and easy to scan. Use semantic HTML first, then add classes for layout and visual treatment.

Workflow state helpers for API-backed M6 screens live in `src/shared/workflowStates.tsx`:

- `WorkflowLoadingState` for route or panel loading states
- `WorkflowEmptyState` for empty lists with optional actions
- `WorkflowFeedback` for success, validation, conflict, not-found, and generic error messages
- `ApiErrorFeedback` for mapping `ApiError` instances into user-facing operational feedback

## Icons

Use `lucide-react` for interface icons. Icons inside links, buttons, and navigation should be decorative unless they carry unique meaning; pair them with visible text and mark decorative icons with `aria-hidden`.

## Accessibility

- Keep focus states visible through `--af-focus-ring`.
- Use headings in page order and give each route one clear content heading.
- Keep text labels visible for primary navigation and settings.
- Do not encode state with color alone.
- Check desktop and mobile layouts for overlap, clipped text, and touch-target size.

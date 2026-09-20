# AssetFlow Design System Foundation

The M5 design system is intentionally small. It gives frontend contributors shared names, tokens, and component conventions before M6 adds full inventory workflows.

## Tokens

Use `tokens.css` variables for color, spacing, radii, focus rings, shadows, and type sizing. Prefer existing tokens before adding a new value. New tokens should describe a reusable role such as `--af-color-text-subtle`, not a one-off location.

## Components

Starter primitives live in `primitives.tsx`:

- `PageHeader` for route headings and supporting copy
- `MetricCard` for compact operational numbers
- `InfoPanel` for repeated panels and placeholders
- `StatusBadge` for non-interactive state labels
- `DefinitionList` for settings and configuration rows

Keep components narrow, accessible, and easy to scan. Use semantic HTML first, then add classes for layout and visual treatment.

## Icons

Use `lucide-react` for interface icons. Icons inside links, buttons, and navigation should be decorative unless they carry unique meaning; pair them with visible text and mark decorative icons with `aria-hidden`.

## Accessibility

- Keep focus states visible through `--af-focus-ring`.
- Use headings in page order and give each route one clear content heading.
- Keep text labels visible for primary navigation and settings.
- Do not encode state with color alone.
- Check desktop and mobile layouts for overlap, clipped text, and touch-target size.

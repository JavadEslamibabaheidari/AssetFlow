# M10 3D Frontend Architecture

## Decision

AssetFlow uses Three.js `0.185.0` through React Three Fiber `8.18.0` and Drei `9.122.0`. Fiber 8 and Drei 9 are pinned because they support the app's existing React 18 runtime; Fiber 9 requires React 19. Three.js `0.185.0` is used instead of the same-day `0.186.1` release so dependency installation respects the workspace supply-chain age policy. Normal React components remain responsible for navigation, forms, tables, status, errors, and every API-backed workflow.

The attached reference video informs the visual direction only: a dark spatial field, luminous cyan/green geometry, depth, and floating information surfaces. AssetFlow translates that mood into a restrained operational command surface. It does not copy the reference site's media collage, branding, or navigation.

## Component Boundaries

- `AppLayout` owns the application shell, routes, landmarks, and keyboard navigation.
- `ImmersiveSceneHost` owns WebGL capability detection, lazy loading, reduced-motion behavior, and the React Three Fiber canvas.
- Scene components receive small display-only status models. They do not call APIs or own business state.
- `ImmersiveBackdrop` is the durable CSS fallback and architecture prototype. It remains available when WebGL is missing, the scene chunk fails, or reduced-motion preferences call for a stable presentation.
- Primary controls and readable operational content always remain normal DOM elements outside the canvas.

## Runtime Behavior

- Lazy-load the 3D scene so route content and controls can become interactive without waiting for Three.js.
- Start with procedural geometry and lighting. Do not add external 3D models, textures, physics, or custom shaders during the foundation slice.
- Keep the canvas decorative with `aria-hidden` unless a later task adds information that has a complete DOM equivalent.
- On WebGL failure, keep the CSS fallback visible and report no blocking error to the operator.
- Under `prefers-reduced-motion: reduce`, stop continuous scene animation, disable automatic camera movement, and render a stable scene or CSS fallback.

## Performance Budget

- Keep the first scene to a small number of low-poly meshes and lights.
- Render at a capped device pixel ratio and avoid expensive post-processing in the foundation.
- Pause or reduce rendering when the tab is hidden and use demand-based rendering for stable reduced-motion states.
- Record the production bundle delta in issue #123 before shell replacement proceeds.

The first production build keeps the main app chunk at about 425 kB minified / 134 kB gzip and lazy-loads the 3D scene as a separate roughly 888 kB minified / 239 kB gzip chunk. The 3D chunk exceeds Vite's default warning threshold but does not block route interactivity; later scene work must avoid adding post-processing, external models, or large textures without a new budget review.

## Visual And Accessibility Rules

- The spatial layer must never reduce text contrast or obscure controls.
- Desktop and mobile screenshots must show a nonblank scene or fallback with no incoherent overlap.
- Keyboard navigation, focus rings, touch targets, and route semantics remain DOM concerns.
- The scene may communicate inventory flow, availability, channel synchronization, or service health only when the same meaning is available in readable UI.

## Verification

Issue #117 verifies the architecture note, CSS fallback prototype, route preservation, responsive rendering, and reduced-motion behavior. Issue #123 installs the selected dependencies and adds canvas/WebGL runtime proof, bundle review, and non-WebGL failure verification.

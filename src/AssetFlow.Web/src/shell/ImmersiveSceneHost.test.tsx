import { render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { ImmersiveSceneHost } from "./ImmersiveSceneHost";
import { supportsWebGL } from "./webgl";

describe("ImmersiveSceneHost", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("keeps the CSS fallback when WebGL is unavailable", () => {
    vi.spyOn(HTMLCanvasElement.prototype, "getContext").mockReturnValue(null);
    expect(supportsWebGL()).toBe(false);
    render(<ImmersiveSceneHost />);

    expect(screen.getByTestId("immersive-backdrop")).toBeVisible();
    expect(screen.queryByTestId("immersive-scene-canvas")).not.toBeInTheDocument();
  });
});

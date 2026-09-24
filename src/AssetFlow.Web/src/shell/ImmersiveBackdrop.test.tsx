import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { ImmersiveBackdrop } from "./ImmersiveBackdrop";

describe("ImmersiveBackdrop", () => {
  it("renders a decorative fallback outside the accessibility tree", () => {
    render(<ImmersiveBackdrop />);

    const backdrop = screen.getByTestId("immersive-backdrop");
    expect(backdrop).toHaveAttribute("aria-hidden", "true");
    expect(backdrop.querySelectorAll(".spatial-node")).toHaveLength(4);
  });
});

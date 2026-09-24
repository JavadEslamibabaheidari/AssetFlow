import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { SettingsPage } from "./SettingsPage";

describe("SettingsPage", () => {
  it("shows the configured runtime API and inventory rules", () => {
    render(<SettingsPage />);

    expect(screen.getByRole("heading", { name: "Runtime connection" })).toBeVisible();
    expect(screen.getByText("http://localhost:8080")).toBeVisible();
    expect(screen.getByText("Active reservations reduce sellable stock")).toBeVisible();
  });
});

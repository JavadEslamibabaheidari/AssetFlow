import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { DefinitionList, InfoPanel, MetricCard, PageHeader, StatusBadge } from "./primitives";

describe("design-system primitives", () => {
  it("renders a route page header with an accessible title id", () => {
    render(
      <PageHeader
        eyebrow="Overview"
        title="Asset operations"
        titleId="overview-title"
        description="Manage market listings and warehouse stock."
      />
    );

    expect(screen.getByRole("heading", { name: "Asset operations" })).toHaveAttribute(
      "id",
      "overview-title"
    );
    expect(screen.getByText("Manage market listings and warehouse stock.")).toBeVisible();
  });

  it("renders compact operational cards and labels", () => {
    render(
      <>
        <MetricCard label="Markets" value="3" description="Connected channels." />
        <InfoPanel title="Warehouse stock">
          <p>Track sellable inventory.</p>
        </InfoPanel>
        <StatusBadge>Ready</StatusBadge>
      </>
    );

    expect(screen.getByText("Markets")).toBeVisible();
    expect(screen.getByText("3")).toBeVisible();
    expect(screen.getByRole("heading", { name: "Warehouse stock" })).toBeVisible();
    expect(screen.getByText("Ready")).toBeVisible();
  });

  it("renders settings as a semantic definition list", () => {
    render(
      <DefinitionList
        items={[
          { term: "Primary markets", description: "Amazon, MediaWorld, and Unieuro" },
          { term: "Availability rule", description: "Reservations reduce sellable stock" }
        ]}
      />
    );

    expect(screen.getByText("Primary markets")).toBeVisible();
    expect(screen.getByText("Amazon, MediaWorld, and Unieuro")).toBeVisible();
  });
});

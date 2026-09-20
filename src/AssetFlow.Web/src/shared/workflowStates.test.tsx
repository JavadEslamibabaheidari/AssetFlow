import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import { ApiError } from "../api/errors";
import {
  ApiErrorFeedback,
  WorkflowEmptyState,
  WorkflowFeedback,
  WorkflowLoadingState
} from "./workflowStates";

afterEach(() => {
  cleanup();
});

describe("workflow state components", () => {
  it("renders a polite loading state", () => {
    render(
      <WorkflowLoadingState
        title="Loading stock items"
        description="Checking the latest availability."
      />
    );

    expect(screen.getByRole("status")).toHaveTextContent("Loading stock items");
    expect(screen.getByText("Checking the latest availability.")).toBeVisible();
  });

  it("renders an empty state with a caller-provided action", () => {
    render(
      <WorkflowEmptyState
        title="No vendors yet"
        description="Create a vendor before adding products."
        action={<button type="button">Create vendor</button>}
      />
    );

    expect(screen.getByRole("heading", { name: "No vendors yet" })).toBeVisible();
    expect(screen.getByRole("button", { name: "Create vendor" })).toBeVisible();
  });

  it("uses status for success feedback and alert for blocking feedback", () => {
    render(
      <>
        <WorkflowFeedback kind="success" title="Vendor created" />
        <WorkflowFeedback kind="validation" title="Check the form" />
      </>
    );

    expect(screen.getByRole("status")).toHaveTextContent("Vendor created");
    expect(screen.getByRole("alert")).toHaveTextContent("Check the form");
  });

  it("maps API conflicts to inventory-specific feedback", () => {
    render(
      <ApiErrorFeedback
        error={
          new ApiError("conflict", "Reservation would oversell stock", 409, {
            type: "https://api.assetflow.local/problems/conflict",
            title: "Reservation would oversell stock",
            status: 409,
            detail: "Only 3 units are available."
          })
        }
      />
    );

    expect(screen.getByRole("alert")).toHaveTextContent(
      "This change conflicts with current inventory"
    );
    expect(screen.getByText("Only 3 units are available.")).toBeVisible();
  });
});

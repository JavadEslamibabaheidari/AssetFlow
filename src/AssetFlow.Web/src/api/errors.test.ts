import { describe, expect, it } from "vitest";
import { ApiError, mapStatusToErrorKind } from "./errors";

describe("API error mapping", () => {
  it("maps expected API statuses to UI-safe error kinds", () => {
    expect(mapStatusToErrorKind(400)).toBe("validation");
    expect(mapStatusToErrorKind(404)).toBe("notFound");
    expect(mapStatusToErrorKind(409)).toBe("conflict");
    expect(mapStatusToErrorKind(500)).toBe("unexpected");
  });

  it("preserves Problem Details on API errors", () => {
    const error = new ApiError("conflict", "Already exists", 409, {
      type: "https://api.assetflow.local/problems/conflict",
      title: "Already exists",
      status: 409
    });

    expect(error.kind).toBe("conflict");
    expect(error.status).toBe(409);
    expect(error.problem?.title).toBe("Already exists");
  });
});

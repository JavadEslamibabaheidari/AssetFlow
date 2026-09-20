import type { components } from "./generated/inventory-api";

export type ProblemDetails = components["schemas"]["ProblemDetails"];

export type ApiErrorKind = "validation" | "notFound" | "conflict" | "unexpected" | "network";

export class ApiError extends Error {
  public readonly kind: ApiErrorKind;
  public readonly status?: number;
  public readonly problem?: ProblemDetails;

  public constructor(
    kind: ApiErrorKind,
    message: string,
    status?: number,
    problem?: ProblemDetails
  ) {
    super(message);
    this.name = "ApiError";
    this.kind = kind;
    this.status = status;
    this.problem = problem;
  }
}

export function mapStatusToErrorKind(status: number): ApiErrorKind {
  if (status === 400) {
    return "validation";
  }

  if (status === 404) {
    return "notFound";
  }

  if (status === 409) {
    return "conflict";
  }

  return "unexpected";
}

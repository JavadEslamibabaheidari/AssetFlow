import type { ReactNode } from "react";
import { ApiError, type ApiErrorKind } from "../api/errors";

type WorkflowLoadingStateProps = {
  title: string;
  description?: string;
};

export function WorkflowLoadingState({ title, description }: WorkflowLoadingStateProps) {
  return (
    <div className="workflow-state" role="status" aria-live="polite">
      <div className="workflow-state-mark" aria-hidden="true" />
      <div>
        <h3>{title}</h3>
        {description ? <p>{description}</p> : null}
      </div>
    </div>
  );
}

type WorkflowEmptyStateProps = {
  title: string;
  description: string;
  action?: ReactNode;
};

export function WorkflowEmptyState({ title, description, action }: WorkflowEmptyStateProps) {
  return (
    <div className="workflow-state">
      <div>
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
      {action ? <div className="workflow-state-action">{action}</div> : null}
    </div>
  );
}

type WorkflowFeedbackKind = "success" | "validation" | "conflict" | "notFound" | "error";

type WorkflowFeedbackProps = {
  kind: WorkflowFeedbackKind;
  title: string;
  children?: ReactNode;
};

export function WorkflowFeedback({ kind, title, children }: WorkflowFeedbackProps) {
  const role = kind === "success" ? "status" : "alert";

  return (
    <div className={`workflow-feedback ${kind}`} role={role}>
      <strong>{title}</strong>
      {children ? <div>{children}</div> : null}
    </div>
  );
}

type ApiErrorFeedbackProps = {
  error: unknown;
};

export function ApiErrorFeedback({ error }: ApiErrorFeedbackProps) {
  const copy = getApiErrorCopy(error);

  return (
    <WorkflowFeedback kind={copy.kind} title={copy.title}>
      <p>{copy.description}</p>
    </WorkflowFeedback>
  );
}

type ApiErrorCopy = {
  kind: Exclude<WorkflowFeedbackKind, "success">;
  title: string;
  description: string;
};

function getApiErrorCopy(error: unknown): ApiErrorCopy {
  if (!(error instanceof ApiError)) {
    return {
      kind: "error",
      title: "Something went wrong",
      description: "The request could not be completed. Try again after checking the service."
    };
  }

  const detail = error.problem?.detail ?? error.problem?.title ?? error.message;

  return {
    kind: mapApiErrorKind(error.kind),
    title: getApiErrorTitle(error.kind),
    description: detail
  };
}

function mapApiErrorKind(kind: ApiErrorKind): Exclude<WorkflowFeedbackKind, "success"> {
  if (kind === "validation" || kind === "conflict" || kind === "notFound") {
    return kind;
  }

  return "error";
}

function getApiErrorTitle(kind: ApiErrorKind): string {
  switch (kind) {
    case "validation":
      return "Check the highlighted fields";
    case "conflict":
      return "This change conflicts with current inventory";
    case "notFound":
      return "This record is no longer available";
    case "network":
      return "AssetFlow could not reach the API";
    case "unexpected":
    default:
      return "AssetFlow could not complete the request";
  }
}

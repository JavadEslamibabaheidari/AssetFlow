import { Component, type ErrorInfo, type ReactNode } from "react";

type Props = {
  children: ReactNode;
};

type State = {
  hasError: boolean;
};

export class AppErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false
  };

  public static getDerivedStateFromError(): State {
    return { hasError: true };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
    console.error("AssetFlow shell error", error, errorInfo);
  }

  public render(): ReactNode {
    if (this.state.hasError) {
      return (
        <main className="error-screen" role="alert">
          <div>
            <p className="eyebrow">Application shell</p>
            <h1>Something interrupted the workspace.</h1>
            <p>
              Refresh the page to reload AssetFlow. If this repeats, capture the browser console
              output with the route you were visiting.
            </p>
            <button type="button" onClick={() => window.location.reload()}>
              Reload
            </button>
          </div>
        </main>
      );
    }

    return this.props.children;
  }
}

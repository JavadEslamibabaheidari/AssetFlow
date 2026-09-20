import { getAppConfig } from "../shared/config";
import { ApiError, mapStatusToErrorKind, type ProblemDetails } from "./errors";

type QueryValue = string | number | boolean | null | undefined;
type QueryParams = Record<string, QueryValue>;

type RequestOptions = {
  method?: "GET" | "POST";
  query?: QueryParams;
  body?: unknown;
  signal?: AbortSignal;
};

export class ApiHttpClient {
  private readonly baseUrl: string;
  private readonly fetcher: typeof fetch;

  public constructor(baseUrl = getAppConfig().apiBaseUrl, fetcher: typeof fetch = fetch) {
    this.baseUrl = baseUrl.replace(/\/$/, "");
    this.fetcher = fetcher;
  }

  public async request<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
    const url = this.buildUrl(path, options.query);

    let response: Response;

    try {
      response = await this.fetcher(url, {
        method: options.method ?? "GET",
        headers: {
          Accept: "application/json",
          ...(options.body === undefined ? {} : { "Content-Type": "application/json" })
        },
        body: options.body === undefined ? undefined : JSON.stringify(options.body),
        signal: options.signal
      });
    } catch (error) {
      throw new ApiError(
        "network",
        error instanceof Error ? error.message : "Network request failed"
      );
    }

    if (!response.ok) {
      throw await this.toApiError(response);
    }

    return (await response.json()) as TResponse;
  }

  private buildUrl(path: string, query?: QueryParams): string {
    const url = new URL(`${this.baseUrl}${path}`);

    Object.entries(query ?? {}).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        url.searchParams.set(key, String(value));
      }
    });

    return url.toString();
  }

  private async toApiError(response: Response): Promise<ApiError> {
    const problem = await readProblemDetails(response);
    const kind = mapStatusToErrorKind(response.status);
    const message = problem?.title ?? response.statusText ?? "Request failed";

    return new ApiError(kind, message, response.status, problem);
  }
}

async function readProblemDetails(response: Response): Promise<ProblemDetails | undefined> {
  const contentType = response.headers.get("content-type");

  if (!contentType?.includes("application/json")) {
    return undefined;
  }

  try {
    return (await response.json()) as ProblemDetails;
  } catch {
    return undefined;
  }
}

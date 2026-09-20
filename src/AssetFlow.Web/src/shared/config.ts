const defaultApiBaseUrl = "http://localhost:8080";

export type AppConfig = {
  apiBaseUrl: string;
};

export function getAppConfig(): AppConfig {
  return {
    apiBaseUrl: import.meta.env.VITE_ASSETFLOW_API_BASE_URL ?? defaultApiBaseUrl
  };
}

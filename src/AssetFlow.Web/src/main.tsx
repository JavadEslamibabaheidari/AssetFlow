import React, { Suspense } from "react";
import ReactDOM from "react-dom/client";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router-dom";
import { AppErrorBoundary } from "./shell/AppErrorBoundary";
import { AppLayout } from "./shell/AppLayout";
import { LoadingView } from "./shell/LoadingView";
import { InventoryPage } from "./pages/InventoryPage";
import { OperationsPage } from "./pages/OperationsPage";
import { OverviewPage } from "./pages/OverviewPage";
import { ReservationsPage } from "./pages/ReservationsPage";
import { SettingsPage } from "./pages/SettingsPage";
import "./styles.css";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 30_000
    }
  }
});

const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <AppErrorBoundary>
        <Suspense fallback={<LoadingView />}>
          <AppLayout />
        </Suspense>
      </AppErrorBoundary>
    ),
    children: [
      { index: true, element: <OverviewPage /> },
      { path: "inventory", element: <InventoryPage /> },
      { path: "reservations", element: <ReservationsPage /> },
      { path: "operations", element: <OperationsPage /> },
      { path: "settings", element: <SettingsPage /> },
      { path: "*", element: <Navigate to="/" replace /> }
    ]
  }
]);

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </React.StrictMode>
);

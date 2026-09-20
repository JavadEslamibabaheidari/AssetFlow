import {
  Boxes,
  Gauge,
  LayoutDashboard,
  PackageCheck,
  Settings,
  TimerReset
} from "lucide-react";
import { NavLink, Outlet } from "react-router-dom";
import { getAppConfig } from "../shared/config";

const navItems = [
  { to: "/", label: "Overview", icon: LayoutDashboard, end: true },
  { to: "/inventory", label: "Inventory", icon: Boxes },
  { to: "/reservations", label: "Reservations", icon: TimerReset },
  { to: "/operations", label: "Operations", icon: Gauge },
  { to: "/settings", label: "Settings", icon: Settings }
];

export function AppLayout() {
  const config = getAppConfig();

  return (
    <div className="app-shell">
      <aside className="sidebar" aria-label="Primary navigation">
        <div className="brand-block">
          <div className="brand-mark" aria-hidden="true">
            <PackageCheck size={22} />
          </div>
          <div>
            <p>AssetFlow</p>
            <span>Inventory operations</span>
          </div>
        </div>

        <nav className="nav-list">
          {navItems.map((item) => {
            const Icon = item.icon;

            return (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.end}
                className={({ isActive }) => `nav-link${isActive ? " active" : ""}`}
              >
                <Icon size={18} aria-hidden="true" />
                <span>{item.label}</span>
              </NavLink>
            );
          })}
        </nav>

        <div className="sidebar-meta">
          <span>API base</span>
          <strong>{config.apiBaseUrl}</strong>
        </div>
      </aside>

      <div className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">M5 foundation</p>
            <h1>Frontend workspace</h1>
          </div>
          <div className="status-strip" aria-label="Foundation status">
            <span>Shell ready</span>
            <span>Contracts next</span>
          </div>
        </header>

        <main className="content" id="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

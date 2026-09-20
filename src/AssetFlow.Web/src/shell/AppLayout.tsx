import {
  Boxes,
  LayoutDashboard,
  PackageCheck,
  Settings,
  Store,
  TimerReset
} from "lucide-react";
import { NavLink, Outlet } from "react-router-dom";

const navItems = [
  { to: "/", label: "Overview", icon: LayoutDashboard, end: true },
  { to: "/inventory", label: "Assets", icon: Boxes },
  { to: "/reservations", label: "Reservations", icon: TimerReset },
  { to: "/operations", label: "Markets", icon: Store },
  { to: "/settings", label: "Settings", icon: Settings }
];

export function AppLayout() {
  return (
    <div className="app-shell">
      <aside className="sidebar" aria-label="Primary navigation">
        <div className="brand-block">
          <div className="brand-mark" aria-hidden="true">
            <PackageCheck size={22} />
          </div>
          <div>
            <p>AssetFlow</p>
            <span>Marketplace inventory</span>
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

      </aside>

      <div className="workspace">
        <header className="topbar">
          <div>
            <p className="eyebrow">Operations</p>
            <h1>Asset command center</h1>
          </div>
        </header>

        <main className="content" id="main-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

import { AnimatePresence, LayoutGroup, MotionConfig, motion } from "motion/react";
import {
  Activity,
  Boxes,
  LayoutDashboard,
  PackageCheck,
  Radio,
  Settings,
  Store,
  TimerReset
} from "lucide-react";
import { NavLink, Outlet, useLocation } from "react-router-dom";
import { ImmersiveSceneHost } from "./ImmersiveSceneHost";

const navItems = [
  { to: "/", label: "Overview", context: "Network overview", icon: LayoutDashboard, end: true },
  { to: "/inventory", label: "Assets", context: "Inventory workspace", icon: Boxes },
  { to: "/reservations", label: "Reservations", context: "Reservation control", icon: TimerReset },
  { to: "/operations", label: "Markets", context: "Channel operations", icon: Store },
  { to: "/settings", label: "Settings", context: "Configuration workspace", icon: Settings }
];

export function AppLayout() {
  const location = useLocation();
  const activeRoute = navItems.find((item) =>
    item.end ? location.pathname === item.to : location.pathname.startsWith(item.to)
  );

  return (
    <MotionConfig reducedMotion="user" transition={{ duration: 0.22, ease: [0.22, 1, 0.36, 1] }}>
      <div className="app-shell">
        <ImmersiveSceneHost />
        <a className="skip-link" href="#main-content">
          Skip to workspace
        </a>
        <aside className="sidebar" aria-label="Primary navigation">
          <motion.div
            className="brand-block"
            initial={{ opacity: 0, y: -8 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <motion.div
              className="brand-mark"
              aria-hidden="true"
              whileHover={{ rotate: -4, scale: 1.04 }}
              whileTap={{ scale: 0.96 }}
            >
              <PackageCheck size={22} />
            </motion.div>
            <div>
              <p>AssetFlow</p>
              <span>Marketplace inventory</span>
            </div>
          </motion.div>

          <LayoutGroup>
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
                    {({ isActive }) => (
                      <>
                        {isActive ? (
                          <motion.span
                            className="nav-link-indicator"
                            layoutId="active-nav-link"
                            aria-hidden="true"
                          />
                        ) : null}
                        <motion.span
                          className="nav-link-content"
                          whileHover={{ x: 3 }}
                          whileTap={{ scale: 0.98 }}
                        >
                          <Icon size={18} aria-hidden="true" />
                          <span>{item.label}</span>
                        </motion.span>
                      </>
                    )}
                  </NavLink>
                );
              })}
            </nav>
          </LayoutGroup>

          <div className="sidebar-meta" aria-label="Workspace status">
            <span>Workspace signal</span>
            <strong>
              <Radio size={14} aria-hidden="true" /> Connected
            </strong>
          </div>
        </aside>

        <div className="workspace">
          <header className="topbar">
            <div className="topbar-title">
              <span className="topbar-signal" aria-hidden="true">
                <Activity size={17} />
              </span>
              <div>
                <p className="eyebrow">Asset command</p>
                <h1>{activeRoute?.context ?? "Operations workspace"}</h1>
              </div>
            </div>
            <div className="status-strip" aria-label="System status">
              <span className="status-badge status-badge-success">System live</span>
              <span className="status-badge status-badge-neutral">3 markets</span>
            </div>
          </header>

          <div className="content" id="main-content">
            <AnimatePresence mode="wait">
              <motion.main
                key={location.pathname}
                initial={{ opacity: 0, y: 14, filter: "blur(8px)" }}
                animate={{ opacity: 1, y: 0, filter: "blur(0px)" }}
                exit={{ opacity: 0, y: -10, filter: "blur(6px)" }}
              >
                <Outlet />
              </motion.main>
            </AnimatePresence>
          </div>
        </div>
      </div>
    </MotionConfig>
  );
}

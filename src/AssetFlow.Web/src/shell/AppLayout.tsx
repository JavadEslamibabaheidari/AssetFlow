import { AnimatePresence, LayoutGroup, MotionConfig, motion } from "motion/react";
import { Boxes, LayoutDashboard, PackageCheck, Settings, Store, TimerReset } from "lucide-react";
import { NavLink, Outlet, useLocation } from "react-router-dom";
import { ImmersiveSceneHost } from "./ImmersiveSceneHost";

const navItems = [
  { to: "/", label: "Overview", icon: LayoutDashboard, end: true },
  { to: "/inventory", label: "Assets", icon: Boxes },
  { to: "/reservations", label: "Reservations", icon: TimerReset },
  { to: "/operations", label: "Markets", icon: Store },
  { to: "/settings", label: "Settings", icon: Settings }
];

export function AppLayout() {
  const location = useLocation();

  return (
    <MotionConfig reducedMotion="user" transition={{ duration: 0.22, ease: [0.22, 1, 0.36, 1] }}>
      <div className="app-shell">
        <ImmersiveSceneHost />
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
        </aside>

        <div className="workspace">
          <header className="topbar">
            <div>
              <p className="eyebrow">Operations</p>
              <h1>Asset command center</h1>
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

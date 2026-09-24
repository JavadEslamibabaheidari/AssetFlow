import { lazy, Suspense, useMemo } from "react";
import { useReducedMotion } from "motion/react";
import { ImmersiveBackdrop } from "./ImmersiveBackdrop";
import { supportsWebGL } from "./webgl";

const ImmersiveScene = lazy(() => import("./ImmersiveScene"));

export function ImmersiveSceneHost() {
  const reduceMotion = useReducedMotion();
  const canRenderScene = useMemo(() => supportsWebGL(), []);

  return (
    <div className="immersive-scene-host" aria-hidden="true">
      <ImmersiveBackdrop />
      {canRenderScene && !reduceMotion ? (
        <Suspense fallback={null}>
          <ImmersiveScene />
        </Suspense>
      ) : null}
    </div>
  );
}

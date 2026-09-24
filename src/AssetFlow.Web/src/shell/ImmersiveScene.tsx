import { Float, Line } from "@react-three/drei";
import { Canvas, useFrame } from "@react-three/fiber";
import { useRef } from "react";
import type { Group } from "three";

const routePoints: [number, number, number][] = [
  [-3.4, -1.1, 0],
  [-1.5, 0.2, 0.45],
  [0.2, -0.45, 0.9],
  [1.8, 0.5, 0.35],
  [3.6, -0.15, 0]
];

function FlowNetwork() {
  const group = useRef<Group>(null);

  useFrame((_, delta) => {
    if (group.current) {
      group.current.rotation.y += delta * 0.045;
      group.current.rotation.z = Math.sin(Date.now() * 0.00012) * 0.035;
    }
  });

  return (
    <group ref={group} rotation={[-0.16, -0.28, -0.08]}>
      <Line points={routePoints} color="#54f2ca" lineWidth={1.4} transparent opacity={0.72} />
      {routePoints.map((point, index) => (
        <Float key={point.join(":")} speed={0.7 + index * 0.08} floatIntensity={0.16}>
          <mesh position={point}>
            <sphereGeometry args={[index === 2 ? 0.24 : 0.13, 18, 18]} />
            <meshStandardMaterial
              color={index === 2 ? "#76ffe2" : "#58a6ff"}
              emissive={index === 2 ? "#1abf99" : "#245fc7"}
              emissiveIntensity={1.4}
              roughness={0.25}
              metalness={0.35}
            />
          </mesh>
        </Float>
      ))}

      <mesh position={[0.2, -0.45, 0.9]} rotation={[0.9, 0.3, 0.2]}>
        <torusGeometry args={[0.72, 0.018, 10, 72]} />
        <meshBasicMaterial color="#8bffe8" transparent opacity={0.58} />
      </mesh>
      <mesh position={[0.2, -0.45, 0.9]} rotation={[1.25, -0.4, 0.8]}>
        <torusGeometry args={[1.12, 0.012, 10, 72]} />
        <meshBasicMaterial color="#4c8dff" transparent opacity={0.36} />
      </mesh>
    </group>
  );
}

export default function ImmersiveScene() {
  return (
    <Canvas
      className="immersive-scene-canvas"
      data-testid="immersive-scene-canvas"
      aria-hidden="true"
      camera={{ position: [0, 0.2, 7.6], fov: 48 }}
      dpr={[1, 1.5]}
      gl={{ alpha: true, antialias: true, powerPreference: "low-power" }}
    >
      <ambientLight intensity={0.45} />
      <directionalLight position={[2, 4, 5]} intensity={1.1} color="#d9fff5" />
      <pointLight position={[-3, -1, 3]} intensity={18} distance={8} color="#1de0af" />
      <pointLight position={[3, 1, 2]} intensity={12} distance={7} color="#3f7dff" />
      <FlowNetwork />
    </Canvas>
  );
}

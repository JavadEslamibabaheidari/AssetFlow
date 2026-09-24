export function ImmersiveBackdrop() {
  return (
    <div className="immersive-backdrop" data-testid="immersive-backdrop" aria-hidden="true">
      <div className="spatial-grid" />
      <div className="spatial-arc spatial-arc-primary" />
      <div className="spatial-arc spatial-arc-secondary" />
      <div className="spatial-route">
        <span className="spatial-node node-a" />
        <span className="spatial-node node-b" />
        <span className="spatial-node node-c" />
        <span className="spatial-node node-d" />
      </div>
    </div>
  );
}

import { apiAreas } from "../data";
import { useReveal } from "../useReveal";

export function Api() {
  const { ref, visible } = useReveal();
  const max = Math.max(...apiAreas.map((a) => a.count));
  return (
    <section id="api" className="section api" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">API surface</p>
        <h2 className="section__title">60+ endpoints across the gateway</h2>
        <p className="section__sub">
          REST for clients, gRPC for service-to-service, JWT for auth. Full contract at{" "}
          <code>/swagger</code>.
        </p>
        <div className="api__grid">
          {apiAreas.map((a, i) => (
            <div
              key={a.area}
              className="api__row"
              style={{ transitionDelay: `${i * 50}ms` }}
            >
              <div className="api__label">
                <span className="api__name">{a.area}</span>
                <span className="api__methods">{a.methods}</span>
              </div>
              <div className="api__bar">
                <span
                  className="api__bar-fill"
                  style={{ width: `${(a.count / max) * 100}%` }}
                />
              </div>
              <span className="api__count">{a.count}</span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

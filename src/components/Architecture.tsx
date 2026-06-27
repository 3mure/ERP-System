import { architecture } from "../data";
import { useReveal } from "../useReveal";

export function Architecture() {
  const { ref, visible } = useReveal();
  return (
    <section id="architecture" className="section arch" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">Architecture</p>
        <h2 className="section__title">Layered, event-driven, observable</h2>
        <div className="arch__layout">
          <div className="arch__diagram" aria-hidden="true">
            {architecture.layers.map((l, i) => (
              <div key={l.title} className="arch__layer">
                <div className="arch__layer-inner">
                  <span className="arch__num">L{i}</span>
                  <div>
                    <strong>{l.title}</strong>
                    <p>{l.detail}</p>
                  </div>
                </div>
                {i < architecture.layers.length - 1 && <span className="arch__connector" />}
              </div>
            ))}
          </div>
          <div className="arch__flow">
            <div className="arch__flow-card">
              <h3>Request path</h3>
              <p>Client → API Gateway → downstream service → MediatR handler → EF Core → SQL Server, with Redis cache reads on hot paths.</p>
            </div>
            <div className="arch__flow-card">
              <h3>Event path</h3>
              <p>Handler writes to domain + transactional outbox → MassTransit publisher → RabbitMQ → Notification, Payment, and Catalog consumers.</p>
            </div>
            <div className="arch__flow-card">
              <h3>gRPC path</h3>
              <p>External clients call Catalog and Promotion gRPC contracts directly for low-latency product and loyalty lookups.</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

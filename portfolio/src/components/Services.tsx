import { services } from "../data";
import { useReveal } from "../useReveal";

export function Services() {
  const { ref, visible } = useReveal();
  return (
    <section id="services" className="section services" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">Microservices</p>
        <h2 className="section__title">Six services, one platform</h2>
        <p className="section__sub">
          Each service owns its domain and communicates through REST, gRPC, and RabbitMQ events.
        </p>
        <div className="services__grid">
          {services.map((s, i) => (
            <article
              key={s.id}
              className={`service service--${s.accent}`}
              style={{ transitionDelay: `${i * 70}ms` }}
            >
              <div className="service__head">
                <span className="service__icon" aria-hidden="true">
                  <span className="service__dot" />
                </span>
                <div>
                  <h3>{s.name}</h3>
                  <p className="service__role">{s.role}</p>
                </div>
              </div>
              <p className="service__desc">{s.description}</p>
              <ul className="service__points">
                {s.points.map((p) => (
                  <li key={p}>
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none">
                      <path d="M5 12l5 5L20 7" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                    {p}
                  </li>
                ))}
              </ul>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

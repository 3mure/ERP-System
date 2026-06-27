import { profile } from "../data";
import { useReveal } from "../useReveal";

export function Contact() {
  const { ref, visible } = useReveal();
  return (
    <section id="contact" className="section contact" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">Contact</p>
        <h2 className="section__title">Let's build something reliable</h2>
        <p className="contact__lead">
          Open to backend engineering roles, freelance microservice work, and architecture review.
          Reach me through any of the channels below.
        </p>
        <div className="contact__cards">
          <a className="contact__card" href={profile.github} target="_blank" rel="noreferrer">
            <span className="contact__card-label">Code</span>
            <span className="contact__card-value">GitHub / 3mure</span>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
              <path d="M7 17L17 7M9 7h8v8" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </a>
          <a className="contact__card" href={profile.liveDemo} target="_blank" rel="noreferrer">
            <span className="contact__card-label">Live API</span>
            <span className="contact__card-value">management.runasp.net</span>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
              <path d="M7 17L17 7M9 7h8v8" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </a>
          <a className="contact__card" href={profile.swagger} target="_blank" rel="noreferrer">
            <span className="contact__card-label">Docs</span>
            <span className="contact__card-value">Swagger / OpenAPI</span>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
              <path d="M7 17L17 7M9 7h8v8" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </a>
        </div>
      </div>
    </section>
  );
}

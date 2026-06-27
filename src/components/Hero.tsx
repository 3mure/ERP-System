import { profile, stats } from "../data";

export function Hero() {
  return (
    <section id="top" className="hero">
      <div className="hero__grid" aria-hidden="true" />
      <div className="hero__glow" aria-hidden="true" />
      <div className="hero__inner">
        <span className="hero__badge">
          <span className="hero__dot" /> Available for backend roles
        </span>
        <h1 className="hero__title">
          {profile.name.split(" ")[0]}
          <span className="hero__title-accent"> {profile.name.split(" ")[1]}</span>
        </h1>
        <p className="hero__role">{profile.role}</p>
        <p className="hero__tagline">{profile.tagline}</p>
        <div className="hero__actions">
          <a href={profile.liveDemo} target="_blank" rel="noreferrer" className="btn btn--primary">
            View live API
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none">
              <path d="M5 12h14M13 6l6 6-6 6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </a>
          <a href={profile.github} target="_blank" rel="noreferrer" className="btn btn--ghost">
            GitHub
          </a>
          <a href={profile.swagger} target="_blank" rel="noreferrer" className="btn btn--ghost">
            Swagger docs
          </a>
        </div>
        <dl className="hero__stats">
          {stats.map((s) => (
            <div key={s.label} className="hero__stat">
              <dt>{s.value}</dt>
              <dd>{s.label}</dd>
            </div>
          ))}
        </dl>
      </div>
      <a href="#about" className="hero__scroll" aria-label="Scroll to about">
        <span />
      </a>
    </section>
  );
}

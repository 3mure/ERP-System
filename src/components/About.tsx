import { profile, principles } from "../data";
import { useReveal } from "../useReveal";

export function About() {
  const { ref, visible } = useReveal();
  return (
    <section id="about" className="section about" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">About</p>
        <h2 className="section__title">Engineering for real retail operations</h2>
        <p className="about__lead">{profile.summary}</p>
        <div className="about__grid">
          {principles.map((p, i) => (
            <article
              key={p.title}
              className="about__card"
              style={{ transitionDelay: `${i * 80}ms` }}
            >
              <span className="about__index">0{i + 1}</span>
              <h3>{p.title}</h3>
              <p>{p.body}</p>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

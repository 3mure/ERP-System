import { techStack } from "../data";
import { useReveal } from "../useReveal";

export function Stack() {
  const { ref, visible } = useReveal();
  const categories = Array.from(new Set(techStack.map((t) => t.category)));
  return (
    <section id="stack" className="section stack" ref={ref}>
      <div className={`reveal ${visible ? "is-in" : ""}`}>
        <p className="eyebrow">Tech stack</p>
        <h2 className="section__title">Tools I reach for</h2>
        <div className="stack__groups">
          {categories.map((cat) => (
            <div key={cat} className="stack__group">
              <h3 className="stack__cat">{cat}</h3>
              <div className="stack__chips">
                {techStack
                  .filter((t) => t.category === cat)
                  .map((t) => (
                    <span key={t.name} className="chip">
                      {t.name}
                    </span>
                  ))}
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

import { useEffect, useState } from "react";

export function Nav() {
  const [scrolled, setScrolled] = useState(false);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 24);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  const links = [
    { href: "#about", label: "About" },
    { href: "#services", label: "Services" },
    { href: "#architecture", label: "Architecture" },
    { href: "#api", label: "API" },
    { href: "#stack", label: "Stack" },
    { href: "#contact", label: "Contact" },
  ];

  return (
    <header className={`nav ${scrolled ? "nav--solid" : ""}`}>
      <a href="#top" className="nav__brand" aria-label="Home">
        <span className="nav__mark">
          <svg width="22" height="22" viewBox="0 0 32 32" fill="none">
            <path d="M9 9h14M9 16h14M9 23h8" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" />
          </svg>
        </span>
        <span>Omar Shipl</span>
      </a>
      <nav className={`nav__links ${open ? "is-open" : ""}`}>
        {links.map((l) => (
          <a key={l.href} href={l.href} onClick={() => setOpen(false)}>
            {l.label}
          </a>
        ))}
        <a className="nav__cta" href="#contact" onClick={() => setOpen(false)}>
          Get in touch
        </a>
      </nav>
      <button
        className="nav__toggle"
        aria-label="Toggle menu"
        aria-expanded={open}
        onClick={() => setOpen((v) => !v)}
      >
        <span />
        <span />
        <span />
      </button>
    </header>
  );
}

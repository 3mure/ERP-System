import { profile } from "../data";

export function Footer() {
  return (
    <footer className="footer">
      <div className="footer__inner">
        <div className="footer__brand">
          <span className="footer__mark">
            <svg width="20" height="20" viewBox="0 0 32 32" fill="none">
              <path d="M9 9h14M9 16h14M9 23h8" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" />
            </svg>
          </span>
          <span>{profile.name}</span>
        </div>
        <p className="footer__note">
          Built as a graduation project. ASP.NET Core 8 microservices, CQRS, gRPC, and event-driven architecture.
        </p>
        <p className="footer__copy">© {new Date().getFullYear()} {profile.name}. Academic / portfolio use.</p>
      </div>
    </footer>
  );
}

import { Nav } from "./components/Nav";
import { Hero } from "./components/Hero";
import { About } from "./components/About";
import { Services } from "./components/Services";
import { Architecture } from "./components/Architecture";
import { Api } from "./components/Api";
import { Stack } from "./components/Stack";
import { Contact } from "./components/Contact";
import { Footer } from "./components/Footer";

export default function App() {
  return (
    <>
      <Nav />
      <main>
        <Hero />
        <About />
        <Services />
        <Architecture />
        <Api />
        <Stack />
        <Contact />
      </main>
      <Footer />
    </>
  );
}

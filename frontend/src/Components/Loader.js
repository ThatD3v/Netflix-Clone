import "./Loader.css";

function Loader() {
  return (
    <div className="loader-overlay" aria-live="polite" aria-busy="true">
      <div className="loader" aria-label="Loading" />
    </div>
  );
}

export default Loader;

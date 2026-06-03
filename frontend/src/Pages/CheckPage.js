import "../Styles/CheckPage.css";

function CheckPage() {
  return (
    <div className="checkPage">
      <header>
        <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"150px"} />
      </header>
      <div className="check-section">
        <div className="text">
          <h1>Enter your info to sign in</h1>
          <h3>Or get started with a new account</h3>
        </div>
        <form>
          <input type="email" placeholder="Email address" />
          <br />
          <button>Continue</button>
        </form>
      </div>
    </div>
  );
}

export default CheckPage;

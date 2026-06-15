import "../Styles/HomePage.css";
import { CiSearch } from "react-icons/ci";
import { CiBellOn } from "react-icons/ci";

function HomePage() {
  return (
    <div className="homepage">
      <div className="homeHeader">
        <div className="leftSide">
          <img src="Netflix Logo/Netflix.png" alt="Netflix" width={"90px"} />
          <ul>
            <li>Home</li>
            <li>Shows</li>
            <li>Movies</li>
            <li>New & Popular</li>
            <li>My List</li>
          </ul>
        </div>
        <div className="rightSide">
          <CiSearch />
          <CiBellOn />
        </div>
      </div>
    </div>
  );
}

export default HomePage;

import "./Plan.css";

function Plan({ plan, gradient }) {
  return (
    <div className="plan">
      <div className="colors" style={{ background: gradient }}>
        <h3>{plan.name}</h3>
        <h4>{plan.videoQuality}</h4>
      </div>
      <br />
      <div>
        <h4 className="sub">Monthly price</h4>
        <h3 className="main">{plan.videoQuality}</h3>
      </div>
      <div>
        <h4 className="sub">Video and sound quality</h4>
        <h3 className="main">NGN {plan.price}</h3>
      </div>
      <div>
        <h4 className="sub">Resolution</h4>
        <h3 className="main">{plan.videoQuality}</h3>
      </div>
      <div>
        <h4 className="sub">Supported devices</h4>
        <h3 className="main">{plan.description}</h3>
      </div>
      <div>
        <h4 className="sub">
          Devices your household can watch at the same time
        </h4>
        <h3 className="main">{plan.videoQuality}</h3>
      </div>
      <div>
        <h4 className="sub">Download devices</h4>
        <h3 className="main">{plan.downloadDevices}</h3>
      </div>
    </div>
  );
}

export default Plan;

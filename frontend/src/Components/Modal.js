import React from "react";
import ReactDOM from "react-dom";
import { MdOutlineCancel } from "react-icons/md";

const overlayStyle = {
  position: "fixed",
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundColor: "black",
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  zIndex: 1000,
};

const modalStyle = {
  backgroundColor: "#161616",
  padding: "50px 30px",
  borderRadius: "8px",
  position: "relative",
  minWidth: "300px",
  width: "50%",
  height: "75%",
  border: "1px solid rgba(255, 255, 255, 0.3)",
};

const closeButtonStyle = {
  position: "absolute",
  top: "20px",
  right: "20px",
  cursor: "pointer",
  color: "white",
  fontSize: "2rem",
};

function Modal({ open, close, children }) {
  if (open)
    return ReactDOM.createPortal(
      <div className="modal" style={overlayStyle}>
        <div style={modalStyle}>
          <MdOutlineCancel onClick={close} style={closeButtonStyle} />
          {children}
        </div>
      </div>,
      document.body,
    );
}

export default Modal;

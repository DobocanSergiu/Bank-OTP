import { useEffect, useState } from "react";
import styles from "./OtpPage.module.css";
import { useNavigate } from "react-router";
import { Toaster } from "react-hot-toast";
import { toast } from "react-hot-toast";
function OtpPage() {
  const [errorVisibility, setErrorVisibility] = useState(false);
  const [userInput, setUserInput] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (localStorage.getItem("username") == null) {
      navigate("/login");
    }
  }, []);
  async function handleLoginButton() {
    const response = await fetch("https://localhost:7229/api/validateCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Username: encodeURIComponent(localStorage.getItem("username")),
        Code: userInput,
      }),
    });

    if (response.ok) {
      navigate("/account");
    } else {
      setErrorVisibility(true);
    }
  }

  async function handleRequestPasscodeButton() {
    const response = await fetch("https://localhost:7229/api/requestCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Username: encodeURIComponent(localStorage.getItem("username")),
      }),
    });

    if (response.ok) {
      var data = await response.json();
      var time = parseInt(data.time);

      toast.dismiss();
      toast(`OTP CODE: ${data.code}`, {
        duration: parseInt(time) * 1000, ///Toast message lifetime is in miliseconds
        position: "bottom-right",
        style: {
          paddingBottom: "10px",
          backgroundColor: "#6728ce",
          color: "white",
          fontFamily: "DM Mono",
        },
      });
    } else {
      toast.dismiss();
      toast("An error occurred, please try again", {
        duration: 10000,
        position: "bottom-right",
        style: {
          paddingBottom: "10px", // Space for progress bar
          backgroundColor: "red",
          color: "white",
          fontFamily: "DM Mono",
        },
      });
    }
  }

  return (
    <div className={styles.otpPageContainer}>
      <div className={styles.header}>One time passcode check</div>
      <input
        type="text"
        className={styles.inputTextBox}
        onChange={(e) => setUserInput(e.target.value)}
        placeholder="Enter OTP"
      ></input>
      {errorVisibility && (
        <div className={styles.otpError}>Invalid OTP, Try Again</div>
      )}
      <button
        className={styles.inputButton}
        onClick={() => handleLoginButton()}
      >
        Login
      </button>
      <button
        className={styles.inputButton}
        onClick={() => handleRequestPasscodeButton()}
      >
        Request New Passcode
      </button>
      <Toaster />
    </div>
  );
}
export default OtpPage;

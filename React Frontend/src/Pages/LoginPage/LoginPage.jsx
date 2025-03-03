import styles from "./LoginPage.module.css";
import { useState } from "react";
import { useNavigate } from "react-router";

function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const [emptyFieldErrorVisibility, setEmptyFieldErrorVisibility] =
    useState(false);
  const [userNotFoundError, setUserNotFoundError] = useState(false);
  const [generalError, setGeneralError] = useState(false);
  const navigate = useNavigate();
  async function handleLoginButtonClick() {
    if (username.trimEnd().length == 0 || password.trimEnd().length == 0) {
      setEmptyFieldErrorVisibility(true);
      setUserNotFoundError(false);
    } else {
      try {
        const encodedUsername = encodeURIComponent(username);
        const encodedPassword = encodeURIComponent(password);

        const response = await fetch("https://localhost:7229/api/loginUser", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            Username: encodedUsername,
            Password: encodedPassword,
          }),
        });
        if (response.ok) {
          localStorage.setItem("username", encodedUsername);
          navigate("/otp");
        } else if (response.status === 401 || response.status === 404) {
          setUserNotFoundError(true);
          setEmptyFieldErrorVisibility(false);
          setGeneralError(false);
        }
      } catch (error) {
        console.log(error);
        setGeneralError(true);
        setUserNotFoundError(false);
        setEmptyFieldErrorVisibility(false);
      }
    }
  }
  return (
    <div className={styles.loginPageContainer}>
      <div className={styles.header}>Login</div>
      <div className={styles.subHeader}>Username:</div>
      <input
        type="text"
        className={styles.inputTextBox}
        placeholder="Enter Username"
        onChange={(e) => setUsername(e.target.value)}
      ></input>
      <div className={styles.subHeader}>Password:</div>
      <input
        type="password"
        className={styles.inputTextBox}
        placeholder="Enter Password"
        onChange={(e) => setPassword(e.target.value)}
      ></input>
      {emptyFieldErrorVisibility && (
        <div className={styles.loginError}>
          Username and Password fields must not be left empty
        </div>
      )}
      {userNotFoundError && (
        <div className={styles.loginError}>
          User with given credentials was not found
        </div>
      )}
      {generalError && (
        <div className={styles.loginError}>An error occured</div>
      )}
      <button
        className={styles.inputButton}
        onClick={() => handleLoginButtonClick()}
      >
        Login
      </button>
      <a href="createUser" className={styles.newUserLink}>
        New User? Create an account
      </a>
    </div>
  );
}
export default LoginPage;

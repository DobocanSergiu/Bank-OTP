import styles from "./CreateUser.module.css";
import { useState } from "react";
import { useNavigate } from "react-router";
function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [emptyFieldErrorVisibility, setEmptyFieldErrorVisibility] =
    useState(false);
  const [userFoundError, setUserFoundError] = useState(false);
  const [generalError, setGeneralError] = useState(false);
  const [passwordMatch, setPasswordMatch] = useState(true);
  const [complexPasswordError, setComplexPasswordError] = useState(false);
  const navigate = useNavigate();
  async function handleLoginButtonClick() {
    if (
      username.trimEnd().length == 0 ||
      password.trimEnd().length == 0 ||
      repeatPassword.trimEnd().length == 0
    ) {
      setEmptyFieldErrorVisibility(true);
      setUserFoundError(false);
      setGeneralError(false);
      setPasswordMatch(true);
      setComplexPasswordError(false);
    } else if (password != repeatPassword) {
      setPasswordMatch(false);
      setEmptyFieldErrorVisibility(false);
      setUserFoundError(false);
      setGeneralError(false);
      setComplexPasswordError(false);
    } else if (
      password.length < 8 ||
      !/[A-Z]/.test(password) ||
      !/[0-9]/.test(password) ||
      !/[!@#$%^&*()_+=~`';:<>,.]/.test(password)
    ) {
      setComplexPasswordError(true);
      setPasswordMatch(true); // If you still want to keep the password match check
      setEmptyFieldErrorVisibility(false);
      setUserFoundError(false);
      setGeneralError(false);
    } else {
      try {
        const encodedUsername = encodeURIComponent(username);
        const encodedPassword = encodeURIComponent(password);

        const response = await fetch(`https://localhost:7229/api/createUser/`, {
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
          navigate("/login");
        } else if (response.status === 409) {
          setUserFoundError(true);
          setEmptyFieldErrorVisibility(false);
          setGeneralError(false);
          setPasswordMatch(true);
          setComplexPasswordError(true);

          ///Username found
        } else if (response.status === 400) {
          console.log("Password is not complex enough");
          setGeneralError(false);
          setUserFoundError(false);
          setEmptyFieldErrorVisibility(false);
          setPasswordMatch(true);
          setComplexPasswordError(true);
        }
      } catch (error) {
        //general error
        setGeneralError(true);
        setUserFoundError(false);
        setEmptyFieldErrorVisibility(false);
        setPasswordMatch(true);
        setComplexPasswordError(false);
      }
    }
  }
  return (
    <div className={styles.createUserPageContainer}>
      <div className={styles.header}>Create User:</div>
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

      <div className={styles.subHeader}>Confirm Password:</div>
      <input
        type="password"
        className={styles.inputTextBox}
        placeholder="Enter Password"
        onChange={(e) => setRepeatPassword(e.target.value)}
      ></input>

      {emptyFieldErrorVisibility && (
        <div className={styles.loginError}>
          Username and Password fields must not be left empty
        </div>
      )}
      {userFoundError && (
        <div className={styles.loginError}>
          User with given username already exists
        </div>
      )}
      {generalError && (
        <div className={styles.loginError}>An error occured</div>
      )}
      {passwordMatch == false && (
        <div className={styles.loginError}>Passwords must match</div>
      )}
      {complexPasswordError && (
        <div className={styles.loginError}>
          <p>Password must meet the following requirements:</p>
          <ul>
            <li>At least 8 characters long</li>
            <li>Contain at least one number</li>
            <li>Contain at least one uppercase letter</li>
            <li>
              Contain at least one special character
              (!@#$%^&*()_+=~`';:&lt;&gt;,.)
            </li>
          </ul>
        </div>
      )}
      <button
        className={styles.inputButton}
        onClick={() => handleLoginButtonClick()}
      >
        Login
      </button>
    </div>
  );
}

export default LoginPage;

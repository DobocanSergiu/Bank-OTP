import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import OtpPage from "./Pages/OtpPage/OtpPage.jsx";
import LoginPage from "./Pages/LoginPage/LoginPage.jsx";
import CreateUserPage from "./Pages/CreateUserPage/CreateUserPage.jsx";
import AccountPage from "./Pages/AccountPage/AccountPage.jsx";

function App() {
  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/otp" element={<OtpPage />} />
          <Route path="/createUser" element={<CreateUserPage />} />
          <Route path="/account" element={<AccountPage />} />
        </Routes>
      </Router>
    </>
  );
}

export default App;

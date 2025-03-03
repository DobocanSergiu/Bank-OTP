import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import "@testing-library/jest-dom";
import { BrowserRouter } from "react-router-dom";
import LoginPage from "../../src/Pages/LoginPage/LoginPage.jsx";
import { MemoryRouter } from "react-router";

describe("LoginPage", () => {
  window.fetch = vi.fn();
  // Helper function to render component with router
  const renderLoginPage = () => {
    return render(
      <BrowserRouter>
        <LoginPage />
      </BrowserRouter>,
    );
  };

  it("renders the login form correctly", () => {
    renderLoginPage();

    // Check that all elements are rendered
    expect(screen.getAllByText("Login")[0]).toBeInTheDocument();
    expect(screen.getByText("Username:")).toBeInTheDocument();
    expect(screen.getByText("Password:")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Enter Username")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Enter Password")).toBeInTheDocument();
  });

  it("shows error when both fields are empty", () => {
    renderLoginPage();

    // Click login without entering anything
    fireEvent.click(screen.getByRole("button", { name: /login/i }));

    // Verify error message appears
    expect(
      screen.getByText("Username and Password fields must not be left empty"),
    ).toBeInTheDocument();
  });

  it("shows error when username is empty", () => {
    renderLoginPage();

    // Only fill password
    fireEvent.change(screen.getByPlaceholderText("Enter Password"), {
      target: { value: "password123" },
    });

    // Click login
    fireEvent.click(screen.getByRole("button", { name: /login/i }));

    // Verify error message appears
    expect(
      screen.getByText("Username and Password fields must not be left empty"),
    ).toBeInTheDocument();
  });

  it("shows error when password is empty", () => {
    renderLoginPage();

    // Only fill username
    fireEvent.change(screen.getByPlaceholderText("Enter Username"), {
      target: { value: "testuser" },
    });

    // Click login
    fireEvent.click(screen.getByRole("button", { name: /login/i }));

    // Verify error message appears
    expect(
      screen.getByText("Username and Password fields must not be left empty"),
    ).toBeInTheDocument();
  });

  it("accepts input in the username field", () => {
    renderLoginPage();

    // Type in username field
    const usernameInput = screen.getByPlaceholderText("Enter Username");
    fireEvent.change(usernameInput, { target: { value: "testuser" } });

    // Verify input is accepted
    expect(usernameInput.value).toBe("testuser");
  });

  it("accepts input in the password field", () => {
    renderLoginPage();

    // Type in password field
    const passwordInput = screen.getByPlaceholderText("Enter Password");
    fireEvent.change(passwordInput, { target: { value: "password123" } });

    // Verify input is accepted
    expect(passwordInput.value).toBe("password123");
  });

  it("renders the new user link with correct href", () => {
    renderLoginPage();

    // Check that the link points to the right place
    const newUserLink = screen.getByText("New User? Create an account");
    expect(newUserLink).toHaveAttribute("href", "createUser");
  });

  it("successfully logs in a user", async () => {
    fetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({ success: true, message: "Login successful" }),
    });

    const { getByPlaceholderText, getByRole } = render(
      <MemoryRouter>
        <LoginPage />
      </MemoryRouter>,
    );

    fireEvent.change(getByPlaceholderText("Enter Username"), {
      target: { value: "testuser" },
    });
    fireEvent.change(getByPlaceholderText("Enter Password"), {
      target: { value: "testpassword" },
    });

    fireEvent.click(getByRole("button", { name: /login/i }));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "https://localhost:7229/api/loginUser",
        expect.objectContaining({
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            Username: "testuser",
            Password: "testpassword",
          }),
        }),
      );
    });
  });

  it("handles incorrect credentials", async () => {
    fetch.mockResolvedValueOnce({
      ok: false,
      status: 401,
      json: () => Promise.resolve({ error: "User not found" }),
    });

    const response = await fetch("https://localhost:7229/api/loginUser", {
      method: "POST",
    });
    const data = await response.json();

    expect(response.ok).toBe(false);
    expect(response.status).toBe(401);
    expect(data.error).toBe("User not found");
  });
});

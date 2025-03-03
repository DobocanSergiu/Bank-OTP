import { describe, it, expect, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import { BrowserRouter } from "react-router-dom";
import CreateUserPage from "../../src/Pages/CreateUserPage/CreateUserPage.jsx"; // Make sure the path is correct

describe("CreateUserPage", () => {
  const renderUserPage = () => {
    return render(
      <BrowserRouter>
        <CreateUserPage />
      </BrowserRouter>,
    );
  };

  window.fetch = vi.fn();
  it("should render user page succesfully", () => {
    renderUserPage();
    expect(screen.getByText("Create User:")).toBeInTheDocument();
    expect(screen.getByText("Username:")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Enter Username")).toBeInTheDocument();
    expect(screen.getByText("Password:")).toBeInTheDocument();
    expect(
      screen.getAllByPlaceholderText("Enter Password")[0],
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Login/i })).toBeInTheDocument();
  });

  it("should show error for empty fields", () => {
    renderUserPage();
    var loginButton = screen.getByRole("button", { name: /Login/i });
    fireEvent.click(loginButton);
    expect(
      screen.getByText("Username and Password fields must not be left empty"),
    ).toBeInTheDocument();
  });

  it("should show error for different passwords", () => {
    renderUserPage();
    var loginButton = screen.getByRole("button", { name: /Login/i });
    var usernameField = screen.getByPlaceholderText("Enter Username");
    var passwordField = screen.getAllByPlaceholderText("Enter Password")[0];
    var reEnterPasswordField =
      screen.getAllByPlaceholderText("Enter Password")[1];

    fireEvent.change(usernameField, { target: { value: "testuser" } });
    fireEvent.change(passwordField, { target: { value: "password1$2@!3123" } });
    fireEvent.change(reEnterPasswordField, {
      target: { value: "Password332322@!3" },
    });
    fireEvent.click(loginButton);
    expect(screen.getByText("Passwords must match")).toBeInTheDocument();
  });

  it("should show an error for simple passwords", () => {
    renderUserPage();
    var loginButton = screen.getByRole("button", { name: /Login/i });
    var usernameField = screen.getByPlaceholderText("Enter Username");
    var passwordField = screen.getAllByPlaceholderText("Enter Password")[0];
    var reEnterPasswordField =
      screen.getAllByPlaceholderText("Enter Password")[1];

    fireEvent.change(usernameField, { target: { value: "testuser" } });
    fireEvent.change(passwordField, { target: { value: "123123" } });
    fireEvent.change(reEnterPasswordField, { target: { value: "123123" } });
    fireEvent.click(loginButton);
    expect(
      screen.getByText("Password must meet the following requirements:"),
    ).toBeInTheDocument();
    expect(screen.getByText("At least 8 characters long")).toBeInTheDocument();
    expect(screen.getByText("Contain at least one number")).toBeInTheDocument();
    expect(
      screen.getByText("Contain at least one uppercase letter"),
    ).toBeInTheDocument();
  });

  async function createUser(username, password) {
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

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.error || "Unknown error");
    }

    return await response.json();
  }

  it("successfully creates a new user", async () => {
    // Mock a successful response
    fetch.mockResolvedValue({
      ok: true,
      json: () =>
        Promise.resolve({
          success: true,
          message: "User created successfully",
        }),
    });

    // Call your function with test data
    const result = await createUser("newuser", "Complex123!");

    // Verify fetch was called with correct data
    expect(fetch).toHaveBeenCalledWith(
      "https://localhost:7229/api/createUser/",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Username: "newuser",
          Password: "Complex123!",
        }),
      },
    );

    // Check result
    expect(result.success).toBe(true);
    expect(result.message).toBe("User created successfully");
  });

  it("handles username already exists error", async () => {
    fetch.mockResolvedValueOnce({
      ok: false,
      status: 409,
      json: () =>
        Promise.resolve({
          error: "Username already exists",
        }),
    });

    try {
      await createUser("existinguser", "Complex123!");
      expect(true).toBe(false); // This line should not execute
    } catch (error) {
      expect(error.message).toBe("Username already exists");
    }
  });

  it("handles simple password error", async () => {
    fetch.mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: () =>
        Promise.resolve({
          error: "Password is too simple",
        }),
    });

    try {
      await createUser("newuser", "simple");
      // If we get here, the test should fail
      expect(true).toBe(false); // This line should not execute
    } catch (error) {
      // Verify error message
      expect(error.message).toBe("Password is too simple");
    }
  });
});

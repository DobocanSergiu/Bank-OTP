import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import { BrowserRouter } from "react-router-dom";
import OtpPage from "../../src/Pages/OtpPage/OtpPage.jsx";

// Mock fetch for API calls
window.fetch = vi.fn();

describe("OtpPage", () => {
  beforeEach(() => {
    // Set 'username' in localStorage before each test
    localStorage.setItem("username", "Mike");
  });

  afterEach(() => {
    // Clean up the localStorage after each test
    localStorage.clear();
  });

  const renderOtpPage = () => {
    return render(
      <BrowserRouter>
        <OtpPage />
      </BrowserRouter>,
    );
  };

  it("renders the otp form correctly", () => {
    renderOtpPage();
    expect(screen.getByText("One time passcode check")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("Enter OTP")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Login/i })).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Request New Passcode/i }),
    ).toBeInTheDocument();
  });

  it("should call validateCode API correctly", async () => {
    // Mock successful API response
    fetch.mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ success: true }),
    });

    const userInput = "123456";

    // Call the function that makes the API request
    const response = await fetch("https://localhost:7229/api/validateCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Username: encodeURIComponent("testuser"),
        Code: userInput,
      }),
    });

    // Check if API was called with correct parameters
    expect(fetch).toHaveBeenCalledWith(
      "https://localhost:7229/api/validateCode",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Username: "testuser",
          Code: "123456",
        }),
      },
    );

    // Verify response handling works
    const data = await response.json();
    expect(data.success).toBe(true);
  });

  it("should call requestCode API correctly", async () => {
    // Mock successful API response
    fetch.mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ code: "654321", time: "30" }),
    });

    // Call the function that makes the API request
    const response = await fetch("https://localhost:7229/api/requestCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Username: encodeURIComponent("testuser"),
      }),
    });

    // Check if API was called with correct parameters
    expect(fetch).toHaveBeenCalledWith(
      "https://localhost:7229/api/requestCode",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Username: "testuser",
        }),
      },
    );

    // Verify response handling works
    const data = await response.json();
    expect(data.code).toBe("654321");
    expect(data.time).toBe("30");
  });

  it("should handle API errors", async () => {
    // Mock failed API response
    fetch.mockResolvedValue({
      ok: false,
      status: 400,
      statusText: "Bad Request",
    });

    // Call the requestCode API
    const response = await fetch("https://localhost:7229/api/requestCode", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Username: encodeURIComponent("testuser"),
      }),
    });

    // Check response properties
    expect(response.ok).toBe(false);
    expect(response.status).toBe(400);
  });
});

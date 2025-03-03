import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import "@testing-library/jest-dom";
import { BrowserRouter } from "react-router-dom";
import AccountPage from "../../src/Pages/AccountPage/AccountPage.jsx"; // Make sure the path is correct

describe("AccountPage", () => {
  it("should render AccountPage with login success message", () => {
    render(
      <BrowserRouter>
        <AccountPage />
      </BrowserRouter>,
    );

    const header = screen.getByText(/Account login successful/i);
    expect(header).toBeInTheDocument();
  });
});

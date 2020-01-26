import React from "react";
import {
  render,
  fireEvent,
  waitForElement,
  getByText
} from "@testing-library/react";
import App from "./App";

describe("App", () => {
  describe("when click on convert button after input", () => {
    it("should all api to convert", async () => {
      const { getByTestId, getByText } = render(<App />);
      const inputBox = getByTestId("input");
      fireEvent.change(inputBox, { target: { value: "【宣告/Proclaim】" } });
      const submitButton = getByTestId("submit");
      submitButton.click();
      const [h1, h2] = await waitForElement(() => [
        getByText("Proclaim"),
        getByText("宣告")
      ]);
      expect(h1.tagName).toBe("H1");
      expect(h2.tagName).toBe("H2");
    });
  });
});

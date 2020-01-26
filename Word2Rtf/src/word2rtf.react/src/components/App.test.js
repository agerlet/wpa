import React from "react";
import {
  render,
  fireEvent,
  waitForElement,
  findByText
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
      const [h1, h2, error] = await waitForElement(() => [
        getByText("Proclaim"),
        getByText("宣告"),
        getByTestId("error")
      ]);
      expect(h1.tagName).toBe("H1");
      expect(h2.tagName).toBe("H2");
      expect(error).toBeEmpty();
    });

    it("should render api error", async () => {
      const { getByTestId, getByText } = render(
        <App />
      );
      const inputBox = getByTestId("input");
      fireEvent.change(inputBox, {
        target: {
          value:
            "【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。"
        }
      });
      const submitButton = getByTestId("submit");
      submitButton.click();
      const [outputElement, errorElement] = await waitForElement(() => [
        getByTestId('output'),
        getByText(/中英文沒有成對/)
      ]);
      expect(outputElement).toBeEmpty();
      expect(errorElement).toHaveTextContent(/Generous Offering/);
    });
  });
});

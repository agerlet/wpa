import React from "react";
import {
  render,
  fireEvent,
  waitForElement
} from "@testing-library/react";
import App from "./App";

describe("App", () => {
  describe("when click on convert button after input", () => {
    it("should call api to convert", async () => {
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

    it("should render api error", async () => {
      const { getByTestId } = render(
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
      const [outputElement] = await waitForElement(() => [
        getByTestId("0")
      ]);
        expect(outputElement.outerHTML).toBe(
          '<h1 data-testid="0" data-error="中英文沒有成對: &quot;【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。&quot;">【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。</h1>'
        );
    });
  });
});

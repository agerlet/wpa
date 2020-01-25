import React from "react";
import { render, fireEvent } from "@testing-library/react";
import App from "./App";

describe("App", () => {
  describe("when click on convert button after input", () => {
    it("should copy input over", () => {
      const { getByTestId } = render(<App />)
      const inputBox = getByTestId('input')
      fireEvent.change(inputBox, { target: { value: 'test' } })
      const submitButton = getByTestId('submit')
      submitButton.click()
      const outputBox = getByTestId('output')
      expect(outputBox.textContent).toBe('test')
    })
  })
})

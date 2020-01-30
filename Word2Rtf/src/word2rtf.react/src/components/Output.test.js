import React from "react";
import { render } from "@testing-library/react";
import "jest-styled-components";
import { Output } from "./Output";

describe("Output", () => {
  it("should render the output div", () => {
    const { getByTestId } = render(<Output />);
    const outputElement = getByTestId("output");
    expect(outputElement).toBeInTheDocument();
  });

  it("should be empty at the beginning", () => {
    const { getByTestId } = render(<Output />);
    const outputElement = getByTestId("output");
    expect(outputElement.textContent).toBe("");
  });

  it("should be the same as props.output", () => {
    const program = [{ verses: [{ Language: 0, Content: "test" }] }];
    const { getByTestId } = render(<Output program={program} />);
    const outputElement = getByTestId("output");
    expect(outputElement.textContent).toBe("test");
  });

  it("should render \\n into multiple lines", () => {
    const program = [{ verses: [{ Language: 0, Content: "test\ntest" }] }];
    const { getByTestId } = render(<Output program={program} />);
    const outputElement = getByTestId("output");
    expect(outputElement.innerHTML).toBe(
      '<h2 data-testid="0">test<br>test</h2>'
    );
  });

  it("should render error message with origin input", () => {
    const error = {
      errorType: "ImbalancedLanguagesException",
      errorMessage:
        '中英文沒有成對: "【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。"',
      stackTrace: [
        "at Word2Rtf.StringExtensions.FilterByLanguages(String input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/StringExtensions.cs:line 150",
        "at Word2Rtf.Parsers.TitleParser.Parse(Element input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/TitleParser.cs:line 19",
        "at Word2Rtf.Parsers.ParserHandler.ParseTitle(Element title) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 58",
        "at Word2Rtf.Parsers.BracketParser.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/BracketParser.cs:line 30",
        "at Word2Rtf.Parsers.ParserHandler.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 42",
        "at Word2Rtf.Function.FunctionHandler(Payload input, ILambdaContext context) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Function.cs:line 49",
        "at lambda_method(Closure , Stream , Stream , LambdaContextInternal )"
      ]
    };
    const program = [{ 
      verses: [{
        Language: 1,
        Content: "【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。",
        Error: error
      }] 
    }];
    const { getByText } = render(<Output program={program} />);
    const output = getByText(/Generous Offering/);
    expect(output.outerHTML).toBe(
      '<h1 data-testid="0" data-error="中英文沒有成對: &quot;【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。&quot;">【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。</h1>'
    );
  });

  describe("should support click & copy to clipboard", () => {
    it("select the whole output when click the output", () => {
      const eventHandler = jest.fn();
      const { getByTestId } = render(
        <Output
          program={[{ verses: [{ Language: 0, Content: "test\ntest" }] }]}
          clipBoard={eventHandler}
        />
      );
      const outputElement = getByTestId("output");
      outputElement.click();
      expect(eventHandler).toHaveBeenCalledTimes(1);
    });

    it("should display copiedToClipboard message", () => {
      let copiedToClipboard = false;
      const eventHandler = jest.fn(() => {
        copiedToClipboard = true;
      });
      const { getByTestId } = render(
        <Output
          program={[{ verses: [{ Language: 0, Content: "test\ntest" }] }]}
          clipBoard={eventHandler}
          copiedToClipboard={copiedToClipboard}
        />
      );
      const outputElement = getByTestId("output");
      outputElement.click();
      expect(copiedToClipboard).toBe(true);
    });

    it("should display copiedToClipboard message", () => {
      const { getByTestId } = render(
        <Output
          program={[{ verses: [{ Language: 0, Content: "test\ntest" }] }]}
          copiedToClipboard={true}
        />
      );
      const ctcElement = getByTestId("copiedToClipboard");
      expect(ctcElement).toBeVisible();
    });

    it("copiedToClipboard message should be displayed with style", () => {
      const { getByTestId } = render(
        <Output
          program={[{ verses: [{ Language: 0, Content: "test\ntest" }] }]}
          copiedToClipboard={true}
        />
      );
      const ctcElement = getByTestId("copiedToClipboard");
      expect(ctcElement).toHaveStyleRule({
        background: "rgba(51, 153, 51, 0.91)",
        color: "#fff"
      });
    });
  });
});

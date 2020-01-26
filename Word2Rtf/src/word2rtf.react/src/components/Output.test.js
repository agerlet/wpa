import React from 'react'
import { render } from '@testing-library/react'
import { Output } from './Output'

describe('Output should', () => {
    it('render the output div', () => {
        const { getByTestId } = render(<Output />)
        const outputElement = getByTestId("output");
        expect(outputElement).toBeInTheDocument();
    })

    it('be empty at the beginning', () => {
        const { getByTestId } = render(<Output />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('')
    })

    it('be the same as props.output', () => {
        const { getByTestId } = render(<Output program={[ {verses: [{Language:0, Content: 'test'}]} ]} />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('test')
    })

    it("render \\n into multiple lines", () => {
      const { getByTestId } = render(
        <Output program={[ {verses: [{ Language: 0, Content: "test\ntest" }]} ]} />
      );
      const outputElement = getByTestId("output");
      expect(outputElement.innerHTML).toBe("<h2>test<br>test</h2>");
    });

    it("render error message", () => {
      const error = {
  "errorType": "ImbalancedLanguagesException",
  "errorMessage": "中英文沒有成對: \"【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。\"",
  "stackTrace": [
    "at Word2Rtf.StringExtensions.FilterByLanguages(String input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/StringExtensions.cs:line 150",
    "at Word2Rtf.Parsers.TitleParser.Parse(Element input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/TitleParser.cs:line 19",
    "at Word2Rtf.Parsers.ParserHandler.ParseTitle(Element title) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 58",
    "at Word2Rtf.Parsers.BracketParser.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/BracketParser.cs:line 30",
    "at Word2Rtf.Parsers.ParserHandler.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 42",
    "at Word2Rtf.Function.FunctionHandler(Payload input, ILambdaContext context) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Function.cs:line 49",
    "at lambda_method(Closure , Stream , Stream , LambdaContextInternal )"
  ]
};
      const { getByTestId } = render(
        <Output error={error} />
      );
      const errorElement = getByTestId("error");
      expect(errorElement).toBeInTheDocument();
    });
})
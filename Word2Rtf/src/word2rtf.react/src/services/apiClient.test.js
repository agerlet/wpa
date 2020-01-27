import { apiClient } from "./apiClient";
import { cleanup } from "@testing-library/react";

describe("apiClient", () => {
  afterEach(cleanup);

  it("should be empty object once the input is empty", async () => {
    const emptyResult = await apiClient();
    expect(emptyResult).toBe(null);
  });

  it("should return json object", async () => {
    const program = await apiClient("【宣告/Proclaim】《詩篇/Psalm 50:23》");
    const { verses } = program[0];
    expect(verses.length).toBeGreaterThan(0);
  });

  describe("Error handling", () => {
    it("throw error from the Api Gateway", async () => {
      expect.assertions(1);
      try {
        await apiClient(
          "【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。"
        );
       } catch (e) {
         expect(e.errorType).toBe("ImbalancedLanguagesException");
        }
      });
  });
});

/*
{
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
}
 */

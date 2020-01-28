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


//{
//         data: {
//           errorType: 'ImbalancedLanguagesException',
//           errorMessage: '中英文沒有成對: "【Generous Offering慷
// 慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。"',
//           stackTrace: [
//             'at Word2Rtf.StringExtensions.FilterByLanguages(String input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/StringExtensions.cs:line 149',
//             'at Word2Rtf.Parsers.TitleParser.Parse(Element input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/TitleParser.cs:line 19',
//             'at Word2Rtf.Parsers.ParserHandler.ParseTitle(Element title) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 58',
//             'at Word2Rtf.Parsers.BracketParser.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/BracketParser.cs:line 32',
//             'at Word2Rtf.Parsers.ParserHandler.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 42',
//             'at Word2Rtf.Function.FunctionHandler(Payload input, ILambdaContext context) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Function.cs:line 49',
//             'at lambda_method(Closure , Stream , Stream , LambdaContextInternal )'
//           ]
//         },
//         status: 200,
//         statusText: 'OK',
//         headers: { 'content-type': 'application/json' },
//         config: {
//           url: 'https://1db54ls3q1.execute-api.ap-southeast-2.amazonaws.com/Stage/Word2Rtf',
//           method: 'post',
//           data: '{"Input":"【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。"}',
//           headers: {
//             Accept: 'application/json, text/plain, */*',
//             'Content-Type': 'application/json;charset=utf-8'
//           },
//           transformRequest: [ [Function: transformRequest] ],
//           transformResponse: [ [Function: transformResponse] ],
//           timeout: 0,
//           adapter: [Function: xhrAdapter],
//           xsrfCookieName: 'XSRF-TOKEN',
//           xsrfHeaderName: 'X-XSRF-TOKEN',
//           maxContentLength: -1,
//           validateStatus: [Function: validateStatus]
//         },
//         request: XMLHttpRequest {
//           upload: XMLHttpRequestUpload { _ownerDocument: [Document] },
//           _registeredHandlers: Set(1) { 'readystatechange' },
//           _eventHandlers: [Object: null prototype] {
//             readystatechange: [Function: handleLoad]
//           },
//           [Symbol(flag)]: {
//             synchronous: false,
//             withCredentials: false,
//             mimeType: null,
//             auth: null,
//             method: 'POST',
//             responseType: '',
//             requestHeaders: [Object],
//             referrer: 'http://localhost/',
//             uri: 'https://1db54ls3q1.execute-api.ap-southeast-2.amazonaws.com/Stage/Word2Rtf',
//             timeout: 0,
//             body: undefined,
//             formData: false,
//             preflight: false,
//             requestManager: [RequestManager],
//             pool: undefined,
//             agentOptions: undefined,
//             strictSSL: undefined,
//             proxy: undefined,
//             cookieJar: [CookieJar],
//             encoding: 'UTF-8',
//             origin: 'http://localhost',
//             userAgent: 'Mozilla/5.0 (darwin) AppleWebKit/537.36 (KHTML, like Gecko) jsdom/11.12.0'
//           },
//           [Symbol(properties)]: {
//             beforeSend: false,
//             send: true,
//             timeoutStart: 0,
//             timeoutId: 0,
//             timeoutFn: null,
//             client: null,
//             responseHeaders: [Object],
//             filteredResponseHeaders: [Array],
//             responseBuffer: <Buffer 7b 0a 20 20 22 65 72 72 6f 72 54 79 70 65 22 3a 20 22 49 6d 62 61 6c 61 6e 63 65 64 4c 61 6e 67 75 61 67 65 73 45 78 63 65 70 74 69 6f 6e 22 2c 0a 20 ... 1048526 more bytes>,
//             responseCache: null,
//             responseTextCache: '{\n' +
//               '  "errorType": "ImbalancedLanguagesException",\n' +
//               '  "errorMessage": "中英文沒有成對: \\"【Generous Offering慷慨奉獻】奉獻是對神愛的回應，如不明白奉獻的意義,無需參與。\\"",\n' +
//               '  "stackTrace": [\n' +
//               '    "at Word2Rtf.StringExtensions.FilterByLanguages(String input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/StringExtensions.cs:line 149",\n' +
//               '    "at Word2Rtf.Parsers.TitleParser.Parse(Element input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/TitleParser.cs:line 19",\n' +
//               '    "at Word2Rtf.Parsers.ParserHandler.ParseTitle(Element title) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 58",\n' +
//               '    "at Word2Rtf.Parsers.BracketParser.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/BracketParser.cs:line 32",\n' +
//               '    "at Word2Rtf.Parsers.ParserHandler.Parse(String[] input) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Parsers/ParserHandler.cs:line 42",\n' +
//               '    "at Word2Rtf.Function.FunctionHandler(Payload input, ILambdaContext context) in /Users/xiaomingfeng/OneDrive/TSA/worship-program-assistance/Word2Rtf/src/Word2Rtf/Function.cs:line 49",\n' +
//               '    "at lambda_method(Closure , Stream , Stream , LambdaContextInternal )"\n' +
//               '  ]\n' +
//               '}\n',
//             responseXMLCache: null,
//             responseURL: 'https://1db54ls3q1.execute-api.ap-southeast-2.amazonaws.com/Stage/Word2Rtf',
//             readyState: 4,
//             status: 200,
//             statusText: 'OK',
//             error: '',
//             uploadComplete: true,
//             uploadListener: false,
//             abortError: false,
//             cookieJar: [CookieJar],
//             bufferStepSize: 1048576,
//             totalReceivedChunkSize: 1398,
//             requestBuffer: null,
//             requestCache: null,
//             origin: 'http://localhost'
//           }
//         }
//       }


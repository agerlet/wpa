import React from "react";
import styled from "styled-components";

const CopiedToClipboard = styled.span`
  background: rgba(51, 153, 51, 0.91);
  color: #fff;
`;

export class Output extends React.Component {

  render() {
    const {
      clipBoard,
      copiedToClipboard,
      isProcessing,
      programs
    } = this.props;
    
    console.log(programs)

    return (
      <>
        <label htmlFor="output">
          點擊轉換後，把結果複製粘貼到Word，並保存為RTF文件。
          {copiedToClipboard && (
            <CopiedToClipboard data-testid="copiedToClipboard">
              已複製，請粘貼到Word（大綱模式）。
            </CopiedToClipboard>
          )}
        </label>
        <div id="output" data-testid="output" onClick={clipBoard}>
          {programs &&
            programs.length > 0 &&
            programs.map(
            program => 
              program &&
              program.length > 0 &&
              program.map(
                (_, index) =>
                  _.verses &&
                  _.verses.length > 0 &&
                  _.verses.map((verse, i) => {
                      const convertLineBreak = line => {
                          const fragments = line.split("\n");
                          return fragments.map((_, j) => {
                              if (j === fragments.length - 1) {
                                  return (
                                      <React.Fragment key={`${i}-${j}`}>{_}</React.Fragment>
                                  );
                              }
                              return (
                                  <React.Fragment key={`${i}-${j}`}>
                                      {_}
                                      <br />
                                  </React.Fragment>
                              );
                          });
                      };
                      return (
                          (verse.Language === 1 && (
                              <h1
                                  key={i}
                                  data-testid={i}
                                  data-error={verse.Error && verse.Error.errorMessage}
                              >
                                  {convertLineBreak(verse.Content)}
                              </h1>
                          )) ||
                          (verse.Language === 0 && (
                              <h2
                                  key={i}
                                  data-testid={i}
                                  data-error={verse.Error && verse.Error.errorMessage}
                              >
                                  {convertLineBreak(verse.Content)}
                              </h2>
                          )) ||
                          (verse.language === 1 && (
                              <h1
                                  key={i}
                                  data-testid={i}
                                  data-error={verse.error && verse.error.errorMessage}
                              >
                                  {convertLineBreak(verse.content)}
                              </h1>
                          )) ||
                          (verse.language === 0 && (
                              <h2
                                  key={i}
                                  data-testid={i}
                                  data-error={verse.error && verse.error.errorMessage}
                              >
                                  {convertLineBreak(verse.content)}
                              </h2>
                          ))
                      );
                  })
              )
            )
          }
        </div>
        {isProcessing && <div>Processing</div>}{" "}
      </>
    );
  }
}

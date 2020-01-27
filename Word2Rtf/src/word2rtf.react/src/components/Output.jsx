import React from "react";
import styled from "styled-components";

const CopiedToClipboard = styled.span`
  background: #339933;
  color: #fff;
`;

export class Output extends React.Component {
  render() {
    const {
      clipBoard,
      copiedToClipboard,
      error,
      isProcessing,
      program
    } = this.props;
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
          {program &&
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
                      <h1 key={i}>{convertLineBreak(verse.Content)}</h1>
                    )) ||
                    (verse.Language === 0 && (
                      <h2 key={i}>{convertLineBreak(verse.Content)}</h2>
                    ))
                  );
                })
            )}
        </div>
        {error && (
          <div className="error" data-testid="error">
            {error.errorMessage}
          </div>
        )}
        {isProcessing && <div>Processing</div>}{" "}
      </>
    );
  }
}

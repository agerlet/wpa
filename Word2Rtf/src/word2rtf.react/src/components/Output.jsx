import React from "react";

export const Output = props => (
  <>
    <label htmlFor="output">
      點擊轉換後，把結果複製粘貼到Word，並保存為RTF文件。
    </label>
    <div id="output" data-testid="output">
      {props.program &&
        props.program.length > 0 &&
        props.program.map(
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
    {props.error && (
      <div className="error" data-testid="error">
        {props.error.errorMessage}
      </div>
    )}
    {props.isProcessing && <div>Processing</div>}{" "}
  </>
);

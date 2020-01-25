import React from 'react'

export const Output = (props) => 
    <>
        <label htmlFor="output">
            點擊轉換後，把結果複製粘貼到Word，並保存為RTF文件。
        </label>
        <div id="output" data-testid="output">
            {props.output}
        </div>
    </>
    
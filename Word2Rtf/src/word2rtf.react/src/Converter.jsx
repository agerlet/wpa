import React from 'react'

export class Converter extends React.Component {
    render () {
        return (
            <>
                <div>
                    <textarea name="content" id="content"></textarea>
                    <input type="submit" value="轉換" id="submit" />
                </div>
                <div>
                    <label htmlFor="output">點擊轉換後，把結果複製粘貼到Word，並保存為RTF文件。</label>
                    <div id="output" data-testid="output"></div>
                </div>
            </>
        )
    }
}
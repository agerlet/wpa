import React from "react";
import "../App.css";
import { Instruction } from "./Instruction";
import { Input } from "./Input";
import { Output } from "./Output";
import { apiClient } from "../Services/apiClient";

class App extends React.Component {
  state = {
    input: "",
    program: [],
    copiedToClipboard: false
  };

  convertHandler = async () => {
    const inputs = this.state.input.split("\n【");
    const getQuery = q => q.startsWith("【") ? q : `【${q}`;
    const programs = await Promise.all(
      inputs.map(async _ => {
        try {
          return await apiClient(getQuery(_));
        } catch (ex) {
          return [
            {
              verses: [
                {
                  Language: 1,
                  Content: getQuery(_),
                  Error: ex
                }
              ]
            }
          ];
        }
      })
    );
    this.setState({
      programs: programs,
      copiedToClipboard: false
    });
  };

  textareOnChange = e => {
    this.setState({ input: e.target.value });
  };

  clipBoard = e => {
    if (JSON.stringify(this.state.output) === JSON.stringify({})) return;

    let range = document.createRange();
    range.selectNode(e.currentTarget);
    const selection = document.getSelection();
    selection.removeAllRanges();
    selection.addRange(range);
    document.execCommand("copy");
    this.setState({ copiedToClipboard: true });
  };

  render() {
    return (
      <>
        <Instruction />
        <Input
          input={this.state.input}
          convertHandler={this.convertHandler}
          textareOnChange={this.textareOnChange}
        />
        <Output
          programs={this.state.programs}
          clipBoard={this.clipBoard}
          copiedToClipboard={this.state.copiedToClipboard}
        />
      </>
    );
  }
}

export default App;

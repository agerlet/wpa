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
    errors: [],
    copiedToClipboard: false
  };

  convertHandler = async () => {
    let program = [];
    let errors = [];
    const inputs = this.state.input.split("\n【");
    await Promise.all(inputs.map(async _ => {
      try {
        const result = await apiClient(`【${_}`);
        program = program.concat(result);
      } catch (ex) {
        errors.push(ex);
      }
    }));
    this.setState({
      program: program,
      errors: errors,
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
          program={this.state.program}
          errors={this.state.errors}
          clipBoard={this.clipBoard}
          copiedToClipboard={this.state.copiedToClipboard}
        />
      </>
    );
  }
}

export default App;

import React from "react";
import "../App.css";
import { Instruction } from "./Instruction";
import { Input } from "./Input";
import { Output } from "./Output";
import { apiClient } from "../Services/apiClient";

class App extends React.Component {
  state = {
    input: "",
    output: {},
    error: {},
    copiedToClipboard: false
  };

  convertHandler = async () => {
    try {
      var program = await apiClient(this.state.input);
      this.setState({
        output: program,
        error: {},
        copiedToClipboard: false
      });
    } catch (ex) {
      this.setState({
        output: {},
        error: ex,
        copiedToClipboard: false
      });
    }
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
    this.setState({copiedToClipboard: true});
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
          program={this.state.output}
          error={this.state.error}
          clipBoard={this.clipBoard}
          copiedToClipboard={this.state.copiedToClipboard}
        />
      </>
    );
  }
}

export default App;

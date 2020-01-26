import React from "react";
import "../App.css";
import { Instruction } from "./Instruction";
import { Input } from "./Input";
import { Output } from "./Output";
import { apiClient } from "../services/apiClient";

class App extends React.Component {
  state = {
    input: "",
    output: {},
    error: {}
  };

  convertHandler = async () => {
    try {
      var program = await apiClient(this.state.input);
      this.setState({
        output: program,
        error: {}
      });
    } catch (ex) {
      this.setState({
        output: {},
        error: ex
      });
    }
  };

  textareOnChange = e => {
    this.setState({ input: e.target.value });
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
        <Output program={this.state.output} error={this.state.error} />
      </>
    );
  }
}

export default App;

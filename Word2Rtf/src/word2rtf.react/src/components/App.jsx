import React from "react";
import "../App.css";
import { Instruction } from "./Instruction";
import { Input } from "./Input";
import { Output } from "./Output";
import { apiClient } from "../services/apiClient";

class App extends React.Component {
  state = {
    input: "",
    output: {}
  };

  convertHandler = async () => {
    var program = await apiClient(this.state.input);
      this.setState({
        output: program
      });
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
        <Output program={this.state.output} />
      </>
    );
  }
}

export default App;

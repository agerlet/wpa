import React from 'react'
import '../App.css'
import { Instruction } from './Instruction'
import { Input } from './Input'
import { Output } from './Output'

class App extends React.Component {
  state = {
      input: '',
      output: ''
  }

  convertHandler = () => {
    this.setState({ output: this.state.input })
  }

  textareOnChange = e => {
    this.setState({input: e.target.value})
  }

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
          output={this.state.output} 
        />
      </>
    )
  }
}

export default App;

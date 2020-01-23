import React from 'react'
import './App.css'
import { Instruction } from './Instruction'
import { Converter } from './Converter'

class App extends React.Component {
  render() {
    return (
      <div className="App">
        <Instruction />
        <Converter />
      </div>
    )
  }
}

export default App;

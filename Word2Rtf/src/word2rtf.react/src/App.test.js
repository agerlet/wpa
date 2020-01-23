import React from 'react';
import { render } from '@testing-library/react';
import App from './App';

describe('renders the elements', () => {

  it('renders the instructions', () => {
    const { getByText } = render(<App />)
    const useElement = getByText(/使用方法/i)
    expect(useElement).toBeInTheDocument() 
  })

  it('renders the convert button', () => {
    const { getByDisplayValue } = render(<App />)
    const buttonElement = getByDisplayValue(/轉換/i)
    expect(buttonElement).toBeInTheDocument()
  })

  it('renders the output div', () => {
    const { getByTestId } = render(<App />)
    const divElement = getByTestId('output')
    expect(divElement).toBeInTheDocument()
  })
})

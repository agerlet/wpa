import React from 'react'
import { render } from '@testing-library/react'
import { Output } from './Output'

describe('Output should', () => {
    it('render the output div', () => {
        const { getByTestId } = render(<Output />)
        const outputElement = getByTestId("output");
        expect(outputElement).toBeInTheDocument();
    })

    it('be empty at the beginning', () => {
        const { getByTestId } = render(<Output />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('')
    })

    it('be the same as props.output', () => {
        const { getByTestId } = render(<Output program={[ {verses: [{Language:0, Content: 'test'}]} ]} />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('test')
    })

    it("render \\n into multiple lines", () => {
      const { getByTestId } = render(
        <Output program={[ {verses: [{ Language: 0, Content: "test\ntest" }]} ]} />
      );
      const outputElement = getByTestId("output");
      expect(outputElement.innerHTML).toBe("<h2>test<br>test</h2>");
    });
})
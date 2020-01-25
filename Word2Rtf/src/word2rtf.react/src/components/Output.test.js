import React from 'react'
import { render } from '@testing-library/react'
import { Output } from './Output'

describe('Output should', () => {
    it('render the output div', () => {
        const { getByTestId } = render(<Output />)
        const divElement = getByTestId('output')
        expect(divElement).toBeInTheDocument()
    })

    it('be empty at the beginning', () => {
        const { getByTestId } = render(<Output />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('')
    })

    it('be the same as props.output', () => {
        const { getByTestId } = render(<Output output={'test'} />)
        const outputElement = getByTestId('output')
        expect(outputElement.textContent).toBe('test')
    })
})
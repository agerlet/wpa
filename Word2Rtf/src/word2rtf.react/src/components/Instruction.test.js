import React from './node_modules/react'
import { render } from './node_modules/@testing-library/react'
import { Instruction } from './Instruction'

describe('Instruction', () => {
    it('renders the instructions', () => {
        const { getByText } = render(<Instruction />)
        const useElement = getByText(/使用方法/i)
        expect(useElement).toBeInTheDocument()
    })
})
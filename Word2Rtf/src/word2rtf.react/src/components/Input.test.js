import React from 'react'
import { render } from '@testing-library/react'
import { Input } from './Input'

describe('The converter should', () => {
    it('renders the convert button', () => {
        const { getByDisplayValue } = render(<Input />)
        const buttonElement = getByDisplayValue(/轉換/i)
        expect(buttonElement).toBeInTheDocument()
    })

    it('fires click event', () => {
        const eventHandler = jest.fn();
        const { getByTestId } = render(<Input convertHandler={eventHandler} />)
        const button = getByTestId('submit')
        button.click()
        expect(eventHandler).toBeCalled()
    })
})
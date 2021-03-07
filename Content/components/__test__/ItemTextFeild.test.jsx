import React from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import '@testing-library/jest-dom'

import ItemTextFeild from '../items/ItemTextFeild.jsx'

const setup = (itemId, Label, defaultName, disabled) => {
    const { getByText } =
        render(<ItemTextFeild id={itemId} label={Label} defVal={defaultName} dbl={disabled} />)

    return {
        getByText,
    }
}

test("renders without crashing", () => {
    const { getByText } = setup("TestId", "TestLabel", "DefaultValue", true)

    getByText('TestLabel')
})

test("text input changes value", () => {
    setup("TestId", "TestLabel", "DefaultValue", false)

    userEvent.type(screen.getByLabelText(/TestLabel/i), '{selectall}{del}new value')

    expect(screen.getByLabelText(/TestLabel/i)).toHaveValue('new value')
})

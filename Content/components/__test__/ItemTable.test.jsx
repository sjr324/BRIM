import React from 'react'
import { getByText, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import '@testing-library/jest-dom'

import ItemTable from '../items/ItemTable.jsx'

const setup = (itemId, Label, defaultName, disabled) => {
    const { getByText, getByLabelText } =
        render(<ItemTable />)

    return {
        getByText, getByLabelText,
    }
}

test("renders without crashing", () => {
    const { getByText, getByLabelText } = setup()

    getByText('Drink')
    getByText('Quantity')
    getByText('Status')
    getByText('Details')

    getByLabelText('create item')
})

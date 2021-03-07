import React from 'react'
import { fireEvent, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import '@testing-library/jest-dom'

import FormDialog from '../items/ItemDialog.jsx'

const setup = () => {
    let row = {
        name: "testName"
    }

    const { getByText, queryByLabelText } = render(<FormDialog item={row} />)

    return {
        getByText, queryByLabelText
    }
}

test("renders details button", () => {
    const { getByText } = setup()

    getByText('Details')
})

test("clicking details button triggers modal open", () => {
    const { getByText } = setup()
    const detailsButton = getByText(/Details/i)

    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()
})

test("opening item modal has row item's name", () => {
    const { getByText } = setup()
    const detailsButton = getByText(/Details/i)

    fireEvent.click(detailsButton)
    expect(getByText(/testName/i)).toBeTruthy()
})


test("clicking exit button triggers modal exit", () => {
    const { getByText, queryByLabelText } = setup()
    const detailsButton = getByText(/Details/i)

    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()

    const exitButton = getByText(/Close/i)

    fireEvent.click(exitButton)
    expect(queryByLabelText(/"form-dialog-title/i)).toBeNull();
})

test("failing test for edit button", () => {
    const { getByText, queryByLabelText } = setup()

    const detailsButton = getByText(/Details/i)
    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()

    userEvent.type(screen.getByLabelText(/name/i), '{selectall}{del}new value')

    const editButton = getByText(/Edit/i)
    fireEvent.click(editButton)

    const closeButton = getByText(/Close/i)
    fireEvent.click(closeButton)

    const detailsButton = getByText(/Details/i)
    fireEvent.click(detailsButton)

    //not a real test, just failing so that we know that edit functionailty doesn't work yet
    expect(screen.queryByLabelText(/TestLabel/i)).toHaveValue('new value')
})

    //TODO tests: edit (save?) button? 


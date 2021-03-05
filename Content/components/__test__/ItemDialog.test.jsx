import React from 'react'
import { fireEvent, render } from '@testing-library/react'

import FormDialog from '../ItemDialog.jsx'

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

    const editButton = getByText(/Edit/i)
    fireEvent.click(editButton)

    //not a real test, just failing so that we know that edit functionailty doesn't work yet
    expect(queryByLabelText(/"form-dialog-title/i)).toBeTruthy();
})

    //TODO tests: edit (save?) button? 


import React from 'react'
import { fireEvent, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import '@testing-library/jest-dom'

import FormDialog from '../items/ItemDialog.jsx'

const setup = () => {
    let row = {
        name: "testName" ,
        lowerEstimate: "360"
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

test("clicking edit button lets you input things", () => {
    const { getByText } = setup()

    const detailsButton = getByText(/Details/i)
    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()

    const editButton = getByText(/Edit/i)
    fireEvent.click(editButton)

    userEvent.type(screen.getByLabelText(/Lower Estimate/i), '{selectall}{del}new value')

    expect(screen.getByLabelText(/Lower Estimate/i)).toHaveValue('new value')
})

test("clicking editing button, makes save possible", () => {
    const { getByText, queryByLabelText } = setup()

    const detailsButton = getByText(/Details/i)
    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()

    const editButton = getByText(/Edit/i)
    fireEvent.click(editButton)

    expect(getByText(/Save/i)).toBeTruthy()

    userEvent.type(screen.getByLabelText(/Lower Estimate/i), '{selectall}{del}new value')

    const saveButton = getByText(/Save/i)
    fireEvent.click(saveButton)

    expect(screen.getByLabelText(/Lower Estimate/i)).toHaveValue('new value')

    const exitButton = getByText(/Close/i)

    fireEvent.click(exitButton)
    expect(queryByLabelText(/"form-dialog-title/i)).toBeNull();
})



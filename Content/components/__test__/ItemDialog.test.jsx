import React from 'react'
import ReactDom from 'react-dom'
import { fireEvent, render } from '@testing-library/react'

import FormDialog from '../ItemDialog.jsx'


/*test("renders without crashing", () => {
    const div = document.createElement("div");
    let row = {
        name: "shit",
        id: "123"
    }
    ReactDom.render(<FormDialog item={row} />, div)
})*/

test("renders without crashing again", () => {
    let row = {
        name: "shit",
        id: "123"
    }
    const { getByText } = render(<FormDialog item={row} />)

    getByText('Details')
})

test("clicking details button triggers modal", () => {
    let row = {
        name: "shit",
        id: "123"
    }

    const { getByText } = render(<FormDialog item={row} />)
    const detailsButton = getByText(/Details/i)

    fireEvent.click(detailsButton)
    expect(getByText(/Item information:/i)).toBeTruthy()
})



import React from 'react'
import { fireEvent, render } from '@testing-library/react'

import ItemTextFeild from '../ItemTextFeild.jsx'

const setup = () => {
    const { getByText, getByLabelText } = render(<ItemTextFeild id={"itemId"} label="Name" defVal={"defName"} dbl={true} />)

    return {
        getByLabelText,
        getByText

    }
}

test("renders without crashing again", () => {
    const { getByLabelText, getByText } = setup()

    getByText('Name')
})


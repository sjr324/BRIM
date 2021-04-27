import React from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import '@testing-library/jest-dom'

import RecipeTable from '../recipes/RecipeTable.jsx'

const setup = (itemId, Label, defaultName, disabled) => {
    let data = {
        name: "testName",
        lowerEstimate: "360"
    }

    const { getByText, getByLabelText } =
        render(<RecipeTable item={ data }/>)

    return {
        getByText, getByLabelText,
    }
}

test("renders add button", () => {
    const { getByLabelText } = setup()

    getByLabelText('create recipe')
})

test("renders recipe cards", () => {
    const { getByLabelText } = setup()

    getByLabelText('recipes')
})

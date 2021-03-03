/*test("opening item modal has row item's id", () => {
    let row = {
        name: "shit",
        id: "123"
    }

    const { getByText, getByLabelText } = render(<FormDialog item={row} />)
    const detailsButton = getByText(/Details/i)

    fireEvent.click(detailsButton)
    expect(getByLabelText(/123/i)).toBeTruthy()
})*/
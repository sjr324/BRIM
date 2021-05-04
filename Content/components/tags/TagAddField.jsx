import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import AddCircle from '@material-ui/icons/AddCircle';
import TextField from '@material-ui/core/TextField';

export default function TagAddField(props){
	const AddButton = () =>(
		<IconButton>
			<AddCircle />
		</IconButton>
	)
	return(
		<TextField
			margin="dense"
			id="tagadd"
			defaultValue="Add Tag"
			variant="filled"
			fullWidth
			InputProps={{endAdornment: <AddButton/>}}
		/>
	);
}
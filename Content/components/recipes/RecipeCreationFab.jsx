import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import CloseIcon from '@material-ui/icons/Close';
import DoneIcon from '@material-ui/icons/Done';
import Autocomplete from '@material-ui/lab/Autocomplete';
import TextField from '@material-ui/core/TextField';
import ItemTextFeild from '../items/ItemTextFeild.jsx'
import Chip from '@material-ui/core/Chip';

export default function ItemDialog(props) {
	const [open, setOpen] = React.useState(false);
	const [values, setValues] = React.useState({
		newRecipeName: '',
		components: [],
	});
	const [selections, setSelections]=React.useState({
		comps: []
	});

	const handleClickOpen = () => {
		let dataurl = "/inventory/itemnames"
		let xhr = new XMLHttpRequest();
		xhr.open('GET', dataurl, true);

		xhr.setRequestHeader('Content-Type', 'application/json');
		xhr.onload = () => {
			let data = JSON.parse(xhr.responseText);
			console.log(data);
			setValues({
				...values,
				components: data.items,
			});
		};
		xhr.send();
		setOpen(true);
	};
	const handleCancel = () => {

		setOpen(false);
	}
	const handleClose = () => {
		let data = new FormData();
		let submitUrl = "/inventory/newrecipe"
		data.append('name', values.newRecipeName);
		data.append('components', selections.comps);
		let xhr = new XMLHttpRequest();

		xhr.open('POST', submitUrl, true);
		xhr.onload = () => {
			console.log("Done");
		}
		xhr.send(data);
		setOpen(false);

	};

	const handleChangeText = (event) => {
		setValues({ ...values, [event.target.id]: event.target.value });
	};
	const moveSelection = () =>{
		console.log(values.selComponents);
	};

	const displayValues = () => {
		console.log(values);
	};
	//console.log(props);
	return (
		<div>
			<Fab color="primary" aria-label="add" onClick={handleClickOpen}>
				<AddIcon />
			</Fab>
			<Dialog open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
				<DialogTitle id="form-dialog-title">Create New Recipe</DialogTitle>
				<DialogContent>
					<DialogContentText>
						Recipe Information:
          			</DialogContentText>
					<ItemTextFeild id={"newItem" + "Recipe"} label="Name" dbl={false} onChange={handleChangeText} />
					<Autocomplete
						id="component_select"
						multiple
						filterSelectedOptions
						options={values.components}
						getOptionLabel={(option) => option.name}
						style={{ width: 300 }}
						renderTags={(value, getTagProps) =>
							value.map((option, index) => (
								<Chip variant="outlined" label={option.name} {...getTagProps({ index })} />
							))
						}
						renderInput={(params) =>
							<TextField
								{...params}
								label="Select Recipe Components"
								variant="filled"
							/>
						}
						onChange={(event,newValue)=>{
							console.log(newValue);
							selections.comps = newValue	
							console.log(selections);
						}}
					/>
				<Button variant="contained" onClick={moveSelection} color="primary">Shift Selection</Button>
				</DialogContent>
				<DialogActions>
					<Button variant="contained" onClick={handleCancel} color="secondary" startIcon={<CloseIcon />}>
						Cancel
          			</Button>
					<Button variant="contained" onClick={handleClose} color="primary" startIcon={<DoneIcon />}>
						Create Item
          		</Button>
					<Button variant="contained" onClick={displayValues} color="primary">ViewValues</Button>
				</DialogActions>
			</Dialog>
		</div>
	);
}
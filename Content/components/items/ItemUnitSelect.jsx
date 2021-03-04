import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import InputLabel from '@material-ui/core/InputLabel';
import MenuItem from '@material-ui/core/MenuItem';
import FormHelperText from '@material-ui/core/FormHelperText';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';

const useStyles = makeStyles((theme) => ({
  formControl: {
    margin: theme.spacing(1),
    minWidth: 120,
  },
  selectEmpty: {
    marginTop: theme.spacing(2),
  },
}));

export default function ItemUnitSelect(){
	const classes = useStyles();
	const [units, setUnits]= React.useState('');

	const handleChange = (event) => {
		setUnits(event.target.value);
	};

	return(
		<FormControl className = {classes.formControl}>
			<InputLabel id="unitselectlabel">Units:</InputLabel>
			<Select
				labelId="unitselectlabel"
				id="unitselect"
				value = {units}
				onChange = {handleChange}
			>
				<MenuItem value = {1}>ml</MenuItem>

				<MenuItem value = {2}>fl oz</MenuItem>
			</Select>
		</FormControl>
	);
}
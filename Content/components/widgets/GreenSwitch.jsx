import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import { green } from '@material-ui/core/colors';
import Switch from '@material-ui/core/Switch';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Typography from '@material-ui/core/Typography';

const SwitchGreen = withStyles({
	switchBase: {
    color: green[300],
    '&$checked': {
      color: green[500],
    },
    '&$checked + $track': {
      backgroundColor: green[500],
    },
  },
  checked: {},
  track: {},
})(Switch); 

export default function GreenSwitch(props){
	const [state, setState] = React.useState({
		checked : props.checked,
	})


	return(
		<FormControlLabel
			control={<SwitchGreen id={props.id} checked={props.checked} onChange = {props.onChange} name = "checked" />}
			label={props.label}
		/> 
	);
}

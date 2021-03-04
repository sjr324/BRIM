import React from 'react';
import TextField from '@material-ui/core/TextField';

export default function ItemTextFeild(props){
    return(
        <TextField
        disabled = {props.dbl}
        margin="dense"
        id={props.id}
        label={props.label}
        defaultValue = {props.defVal}
        variant="filled"
        value = {props.value}
        onChange= {props.onChange}
      />

    );
}
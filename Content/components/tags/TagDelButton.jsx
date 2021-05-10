import React from 'react';

import { Button  } from '@material-ui/core';
export default  function DelTagButton(props){

	const DetTag=()=>{
		let data = new FormData;
		let submitUrl = "/inventory/deltag"
		data.append('name',props.tag.name);
		data.append('id',props.tag.id);
		console.log(data);

		let xhr= new XMLHttpRequest();

		xhr.open('POST',submitUrl,true);
		xhr.onload=()=>{
			props.onTagSubmit();
			console.log("Tag Deleted");
		}
		xhr.send(data);
		props.onClick();
	}
	return (
		<Button variant="contained" color="secondary" onClick={DelTag}>
			Delete
		</Button>
	)
}
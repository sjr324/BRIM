import React, {useEffect} from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import FormDialog from './ItemDialog.jsx'
import AddItemFab from './AddItemFab.jsx'
import { Button, Container } from '@material-ui/core';
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';

const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
});

export default function ItemTableBasic(props) {
  const classes = useStyles();
	let [state, updateState]= React.useState({
		items:props.initialItems
	});

  const loadItemsFromServer=()=>{
    console.log("Loading items:")
    let itemurl='/inventory/items'
    let xhr = new XMLHttpRequest();
    xhr.open('GET',itemurl,true);
    xhr.setRequestHeader('Content-Type','application/json');
    xhr.onload = ()=>{
      console.log("##################################################################################")
      console.log(xhr.responseText);

      console.log("##################################################################################")
      /*
      let data = JSON.parse(xhr.responseText);
      updateState({
        items: data
      });
      */
    };
    xhr.send();
    
  }
  /*
  useEffect(()=>{
    //loadItemsFromServer();
    //window.setInterval(loadItemsFromServer,1000);
  });
  */

	console.log("Rendering table");
  return (
    <Container maxWidth="md">
    <TableContainer component={Paper}>
      <Table className={classes.table} aria-label="simple table">
        <TableHead>
          <TableRow>
            <TableCell align="center">Drink</TableCell>
            <TableCell align="center">Quantity</TableCell>
            <TableCell align="center">Status</TableCell>
            <TableCell align="center">Details</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {state.items.map((row) => (
            <TableRow key={row.id}>
              <TableCell component="th" scope="row" align="center">
                {row.name}
              </TableCell>
              <TableCell align="center">{row.lowerEstimate}-{row.upperEstimate}</TableCell>
              <TableCell align="center">{row.status}</TableCell>
              <TableCell align="center"><FormDialog item={row} /></TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
    <AddItemFab />
    <Button variant="contained" onClick={loadItemsFromServer} color="primary">Refresh Table</Button>
    </Container>
  
  );
}
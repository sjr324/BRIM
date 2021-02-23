import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

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
    let xhr = new XMLHttpRequest();
    xhr.open('get',this.props.itemurl,true);
    xhr.setRequestHeader('Content-Type','application/json');
    xhr.onload = ()=>{
      let data = JSON.parse(xhr.responseText);
      updateState({
        items: data
      });
    };
    xhr.send();
  }

  useEffect(()=>{
    window.setInterval(loadItemsFromServer,2000);
  });

	console.log(props);
  return (
    <TableContainer component={Paper}>
      <Table className={classes.table} aria-label="simple table">
        <TableHead>
          <TableRow>
            <TableCell align="center">Drink</TableCell>
            <TableCell align="center">Quantity</TableCell>
            <TableCell align="center">Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {state.items.map((row) => (
            <TableRow key={row.name}>
              <TableCell component="th" scope="row">
                {row.name}
              </TableCell>
              <TableCell align="right">{row.lowerEstimate}-{row.upperEstimate}</TableCell>
              <TableCell align="right">{row.status}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
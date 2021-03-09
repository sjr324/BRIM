import React, { useEffect } from 'react';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import RecipeCard from './RecipeCard.jsx';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core';
import RecipeCreationFab from './RecipeCreationFab.jsx';


export default function RecipeTable(props){
  let cdate = new Date();
  console.log("hey, it's me again");
  let [state,updateState]=React.useState({
    recipes:[],
    curdate:cdate.toLocaleString()
  });
	const loadRecipesFromServer=()=>{
		console.log("Loading items:")
    let itemurl='/inventory/recipes'
    let xhr = new XMLHttpRequest();
    xhr.open('GET',itemurl,true);
    xhr.setRequestHeader('Content-Type','application/json');
    xhr.onload = ()=>{
      /*
      console.log("##################################################################################")
      console.log(xhr.responseText);

      console.log("##################################################################################")
      */
      let data = JSON.parse(xhr.responseText);
      console.log(state.curdate+ " Tag:" + Math.floor(Math.random()*1000));
      console.log(data);
      updateState(prevState=>({
        ...prevState,
        recipes: data.recipes,
        curdate: cdate.toLocaleString()
      }));
    };
    xhr.send();
	}
	useEffect(()=>{
		loadRecipesFromServer();
	},[]);
  console.log(state.recipes)
  //let currentdate= new Date().toLocaleString();
  //  console.log("Last Load: " + currentdate);
	return(
        <Grid container spacing={3} direction="column" > 
            <Grid item md={9} > 

                //grid of cards instead of table
                <TableContainer component={Paper}>
                    <Table aria-label="simple table">
                        <TableHead>
                            <TableRow>
                                <TableCell align="center">Recipe Name</TableCell>
                                <TableCell align="center">Recipe Details</TableCell>
                            </TableRow>
                        </TableHead>
                    </Table>
                    <TableBody key={row.id}>
                        {state.recipes.map((row) => (
                            <TableRow>
                                <TableCell component="th" scope="row" align="center">
                                    <RecipeCard key={row.id} {...row}></RecipeCard>
                                </TableCell>
                                <TableCell align="center">
                                    <Button>
                                        details
          			                </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </TableContainer>
            </Grid>

            <Grid item sm={1} >
                <RecipeCreationFab />
            </Grid>
        </Grid>
    );
}
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
import { makeStyles } from '@material-ui/core/styles';

import RecipeCreationFab from './RecipeCreationFab.jsx';

const useStyles = makeStyles((theme) => ({
    root: {
        flexGrow: 1,
    },
    paper: {
        padding: theme.spacing(2),
        textAlign: 'center',
        color: theme.palette.text.secondary,
    },
    fab: {
        position: 'fixed',
        bottom: theme.spacing(2),
        right: theme.spacing(2),
    },
}));

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
    const classes = useStyles();

    return (
        <div className={classes.root}>
            <Grid container justify="flex-start" alignItems="stretch" >
                <Grid container item xs={10} direction="row"
                    justify="space-around" >
                    {state.recipes.map((row) => (
                        <RecipeCard key={row.id} {...row}></RecipeCard>
                        ))}
                </Grid>

                <Grid container item xs={1} alignItems="flex-end">
                    <Grid item xs={12}> 
                        <RecipeCreationFab />
                    </Grid>
                </Grid>
            </Grid>
        </div>
    );
}
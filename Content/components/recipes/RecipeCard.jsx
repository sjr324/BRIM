import React from 'react';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography'
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles({
  root: {
    minWidth: 275,
  },
  bullet: {
    display: 'inline-block',
    margin: '0 2px',
    transform: 'scale(0.8)',
  },
  title: {
    fontSize: 14,
  },
  pos: {
    marginBottom: 12,
  },
});


export default function RecipeCard(props){
	const classes = useStyles();
	console.log(props);
	return(
		<Card className={classes.root} variant="outlined">
			<CardContent>
				<Typography className={classes.title} color="textSecondary" gutterBottom>
					{props.name}	
				</Typography>
			</CardContent>
		</Card>
	);
}
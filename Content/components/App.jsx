import React from 'react'
import { BrowserRouter, Route,StaticRouter, Switch } from "react-router-dom";
import ItemTableBasic from './items.jsx'
import NavDrawer from "./NavDrawer.jsx";
import { makeStyles } from "@material-ui/core/styles";
import { PinDropSharp } from '@material-ui/icons';

const useStyles = makeStyles({
  container: {
    display: "flex"
  }
});

export default function App(props) {
  console.log(props);
  let [state, updateState]= React.useState({
		items:props.initialItems
	});
  const classes = useStyles();
  console.log("here");
  const app = (
    <div className={classes.container}>
      <NavDrawer />
      <Switch>
        <Route exact from="/" render={props => <ItemTableBasic initialItems={state.items} {...props} />} />
      </Switch>
    </div>
  );
  if (typeof window === 'undefined') {
			return (
				<StaticRouter
					context={props.context}
					location={props.location}
				>
					{app}
				</StaticRouter>
			);
		}
  return <BrowserRouter>{app}</BrowserRouter>
}
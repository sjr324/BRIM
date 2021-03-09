import React from 'react';
import { BrowserRouter, Route,StaticRouter, Switch } from "react-router-dom";
import ItemTableBasic from './items/ItemTable.jsx'
import RecipeTable from './recipes/RecipeTable.jsx'
import NavDrawer from "./NavDrawer.jsx";
import { makeStyles } from "@material-ui/core/styles";
import { Helmet} from 'react-helmet';
import { PinDropSharp } from '@material-ui/icons';

const useStyles = makeStyles({
  container: {
    display: "flex"
  }
});

export default function App(props) {
  console.log("App Props: ");
  console.log(props);
  let [state, updateState]= React.useState({
		items:props.initialItems
	});
  const classes = useStyles();
  const app = (
    <div className={classes.container}>
      <Helmet>
        <title>
          BRIM
        </title>
      </Helmet>
      <NavDrawer />
      <Switch>
        <Route exact from="/" render={props => <ItemTableBasic initialItems={state.items}  />} />
      </Switch>
      <Switch>
        <Route exact from="/recipes" render={props => <RecipeTable  />} />
      </Switch>
    </div>
  );
  if (typeof window === 'undefined') {
    console.log("Undefined window");
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
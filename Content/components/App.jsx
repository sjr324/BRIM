import React from 'react'
import { Route, Switch } from "react-router-dom";
import ItemTableBasic from './items'
import NavDrawer from "./NavDrawer";
import { makeStyles } from "@material-ui/core/styles";

const useStyles = makeStyles({
  container: {
    display: "flex"
  }
});

export default function App() {
  const classes = useStyles();
  return (
    <div className={classes.container}>
      <NavDrawer />
      <Switch>
        <Route exact from="/" render={props => <ItemTableBasic {...props} />} />
      </Switch>
    </div>
  );
}
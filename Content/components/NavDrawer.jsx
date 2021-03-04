import React from "react";
import {
  Drawer as MUIDrawer,
  ListItem,
  List,
  ListItemIcon,
  ListItemText
} from "@material-ui/core";
import { makeStyles } from "@material-ui/core/styles";
import InboxIcon from "@material-ui/icons/MoveToInbox";
import MailIcon from "@material-ui/icons/Mail";
import { withRouter } from "react-router-dom";

//https://codesandbox.io/s/winter-brook-fnepe?file=/src/Drawer.jsx:0-1323
const useStyles = makeStyles({
  drawer: {
    width: "190px"
  }
});

const NavDrawer = props => {
  const { history } = props;
  const classes = useStyles();
  const itemsList = [
    {
      text: "Items",
      icon: <InboxIcon />,
      onClick: () => history.push("/")
    },
    {
      text: "Recipes",
      icon: <InboxIcon />,
      onClick: () => history.push("/recipes")
    },
  ];
  return (
    <MUIDrawer variant="permanent" className={classes.drawer}>
      <List>
        {itemsList.map((item, index) => {
          const { text, icon, onClick } = item;
          return (
            <ListItem button key={text} onClick={onClick}>
              {icon && <ListItemIcon>{icon}</ListItemIcon>}
              <ListItemText primary={text} />
            </ListItem>
          );
        })}
      </List>
    </MUIDrawer>
  );
};

export default withRouter(NavDrawer);
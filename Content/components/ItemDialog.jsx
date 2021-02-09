import React from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';

export default function ItemDialog(props) {
  const [open, setOpen] = React.useState(false);

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };
  console.log(props);
  return (
    <div>
      <Button variant="outlined" color="primary" onClick={handleClickOpen}>
        Details
      </Button>
      <Dialog open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">{props.item.name}</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Item information:
          </DialogContentText>
          <TextField
            autoFocus
            disabled
            margin="dense"
            id="name"
            label="Name"
            defaultValue = {props.item.name}
            variant="filled"
          />
          
        </DialogContent>
        <DialogActions>
          <Button variant = "contained" onClick={handleClose} color="primary">
           Edit 
          </Button>
          <Button variant = "contained" onClick={handleClose} color="primary">
           Close 
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
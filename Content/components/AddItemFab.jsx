import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import ItemTextFeild from './ItemTextFeild.jsx'
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import CloseIcon from '@material-ui/icons/Close'
import DoneIcon from '@material-ui/icons/Done'

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
        <Fab color="primary" aria-label="add" onClick={handleClickOpen}>
            <AddIcon />
        </Fab>
      <Dialog open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">Create New Item</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Item information:
          </DialogContentText>
          <ItemTextFeild id={"newItem" + "Name"} label = "Name" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Id"} label = "Id" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Status"} label = "Status" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "loEst"} label = "Lower Estimate" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "upEst"} label = "Upper Estimate" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Ideal"} label = "Ideal Level" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Par"} label = "Par Level" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Brand"} label = "Brand" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Vintage"} label = "Vintage" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Price"} label = "Price"  dbl={false}/> 
          <ItemTextFeild id={"newItem" + "bottleSize"} label = "Bottle Size" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "UPC"} label = "Units per Case" dbl={false}/> 
          <ItemTextFeild id={"newItem" + "Measurement"} label = "Measurement" dbl={false}/> 
        </DialogContent>
        <DialogActions>
          <Button variant = "contained" onClick={handleClose} color="secondary" startIcon={<CloseIcon/>}>
            Cancel
          </Button>
          <Button variant = "contained" onClick={handleClose} color="primary" startIcon={<DoneIcon/>}>
            Create Item
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
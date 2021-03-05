import React from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import ItemTextFeild from './ItemTextFeild.jsx'

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
      <Dialog data-testid="modal" open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">{props.item.name}</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Item information:
          </DialogContentText>
          <ItemTextFeild id={props.item.id + "Name"} label = "Name" defVal = {props.item.name} dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Id"} label = "Id" defVal = {props.item.id} dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Status"} label = "Status" defVal = {props.item.status}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "loEst"} label = "Lower Estimate" defVal = {props.item.lowerEstimate}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "upEst"} label = "Upper Estimate" defVal = {props.item.upperEstimate}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Ideal"} label = "Ideal Level" defVal = {props.item.idealLevel}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Par"} label = "Par Level" defVal = {props.item.parLevel}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Brand"} label = "Brand" defVal = {props.item.brand}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Vintage"} label = "Vintage" defVal = {props.item.vintage}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Price"} label = "Price" defVal = {props.item.price}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "bottleSize"} label = "Bottle Size" defVal = {props.item.bottleSize}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "UPC"} label = "Units per Case" defVal = {props.item.unitsPerCase}dbl={true}/> 
          <ItemTextFeild id={props.item.id + "Measurement"} label = "Measurement" defVal = {props.item.measurement}dbl={true}/> 
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
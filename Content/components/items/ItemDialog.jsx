import React from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import ItemTextFeild from './ItemTextFeild.jsx'
import GreenSwitch from '../widgets/GreenSwitch.jsx'
import ItemUnitSelect from './ItemUnitSelect.jsx'

export default function ItemDialog(props) {
  const [open, setOpen] = React.useState(false);
  const [values, setValues] = React.useState({
    ...props.item
  })
  const [edit,setEdit]= React.useState(true);
  const [text,setText]= React.useState("Edit") 

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };
  const handleChangeText =(event)=>{
    setValues({...values,[event.target.id]:event.target.value});
  };
  const handleChangeSelect = (event)=>{
    setValues({...values,newItemUnits:event.target.value});
  }
  const handleChangeSwitch =(event)=>{

    setValues({...values,[event.target.id]:event.target.checked});
  };
  const handleSave= () => {
    let data = new FormData();
    let submitUrl = "/inventory/newitem" 
    data.append('name', values.name);
    data.append('lo', values.lowerEstimate);
    data.append('hi', values.upperEstimate);
    data.append('ideal', values.idealLevel);
    data.append('par', values.parLevel);
    data.append('brand', values.brand);
    data.append('price', values.price);
    data.append('size', values.bottleSize);
    data.append('upc', values.unitsPerCase);
    data.append('vinatage', values.vintage);
    data.append('units', values.measurement);
    data.append('id',values.id);
    let xhr = new XMLHttpRequest(); 

    xhr.open('POST',submitUrl,true);
    xhr.onload = () =>{
      console.log("Done");
    }
    xhr.send(data);
    
  };
  const toggleEdit= () =>{
    setEdit(!edit);
    //not these are inverted because I can only toggle whether buttons are enabled or disabled
    if(edit==true){
      setText("Save")
    }else{
      handleSave()
      setText("Edit")
    }
  }
  return (
    <div>
      <Button variant="contained" color="primary" onClick={handleClickOpen}>
        Details
      </Button>
      <Dialog open={open} onClose={handleClose} aria-labelledby="form-dialog-title">
        <DialogTitle id="form-dialog-title">{props.item.name}</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Item information:
          </DialogContentText>
          <ItemTextFeild id={"name"} label = "Name" defVal = {props.item.name} dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"lowerEstimate"} label = "Lower Estimate" defVal = {props.item.lowerEstimate}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"upperEstimate"} label = "Upper Estimate" defVal = {props.item.upperEstimate}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"idealLevel"} label = "Ideal Level" defVal = {props.item.idealLevel}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"parLevel"} label = "Par Level" defVal = {props.item.parLevel}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"brand"} label = "Brand" defVal = {props.item.brand}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"price"} label = "Price" defVal = {props.item.price}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"bottleSize"} label = "Bottle Size" defVal = {props.item.bottleSize}dbl={edit}onChange = {handleChangeText}/> 
          <ItemTextFeild id={"unitsPerCase"} label = "Units per Case" defVal = {props.item.unitsPerCase}dbl={edit}onChange = {handleChangeText}/> 

          <ItemUnitSelect id={"measurement"} value={values.measurement} disabled={edit} onChange={handleChangeSelect}/>

          <GreenSwitch id={"vintage"} checked={values.vintage} disabled={edit} onChange={handleChangeSwitch} label={"Vintage"} />
        </DialogContent>
        <DialogActions>
          <Button variant = "contained" onClick={toggleEdit} color="primary">
            {text} 
          </Button>
          <Button variant = "contained" onClick={handleClose} color="primary">
           Close 
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
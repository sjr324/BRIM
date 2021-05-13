import Grid from '@material-ui/core/Grid';
import Autocomplete from '@material-ui/lab/Autocomplete';
import TextField from '@material-ui/core/TextField';


export default function StatisticsPage(props) {

    let [state, updateState] = React.useState({
        items: [],
    });

    let dataurl = "/inventory/itemnames"
    let xhr = new XMLHttpRequest();
    xhr.open('GET', dataurl, true);

    xhr.onload = () => {
        let data = JSON.parse(xhr.responseText);
        updateState({
            items: data.items
        });
    };
    xhr.send();


    return (
        <Grid container>
            <Grid item xs={10}> 
                <Autocomplete
                    options={state.items}
                    getOptionLabel={(option) => option.name}
                    renderInput={(params) => <TextField {...params} label="Combo box" variant="outlined" />}
                />
            </Grid>
        </Grid>
    );
}
import React from 'react';
import Button from '@material-ui/core/Button';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';

export default function NtfcnSnackbar() {
	const [open, setOpen] = React.useState(false);


	useEffect(() => {
		const sse = new EventSource('/notification/get',{ withCredentials: true }); 
		function getRealtimeData(data) {
				console.log(data);
				setOpen(true);
				// process the data here,
				// then pass it to state to be rendered
			} sse.onmessage = e => getRealtimeData(JSON.parse(e.data)); 
			sse.onerror = () => {
				// error log here 

				sse.close();
			}
		return () => {
			sse.close();
		};
	}, []);
	const handleClose = (event, reason) => {
		if (reason === 'clickaway') {
			return;
		}

		setOpen(false);
	};

	return (
		<div>
			<Snackbar
				anchorOrigin={{
					vertical: 'bottom',
					horizontal: 'left',
				}}
				open={open}
				autoHideDuration={6000}
				onClose={handleClose}
				message="Note archived"
				action={
					<React.Fragment>
						<Button color="secondary" size="small" onClick={handleClose}>
							UNDO
            </Button>
						<IconButton size="small" aria-label="close" color="inherit" onClick={handleClose}>
							<CloseIcon fontSize="small" />
						</IconButton>
					</React.Fragment>
				}
			/>
		</div>
	);
}
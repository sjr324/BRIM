class ItemWrapper extends React.Component{
	state = {
		data:this.props.initialData

	};
	loadItemsFromServer = () =>{
		var xhr = new XMLHttpRequest();
		xhr.open('get',this.props.url,true);
		xhr.onload = function() {
			var data = JSON.parse(xhr.responseText);
			this.setState({ data: data });
		}.bind(this);
		xhr.send();
	};
	componentDidMount() {
		window.setInterval(this.loadItemsFromServer, this.props.pollInterval);
	}
	render(){
		return(
			<div className="nice" id="content" >
				<div className="jumbotron jumbotron-fluid">
					<h1>Items</h1> 
				</div>
				
				<ItemList data={this.state.data} />
			</div>
		);
	}
}
class ItemList extends React.Component{
	render(){
		var itemNodes = this.props.data.map(function(item){
			return(
				<Item item = {item} key={item.id} />
			);
		});
	return (
			<table className = "table table-bordered table-striped table-hover" >
				<thead>
					<tr>
						<th>Item</th>
						<th>Quantity</th>
						<th>Details</th>
					</tr>
				</thead>
				<tbody>
				{itemNodes}
				</tbody>
			</table>
		);
	}

}

class Item extends React.Component{
	
	render(){
		return (
			<tr className = "item">
				<td className="itemname">{this.props.item.name}</td>
				<td className="itemquant">{this.props.item.lowerEstimate}-{this.props.item.upperEstimate}</td>
				<td>
				<button type="button" className="btn btn-danger" data-toggle="modal" data-target={this.props.item.ID} >Details</button>
				<ItemModal item={this.props.item} />
				</td>
			</tr>
		)
	}
}

class ItemModal extends React.Component{
	render(){
		return (
			<div className="modal" id={this.props.item.ID}>
				<div className="modal-dialog">
					<div className="modal-content">

						<div className="modal-header">
							<h4 className="modal-title">Example Item</h4>
							<button type="button" className="close" data-dismiss="modal">&times;</button>
						</div>

						<div className="modal-body">
							<table className="table-bordered">
								<tbody>
								<tr>
									<td>
										Bottle Size
									</td>
									<td>
										750ml
									</td>
								</tr>
								<tr>
									<td>
										Brand
									</td>
									<td>
										Example Brand
									</td>
								</tr>
								<tr>
									<td>
										Cost
									</td>
									<td>
										$10
									</td>
								</tr>
								<tr>
									<td>
										Units Per Case
									</td>
									<td>
										12
									</td>
								</tr>
								<tr>
									<td>
										Vintage
									</td>
									<td>
										No
									</td>
								</tr>
								<tr>
									<td>
										Par Level
									</td>
									<td>
										30 oz
									</td>
								</tr>
								<tr>
									<td>
										Ideal Level
									</td>
									<td>
										70 oz
									</td>
								</tr>
								<tr>
									<td>
										Current Inventory
									</td>
									<td>
										60 - 80 oz
									</td>
								</tr>
								</tbody>
							</table>
							<button type="button" className="btn btn-warning">Update Item</button>
							<button type="button" className="btn btn-danger">Delete Item</button>
						</div>

						<div className="modal-footer">
							<button type="button" className="btn btn-success" data-dismiss="modal">Ok</button>
						</div>

					</div>

				</div>
			</div>
		);
	}
}


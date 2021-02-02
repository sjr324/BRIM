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
			<div className="ItemWrapper">
				<h1>Items</h1> 
				<ItemList data={this.state.data} />
			</div>
		);
	}
}
class ItemList extends React.Component{
	render(){
		var itemNodes = this.props.data.map(function(item){
			return(
				<Item name={item.name} />
			);
		});
	return <div className="itemList">{itemNodes}</div>;
	}

}

class Item extends React.Component{
	
	render(){
		return (
			<div className="item">
				<h2 className="itemname">{this.props.name}</h2>
			</div>
		)
	}
}


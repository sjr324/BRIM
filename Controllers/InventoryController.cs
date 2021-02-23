using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BRIM.BackendClassLibrary;

namespace BRIM 
{
	public class InventoryController: Controller
	{

		private Inventory inventory;
		private const int COMMENTS_PER_PAGE = 3;


		public InventoryController()
		{
			//initialize the inventory
			inventory = new Inventory();
			inventory.GetItemList();
			inventory.GetRecipeList();	
		}
		[Route("items")]
		[HttpGet]
		public ActionResult Items(){
			return View(new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});
		}
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}

/*
		public class IndexViewModel
		{
			public IReadOnlyList<CommentModel> Comments { get; set; }
			public int CommentsPerPage { get; set; }
			public int Page { get; set; }
		}
		*/
	}
}

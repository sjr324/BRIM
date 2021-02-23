using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BRIM.BackendClassLibrary;

namespace BRIM 
{
	public class InventoryController: Controller
	{

		private Inventory inventory;


		public InventoryController()
		{
			//initialize the inventory
			inventory = new Inventory();
			inventory.GetItemList();
			inventory.GetRecipeList();	
		}
		public ActionResult Index(){
			return View(new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});
		}
		
		public ActionResult Items(){

			if (ControllerContext.HttpContext.Request.ContentType == "application/json")
			{
				return new JsonResult(new
				{
					Items= inventory.ItemList.AsReadOnly()
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});	
		}
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}


	}
}

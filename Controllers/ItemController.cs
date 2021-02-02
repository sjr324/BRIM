using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BRIM;

namespace BRIM
{
	public class ItemController : Controller
	{
		private Inventory inventory;
		private readonly IList<Item> _items;

		public ItemController()
		{
			inventory= new Inventory();
			inventory.GetItemList();
			inventory.GetRecipeList();
			

			_items = inventory.ItemList.AsReadOnly();
			
			
		}

		public ActionResult Index()
		{
			return View(_items);
		}

		[Route("items")]
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
		public ActionResult Items()
		{
			return Json(_items);
		}

		[Route("items/new")]
		[HttpPost]
		public ActionResult AddItem(Item item)
		{
			// Create a fake ID for this comment
			item.ID = _items.Count + 1;
			_items.Add(item);
			return Content("Success :)");
		}
	}
}
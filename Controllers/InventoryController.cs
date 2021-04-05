using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using BRIM.BackendClassLibrary;
using Microsoft.Extensions.Logging;

namespace BRIM 
{
	public class InventoryController: Controller
	{
		private readonly ILogger<InventoryController> _logger;

		private Inventory inventory;
		


		public InventoryController(ILogger<InventoryController> logger)
		{
			_logger = logger;
			_logger.LogInformation("In inventory");
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
			_logger.LogInformation("Content type"+ ControllerContext.HttpContext.Request.ContentType, DateTimeOffset.Now);
			if (ControllerContext.HttpContext.Request.ContentType == "application/json")
			{
				List<Drink> items = inventory.ItemList.Select(p=>(Drink)p).ToList();
				return new JsonResult(new
				{
					Items= items.AsReadOnly()
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.Select(p=>(Drink)p).ToList().AsReadOnly()
			});	
		}
		
		public ActionResult SubmitItem(DrinkSubmissionModel item){
			Drink dr;
			if(item.Id==-1){

				dr = new Drink();
				//set the id to the next highest one
				dr.ID =  this.inventory.ItemList.Select(p=> p.ID).Max()+1;
			}else{
				dr=(Drink)this.inventory.ItemList.Where(p=>p.ID == item.Id).ToList().First();
			}
			dr.Name = item.Name;
			dr.Brand = item.Brand;
			dr.Estimate = Convert.ToDouble(item.Est);
			dr.Measurement= (unit) Enum.Parse(typeof(unit), item.Units);
			dr.Price = Convert.ToDouble(item.Price);
			dr.IdealLevel = Convert.ToDouble(item.Ideal);
			dr.ParLevel = Convert.ToDouble(item.Par);
			dr.BottleSize = Convert.ToInt32(item.Size);
			dr.UnitsPerCase = Convert.ToInt32(item.Upc);
			if (item.Vintage != ""){
				dr.Vintage = Convert.ToInt32(item.Vintage);
			}else{
				dr.Vintage = null;
			}
			dr.CalculateStatus();
			if(item.Id == -1){
				this.inventory.AddItem(dr);
			}else{
				this.inventory.UpdateItem(dr);
			}
			return Content("Success");
		}
		
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}

	}
}

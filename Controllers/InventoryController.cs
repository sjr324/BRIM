using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using bcl = BRIM.BackendClassLibrary;
using Microsoft.Extensions.Logging;

namespace BRIM 
{
	public class InventoryController: Controller
	{
		private readonly ILogger<InventoryController> _logger;

		


		public InventoryController(ILogger<InventoryController> logger)
		{
			_logger = logger;
			_logger.LogInformation("In inventory");
			//initialize the inventory
			bcl.Inventory.GetItemList();

		}
		public ActionResult Index(){
			return View(new ItemViewModel{
				Items = bcl.Inventory.ItemList.ToList()
			});
		}
		
		public ActionResult Items(){
			_logger.LogInformation("Content type"+ ControllerContext.HttpContext.Request.ContentType, DateTimeOffset.Now);
			if (ControllerContext.HttpContext.Request.ContentType == "application/json")
			{
				List<bcl.Drink> items = bcl.Inventory.ItemList.Select(p=>(bcl.Drink)p).ToList();
				return new JsonResult(new
				{
					Items= items.AsReadOnly()
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = bcl.Inventory.ItemList.Select(p=>(bcl.Drink)p).ToList().AsReadOnly()
			});	
		}
		
		public ActionResult SubmitItem(DrinkSubmissionModel item){
			bcl.Drink dr;
			if(item.Id==-1){

				dr = new bcl.Drink();
				//set the id to the next highest one
				dr.ID =  bcl.Inventory.ItemList.Select(p=> p.ID).Max()+1;
			}else{
				dr=(bcl.Drink)bcl.Inventory.ItemList.Where(p=>p.ID == item.Id).ToList().First();
			}
			dr.Name = item.Name;
			dr.Brand = item.Brand;
			dr.Estimate = Convert.ToDouble(item.Est);
			dr.Measurement= (bcl.unit) Enum.Parse(typeof(bcl.unit), item.Units);
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
				bcl.Inventory.AddItem(dr);
			}else{
				bcl.Inventory.UpdateItem(dr);
			}
			return Content("Success");
		}
		
		
		public class ItemViewModel
		{
			public IReadOnlyList<bcl.Item> Items { get; set; }

		}

	}
}

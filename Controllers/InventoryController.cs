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
				return new JsonResult(new
				{
					Items= inventory.ItemList.AsReadOnly()
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});	
		}
		public ActionResult Recipes(){
			_logger.LogInformation("Recipe call");
			if (ControllerContext.HttpContext.Request.ContentType == "application/json"){
				List<RecipeView> reclist = this.inventory.RecipeList.Select(p=>new RecipeView()
				{
					id = p.ID,
					name = p.Name,
					baseliquor = p.BaseLiquor,
					components = p.ItemList.Select(q=>new RecipeComponent()
					{
						component = q.Item1,
						amount = q.Item2
					}).ToList()
				}).ToList();
				return new JsonResult(new{
					Recipes = reclist.AsReadOnly()//,
					//RecursionLimit=100
				});
			}
			return View("~/Views/Home/Index.cshtml",new ItemViewModel{
				Items = this.inventory.ItemList.AsReadOnly()
			});
		}
		
		public ActionResult SubmitItem(ItemModel item){
			int maxid = this.inventory.ItemList.Select(p=> p.ID).Max();
			Drink dr = new Drink();
			dr.ID = maxid+1;
			dr.Name = item.Name;
			dr.LowerEstimate = Convert.ToDouble(item.Lo);
			dr.UpperEstimate = Convert.ToDouble(item.Hi);
			dr.Measurement= (unit) Enum.Parse(typeof(unit), item.Units);
			dr.Price = Convert.ToDouble(item.Price);
			dr.IdealLevel = Convert.ToDouble(item.Ideal);
			dr.ParLevel = Convert.ToDouble(item.Par);
			dr.BottleSize = Convert.ToInt32(item.Size);
			dr.UnitsPerCase = Convert.ToInt32(item.Upc);
			dr.Vintage = item.Vintage;
			dr.CalculateStatus();
			this.inventory.AddItem(dr);
			return Content("Success");
		}
		public class ItemModel{
			public string Name{get;set;}
			public string Lo{get;set;}
			
			public string Hi{get;set;}

			public string Ideal{get;set;}

			public string Par{get;set;}

			public string Brand{get;set;}

			public string Price{get;set;}

			public string Size{get;set;}

			public string Upc{get;set;}

			public bool Vintage{get;set;}

			public string Units{get;set;}

		}
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}
		public class RecipeView
		{
			public int id {get;set;}
			public string name{get;set;}

			public string baseliquor{get;set;}
			public IReadOnlyList<RecipeComponent> components {get;set;}

		}
		public class RecipeComponent
		{
			public Item component{get;set;}
			public double amount{get;set;}
		}
		public class RecipeListViewModel
		{
			public IReadOnlyList<RecipeView> recipes { get;set;}
		}


	}
}

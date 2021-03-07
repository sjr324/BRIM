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
			Drink dr;
			if(item.id==-1){

				dr = new Drink();
				//set the id to the next highest one
				dr.ID =  this.inventory.ItemList.Select(p=> p.ID).Max()+1;
			}else{
				dr=(Drink)this.inventory.ItemList.Where(p=>p.ID == item.id).ToList().First();
			}
			dr.Name = item.Name;
			dr.Brand = item.Brand;
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
			if(item.id == -1){
				this.inventory.AddItem(dr);
			}else{
				this.inventory.UpdateItem(dr);
			}
			return Content("Success");
		}
		public ActionResult SubmitRecipe([FromBody]RecipeModel recipe){
			int maxid = this.inventory.RecipeList.Select(p=>p.ID).Max();
			Recipe r = new Recipe();
			r.ID = maxid;
			r.Name = recipe.name;
			r.ItemList =
				(from item in inventory.ItemList
				join comp in recipe.components
				on item.ID equals comp.id
				select (item, Convert.ToDouble(comp.quantity))).ToList();
			inventory.AddRecipe(r);
			return Content("Success");
		}
		public ActionResult ItemNames(){
			List<RecipeComponentModel> names = this.inventory.ItemList.Select(p=>new RecipeComponentModel
			{
				id = p.ID,
				name = p.Name,
				brand = ((Drink)p).Brand,
				baseliquor = false,
				quantity = "0" 
			}).ToList();
			return new JsonResult(new{
				items = names.AsReadOnly()
			});
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
			public int id{get;set;}
		}
		
		public class ItemViewModel
		{
			public IReadOnlyList<Item> Items { get; set; }

		}
		public class RecipeModel
		{
			public string name{get;set;}
			public List<RecipeComponentModel> components{get;set;}
		}
		/// <summary>
		/// This should be renamed to something else less confusing
		/// </summary>
		public class RecipeComponentModel{
			public int id{get;set;}
			public string name{get;set;}
			public string brand{get;set;}
			public bool baseliquor{get;set;}
			public string quantity{get;set;}
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

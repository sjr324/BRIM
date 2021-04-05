using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using BRIM.BackendClassLibrary;
using Microsoft.Extensions.Logging;

namespace BRIM 
{
	public class RecipesController: Controller
	{
		private readonly ILogger<InventoryController> _logger;

		private Inventory inventory;
		


		public RecipesController(ILogger<InventoryController> logger)
		{
			_logger = logger;
			_logger.LogInformation("In inventory");
			//initialize the inventory
			inventory = new Inventory();
			inventory.GetItemList();
			inventory.GetRecipeList();	

		}
		
		public ActionResult Recipes(){
			_logger.LogInformation("Recipe call");
			List<RecipeModel> reclist = this.inventory.RecipeList.Select(p=>new RecipeModel()
			{
				id = p.ID,
				name = p.Name,
				components = p.ItemList.Select(q=>new RecipeComponentModel()
				{
					id = q.Item.ID,
					name= q.Item.Name,
					brand=((Drink)q.Item).Brand,
					baseliquor = false,
					quantity= Convert.ToString(q.Quantity)
				}).ToList()
			}).ToList();
			return new JsonResult(new{
				Recipes = reclist.AsReadOnly()//,
				//RecursionLimit=100
			});
			
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
				select ( new RecipeItem((Drink)item,Convert.ToDouble(comp.quantity))
				)).ToList();
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
		
		public class RecipeModel
		{
			public int id{get;set;}
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
		
		public class RecipeComponent
		{
			public Item component{get;set;}
			public double amount{get;set;}
		}

	}
}

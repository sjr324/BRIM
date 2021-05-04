using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using bcl=BRIM.BackendClassLibrary;

namespace BRIM
{
	public class TagController : Controller
	{
		public TagController(){
			bcl.Inventory.GetTagList();	
		}

		public ActionResult GetTags()
		{
			return View(new TagListModel 
			{
				tags=bcl.Inventory.TagList.AsReadOnly()
			});
		}
		public ActionResult AddTag(bcl.Tag tag)
		{
			bcl.Inventory.AddTag(tag.Name);
			return Content("success");
		}
		public ActionResult DelTag(bcl.Tag tag)
		{
			bcl.Inventory.RemoveTag(tag.ID);
			return Content("success");
		}
		public class TagListModel
		{
			public IReadOnlyList<bcl.Tag> tags{get;set;}
		}
	}
}
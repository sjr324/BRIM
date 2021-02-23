using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BRIM
{
	public class HomeController : Controller
	{
		private readonly IList<UrlModel> _urls;
		public HomeController(){

		}

		public ActionResult Index()
		{
			return View(new UrlViewModel
			{
				urls=_urls.ToList().AsReadOnly()
			});
		}
		public class UrlModel
		{
			public string Url{get;set;}
		}
		public class UrlViewModel
		{
			public IReadOnlyList<UrlModel> urls {get;set;}
		}
	}
}
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

//namespace React.Sample.Webpack.CoreMvc
namespace BRIM
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Inventory test = new Inventory();
			test.GetItemList();
			test.GetRecipeList();
			Console.WriteLine(test.ItemList);

			//BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
	}
}

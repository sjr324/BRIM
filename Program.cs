using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using BRIM.BackendClassLibrary;
//why is This Using Directive unneccesary? Is it because it's not in a 
//separate Project, but rather a free-floating file in the project that contain the Class Library? 

//namespace React.Sample.Webpack.CoreMvc
namespace BRIM
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
	}
}

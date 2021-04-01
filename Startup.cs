using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using React.AspNet;

//namespace React.Sample.Webpack.CoreMvc
namespace BRIM
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
				.AddChakraCore();

			services.AddReact();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// Build the intermediate service provider then return it
			services.BuildServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostEnvironment env)
		{
			// Initialise ReactJS.NET. Must be before static files.
			app.UseReact(config =>
			{
				config
					.SetReuseJavaScriptEngines(true)
					.SetLoadBabel(false)
					.SetLoadReact(false)
					.SetReactAppBuildPath("~/dist");
			});

			if (env.IsDevelopment())
			{
					app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				/*
				endpoints.MapControllerRoute(
					name:"home",
					pattern:"/",
					defaults: new {controller = "Home", action = "loadurls"}
				);
				*/
				
				endpoints.MapControllerRoute(
					name: "items",
					pattern: "inventory/items",
					defaults: new { controller = "Inventory", action = "Items" }
				);
				endpoints.MapControllerRoute(
					name: "additem",
					pattern: "inventory/newitem",
					defaults: new { controller = "Inventory", action = "SubmitItem" }
				);
				endpoints.MapControllerRoute(
					name:"recipes",
					pattern:"inventory/recipes",
					defaults: new { controller = "Recipes", action = "Recipes"}
				);
				endpoints.MapControllerRoute(
					name: "addrecipe",
					pattern: "inventory/newrecipe",
					defaults: new {controller = "Recipes", action = "SubmitRecipe"}
				);
				endpoints.MapControllerRoute(
					name:"item_names",
					pattern: "inventory/itemnames",
					defaults: new {controller = "Recipes", action = "ItemNames"}
				);
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{path?}",
					new {controller = "Inventory", action = "Index"}
				);
				
				//endpoints.MapControllerRoute("default", "{path?}", new { controller = "Home", action = "Index" });
				//endpoints.MapControllerRoute("comments-root", "comments", new { controller = "Home", action = "Index" });
				//endpoints.MapControllerRoute("comments", "comments/page-{page}", new { controller = "Home", action = "Comments" });
			});
		}
	}
}

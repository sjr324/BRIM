using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Certificate;
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
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddDebug();
			})
				.UseStartup<Startup>()
				.ConfigureKestrel(o=>{
					o.ConfigureHttpsDefaults(o=>o.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.RequireCertificate);
				})
				.Build();
	}
	//A change woo hoo!
}

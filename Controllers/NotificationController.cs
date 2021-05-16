using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BRIM.BackendClassLibrary;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BRIM 
{
	public class NotificationController: Controller
	{
		public async Task Get()
    {
        var response = Response;
        response.Headers.Add("Content-Type", "text/event-stream");

        for(var i = 0; true; ++i)
        {
            await response
                .WriteAsync($"data: Controller {i} at {DateTime.Now}\r\r");

            response.Body.Flush();
            await Task.Delay(5 * 1000);
        }
    }
	}
}
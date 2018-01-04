﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BlogWeb.Controllers
{
	public abstract class BaseController : Controller
	{
		private readonly JsonSerializerSettings jsonSettings;

		

		public BaseController()
		{
			this.jsonSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			};
		}
		protected string ToJsonString(object model)
		{
			return JsonConvert.SerializeObject(model, this.jsonSettings);
		}

		

		
	}
}
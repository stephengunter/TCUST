﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Blog.DAL;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Permissions.DAL;
using ApplicationCore.Helpers;
using BlogWeb.Authorization;
using Permissions.Services;
using Microsoft.AspNetCore.Http.Features;
using Tcust.Services;
using Tcust.DAL;

namespace BlogWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		

		public void ConfigureServices(IServiceCollection services)
		{
			

			services.AddMvc().AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			services.AddAuthorization(options =>
			{
				
				options.AddPolicy("DEV_ONLY", policy =>
				  policy.RequireRole("Dev"));

				options.AddPolicy("EDIT_POSTS", policy =>
					policy.Requirements.Add(new HasPermissionRequirement("EDIT_POSTS")));

				options.AddPolicy("REVIEW_POSTS", policy =>
					policy.Requirements.Add(new HasPermissionRequirement("REVIEW_POSTS")));

				options.AddPolicy("MANAGE_USERS", policy =>
					policy.Requirements.Add(new HasPermissionRequirement("MANAGE_USERS")));
			});

			services.AddScoped<IAuthorizationHandler, HasPermissionHandler>();
			
			

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = "Cookies";
				options.DefaultChallengeScheme = "oidc";
			})
			.AddCookie("Cookies")
			.AddOpenIdConnect("oidc", options =>
			{
				options.SignInScheme = "Cookies";

				options.Authority = Configuration["AppSettings:AuthUrl"];
				options.RequireHttpsMetadata = false;

				options.ClientId = Configuration["AppSettings:AuthId"];
				options.ClientSecret = Configuration["AppSettings:AuthSecret"]; ;
				options.ResponseType = "code id_token";

				options.SaveTokens = true;
				options.GetClaimsFromUserInfoEndpoint = true;
					
				options.Scope.Add("apiApp");
				options.Scope.Add("profile");
				options.Scope.Add("offline_access");

				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					NameClaimType = "name",
					RoleClaimType = "role"
				};



			});



			services.AddOptions();

			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

			services.AddDbContext<PermissionContext>(c =>
			{
				try
				{

					c.UseSqlServer(Configuration.GetConnectionString("BlogConnection"));
				}
				catch (System.Exception ex)
				{
					var message = ex.Message;
				}
			});

			services.AddDbContext<BlogContext>(c =>
			{
				try
				{

					c.UseSqlServer(Configuration.GetConnectionString("BlogConnection"));
				}
				catch (System.Exception ex)
				{
					var message = ex.Message;
				}
			});

			services.AddDbContext<TcustContext>(c =>
			{
				try
				{

					c.UseSqlServer(Configuration.GetConnectionString("TcustConnection"));
				}
				catch (System.Exception ex)
				{
					var message = ex.Message;
				}
			});




			services.AddScoped(typeof(IPermissionRepository<>), typeof(PermissionRepository<>));

			services.AddScoped(typeof(IBlogRepository<>), typeof(BlogRepository<>));

			services.AddScoped(typeof(ITcustRepository<>), typeof(TcustRepository<>));

			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IAttachmentService, AttachmentService>();
			services.AddScoped<ITopPostService, TopPostService>();

			services.AddScoped<IDepartmentService, DepartmentService>();
			services.AddScoped<ITermService, TermService>();
			services.AddScoped<ITargetService, TargetService>();


			services.AddScoped<IPermissionService, PermissionService>();




		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }



			app.UseAuthentication();

			
		

			

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "areaRoute",
					template: "{area:exists}/{controller=Posts}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "default",
					template: "{controller=Posts}/{action=Index}/{id?}");
			});

			






		}
    }
}

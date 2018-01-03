﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Blog.Models;
using Blog.Services;
using ApplicationCore.Helpers;
using BlogWeb.Models;
using ApplicationCore.Paging;

using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlogWeb.Areas.Admin.Controllers
{
	


	public class PostsController: BaseAdminController
	{
		private readonly int pagesize = 8;
		private readonly IPostService postService;

		public PostsController(IHostingEnvironment environment, IPostService postService) : base(environment)
		{
			this.postService = postService;
		}

		public IActionResult Test()
		{


			return View();
		}

		

		public async Task<IActionResult> Index()
		{
			int year = 0;
			int page = 1;


			var allPost = await postService.GetAllAsync();

			year = await postService.CheckYearAsync(year, allPost);

			var yearPosts = await postService.GetByYearAsync(year, allPost);


			var pageList = new PagedList<Post, PostViewModel>(yearPosts, page, this.pagesize);

			foreach (var item in pageList.List)
			{
				pageList.ViewList.Add(new PostViewModel(item));
			}

			pageList.List = null;


			var model = new PostSearchModel();
			model.PagedList = pageList;

			ViewBag.list = this.ToJsonString(model.PagedList);

			return View();
		}

		[HttpGet]
		public IActionResult Create()
		{
			var model = new PostEditForm
			{
				post = new PostViewModel()
			};
		


			return new ObjectResult(model);
		}

		[HttpPost]
		public IActionResult Store([FromBody] PostEditForm model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var post = model.post.MapToEntity(CurrentUserId);

			foreach (var  item in model.post.medias)
			{
				post.Attachments.Add(item.MapToEntity(CurrentUserId));
			}
			
		

			post = postService.Create(post);


			return new ObjectResult(post);

		
		}

		[HttpGet("{id}")]
		public IActionResult Edit(int id)
		{
			var post = postService.GetById(id);
			if (post == null) return NotFound();

			bool allMedias = true;
			var model = new PostEditForm
			{
				post = new PostViewModel(post, allMedias)
			};

			return new ObjectResult(model);
		}



	}
}
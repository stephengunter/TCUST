﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Blog.Models;
using Blog.Services;
using ApplicationCore.Helpers;
using Blog.Views;
using ApplicationCore.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Blog.Helpers;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;
using Permissions.Services;

using Tcust.Services;
using Tcust.Models;
using System.Net.Http;
using IdentityModel.Client;

namespace BlogWeb.Areas.Admin.Controllers
{
	[Authorize(Policy = "EDIT_POSTS")]
	public class PostsController : BaseAdminController
	{
		private readonly IPostService postService;
		private readonly ViewService viewService;

		public PostsController(IHostingEnvironment environment, IOptions<AppSettings> settings, IPermissionService permissionService ,
			IPostService postService) : base(environment, settings, permissionService)
		{
			
			this.postService = postService;
			this.viewService = new ViewService(this.Settings, this.postService); 
		}

		Category defaultCategory;
		Category GetDefaultCategory()
		{
			if (this.defaultCategory != null) return this.defaultCategory;

			this.defaultCategory = postService.GetCategoryByCode("diary");
			return this.defaultCategory;
		}

		[HttpGet]
		public async Task<IActionResult> Index(int category=0 , bool reviewed=true , string terms="" ,string keyword="" , string sortby = "", string sort = "", int page = 1, int pageSize=10 )
		{
			
			Category selectedCategory = null;
			if (category > 0) selectedCategory = await postService.GetCategoryByIdAsync(category);
			if (selectedCategory == null) category = 0;

			var posts = await postService.FetchPosts(selectedCategory, reviewed, keyword);

			if (!String.IsNullOrEmpty(terms))
			{
				var termNumbers= terms.Split(',').ToList().Select(int.Parse).ToList();
				posts = posts.Where(p => termNumbers.Contains(p.TermNumber));
			}

			if (String.IsNullOrEmpty(sortby)) sortby = "date";

			if (String.IsNullOrEmpty(sort)) sort = "desc";

			if (sortby == "date")
			{
				sort = "desc";
				posts = viewService.OrderPosts(posts);
			}
			else
			{
				bool desc = sort != "asc";

				if (desc) posts = posts.OrderByDescending(p => p.Number);
				else posts = posts.OrderBy(p => p.Number);

			}
			

			bool withCategories = true;
			var pageList = await viewService.GetPostPagedList(posts, page, pageSize, withCategories);

			
			if (Request.IsAjaxRequest())
			{
				return new ObjectResult(pageList);
			}

			bool edit = false;
			var categoryOptions = await GetCategoryOptions(edit);

			ViewData["categories"] = this.ToJsonString(categoryOptions);


			ViewData["list"]= this.ToJsonString(pageList);
			

			ViewData["can_delete"] = CanReviewPost().ToInt();

			return View();
		}

		

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			bool canReviewPost = CanReviewPost();
			var post = new PostViewModel()
			{
				reviewed= canReviewPost
			};

			bool edit = true;
			var categoryOptions = await GetCategoryOptions(edit);

			var model = new PostEditForm
			{
				post = post,
				categoryOptions= categoryOptions,
				canReview= canReviewPost.ToInt(),
				issuerIds=new List<int>(),
				departmentIds = new List<int>()

			};
			


			return new ObjectResult(model);
		}

		
		[HttpPost("[area]/[controller]")]
		public async Task<IActionResult> Store([FromBody] PostEditForm model)
		{
			var departmentIds = model.departmentIds;
			var issuerIds = model.issuerIds;
			
			if (issuerIds.IsNullOrEmpty()) ModelState.AddModelError("issuerIds", "請選擇發稿單位");
			if (departmentIds.IsNullOrEmpty()) ModelState.AddModelError("departmentIds", "請選擇活動單位");


			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			

			var post = model.post.MapToEntity(CurrentUserId);

			foreach (var  item in model.post.medias)
			{
				post.Attachments.Add(item.MapToEntity(CurrentUserId));
			}

			if (model.post.categoryIds.IsNullOrEmpty())
			{
				post.Categories.Add(this.GetDefaultCategory());
			}
			else
			{
				foreach (var categoryId in model.post.categoryIds)
				{
					var caregory = await postService.GetCategoryByIdAsync(categoryId);
					post.Categories.Add(caregory);
				}
			}

			
			

			post = await postService.CreateAsync(post);

			await postService.SyncPostDepartments(post, departmentIds);

			await postService.SyncPostIssuers(post, issuerIds);





			return new ObjectResult(post);

		
		}

		[HttpGet("[area]/[controller]/{id}/edit")]
		public async Task<IActionResult> Edit(int id)
		{
			var post = postService.GetById(id);
			if (post == null) return NotFound();

			bool canReviewPost = CanReviewPost();

			bool allMedias = true;
			var model = new PostEditForm
			{
				post = viewService.MapPostViewModel(post, allMedias),
				canReview = canReviewPost.ToInt()
			};

			var categoryIds =await postService.GetCategoryIdsAsync(id);

			model.post.categoryIds = categoryIds;

			model.departmentIds = await postService.GetDepartmentIdsInPostAsync(post);
			model.issuerIds = await postService.GetIssuerIdsInPostAsync(post);

			bool edit = true;
			var categoryOptions = await GetCategoryOptions(edit);

			model.categoryOptions = categoryOptions;

			return new ObjectResult(model);
		}

		[HttpPut("[area]/[controller]/{id}")]
		public async Task<IActionResult> Update(int id , [FromBody] PostEditForm model)
		{
			var departmentIds = model.departmentIds;
			var issuerIds = model.issuerIds;

			if (issuerIds.IsNullOrEmpty()) ModelState.AddModelError("issuerIds", "請選擇發稿單位");
			if (departmentIds.IsNullOrEmpty()) ModelState.AddModelError("departmentIds", "請選擇活動單位");

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var post = postService.GetById(id);
			if (post == null) return NotFound();

			post = model.post.MapToEntity(CurrentUserId, post);

			if (!CanReviewPost()) post.Reviewed = false;

			if (post.Attachments.IsNullOrEmpty()) post.Attachments = new List<UploadFile>();

			foreach (var item in model.post.medias)
			{
				var attachment = post.Attachments.Where(a=>a.Id== item.id).FirstOrDefault();

				if (attachment == null) post.Attachments.Add(item.MapToEntity(CurrentUserId));				
				else attachment = item.MapToEntity(CurrentUserId, attachment);
				
			}

			var categoryIds = new List<int>();

			if (model.post.categoryIds.IsNullOrEmpty())
			{
				categoryIds.Add(this.GetDefaultCategory().Id);
			}
			else
			{
				categoryIds = model.post.categoryIds.ToList();
			}

			await postService.UpdateAsync(post, categoryIds);


			await postService.SyncPostDepartments(post, departmentIds);

			await postService.SyncPostIssuers(post, issuerIds);


			return new ObjectResult(post);


		}



		[Authorize(Policy = "REVIEW_POSTS")]
		[HttpDelete]
		public async Task<IActionResult> Delete(int id)
		{
			
			await postService.DeleteAsync(id, CurrentUserId);

			return new NoContentResult();
			
		}


		private async Task<List<BaseOption>> GetCategoryOptions(bool edit)
		{
			bool emptyOption = !edit;
			var categories = await postService.GetCategoriesAsync();

			categories = categories.Where(c => c.Id != this.GetDefaultCategory().Id);


			return categories.ToOptions(emptyOption);

		}



	}
}
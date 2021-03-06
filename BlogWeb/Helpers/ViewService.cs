﻿using System;
using System.Collections.Generic;
using System.Linq;
using Blog.Models;
using BlogWeb.Models;
using Microsoft.Extensions.Options;
using ApplicationCore.Views;
using ApplicationCore.Paging;
using System.Threading.Tasks;
using Blog.Services;

namespace BlogWeb.Helpers
{
	public static class ExtensionHelpers
	{
		public static List<BaseOption> ToOptions(this IEnumerable<Category> Categories, bool withEmpty=true)
		{
			var options = Categories.Select(c => new BaseOption(c.Id.ToString(), c.Name)).ToList();
			if(withEmpty) options.Insert(0, new BaseOption("","所有分類") );

			return options;
		}

	}



	public class ViewService
    {
		private readonly IOptions<AppSettings> settings;
		private readonly IPostService postService;

		public ViewService(IOptions<AppSettings> settings, IPostService postService)
		{
			this.settings = settings;
			this.postService = postService;
		}

		public PostViewModel MapPostViewModel(Post post, bool allMedias = false)
		{
			var model = new PostViewModel();
			model.id = post.Id;
			model.title = post.Title;
			model.termNumber = post.TermNumber;
			model.number = post.Number;
			model.author = post.Author;
			model.content = post.Content;
			model.reviewed = post.Reviewed;
			model.date = post.Date.ToShortDateString();
			model.createdAt = post.CreatedAt;

			model.url = String.Format("{0}/posts/{1}?title={2}", settings.Value.Url, post.Id , post.Title);

			if (String.IsNullOrEmpty(post.Summary)) model.summary = PostViewModel.GetDefaultSummary(post);
			else model.summary = post.Summary;

			if (allMedias)
			{
				model.medias = new List<MediaViewModel>();
				foreach (var media in post.Attachments.OrderByDescending(a=>a.Order))
				{

					model.medias.Add(MapMediaViewModel(media));
				}
			}
			else
			{
				if (post.TopFile != null) model.cover = MapMediaViewModel(post.TopFile);
			}

			

			return model;
		}

		public MediaViewModel MapMediaViewModel(UploadFile file)
		{
			var model = new MediaViewModel();

			model.id = file.Id;
			model.postId = file.PostId;
			model.name = file.Name;
			model.title = file.Title;
			model.order = file.Order;

			model.width = file.Width;
			model.height = file.Height;
			model.type = file.Type;
			model.path = String.Format("{0}/{1}/{2}", settings.Value.Url , settings.Value.UploadFoler, file.Path);

			if (String.IsNullOrEmpty(file.PreviewPath)) model.previewPath = model.path;
			else model.previewPath = String.Format("{0}/{1}/{2}", settings.Value.Url, settings.Value.UploadFoler, file.PreviewPath);
			

			return model;
		}


		public async Task<PagedList<Post, PostViewModel>> GetPostPagedList(IEnumerable<Post> posts,int page, int pageSize, bool withCategories=false)
		{
			var pageList = new PagedList<Post, PostViewModel>(posts, page, pageSize);

			foreach (var post in pageList.List)
			{
				var postViewModel = MapPostViewModel(post);

				if (withCategories)
				{
					var categories = await postService.GetPostCategoriesAsync(post);
					postViewModel.categoryName = String.Join(",", categories.Select(c => c.Name));
				}
				
				postViewModel.clickCount = await postService.GetPostClickCount(post.Id);

				pageList.ViewList.Add(postViewModel);
			}

			pageList.List = null;

			return pageList;
		}

		


	}
}

﻿using System;
using System.Collections.Generic;

using Blog.Models;
using ApplicationCore.Helpers;
using System.ComponentModel.DataAnnotations;

using ApplicationCore.Views;

namespace BlogWeb.Models
{
	
	public class PostViewModel
	{
		public PostViewModel()
		{

		}

		public int id { get; set; }

		[Required(ErrorMessage = "請填寫標題")]
		public string title { get; set; }

		[Required(ErrorMessage = "請填寫學年標題")]
		public int termNumber { get; set; }

		public string number { get; set; }
		public string author { get; set; }

		[Required(ErrorMessage = "請填寫內容")]
		public string content { get; set; }

		public string summary { get; set; }

		public string date { get; set; }

		public bool reviewed { get; set; }


		public DateTime createdAt { get; set; }

		public int categoryId { get; set; }

		public string categoryName { get; set; }

		public string url { get; set; }

		public int clickCount { get; set; }  

		public MediaViewModel cover { get; set; }

		public List<MediaViewModel> medias { get; set; }

		

		public static string GetDefaultSummary(Post post)
		{
			string str = post.Content.RemoveHtmlTags().Trim();
			return str.Substring(0, Math.Min(str.Length, 100)) + " ...";
		}


		public Post MapToEntity(string updatedBy , Post post=null)
		{
			if (post == null)
			{
				post = new Post();
				post.Attachments = new List<UploadFile>();
			}


			post.Title = title;
			post.Content = content;
			post.TermNumber = termNumber;

			post.Author = author;
			post.Reviewed = reviewed;
			post.Number = number;

			post.Date = date.ToDatetimeOrDefault(DateTime.Now);

			post.SetUpdated(updatedBy);



			return post;
		}



		//copy專用

		public DateTime updatedAt { get; set; }
		public string fileIds { get; set; }

		

	}

	public class PostEditForm
	{
		public int canReview { get; set; }

		public PostViewModel post { get; set; }

		public List<BaseOption> categoryOptions { get; set; }
	}

	public class PostReviewForm
	{
		public IList<int> postIds { get; set; }
	}









}

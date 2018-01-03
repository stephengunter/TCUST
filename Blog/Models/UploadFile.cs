﻿
using System.Text.RegularExpressions;
using ApplicationCore.Entities;
using System;

namespace Blog.Models
{
	public class UploadFile : BsseUploadFile
	{
		public UploadFile()
		{
			CreatedAt = DateTime.Now;
		}

		public int Order { get; set; }

		public int PostId { get; set; }
		public Post Post { get; set; }

		
	}
}

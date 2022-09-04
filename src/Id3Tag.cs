/*
 * Part of the ID3-AutoGen project
 * Copyright (c) 2012-2022 Michael Bemmerl
 *
 * SPDX-License-Identifier: MIT
 */

namespace ID3_AutoGen
{
	using System;
	using CommunityToolkit.Diagnostics;

	/// <summary>
	/// Represents the tagging information for an MP3 file.
	/// </summary>
	class Id3Tag : ICloneable
	{
		ushort? year;

		/// <summary>
		/// Comment tag
		/// </summary>
		public string Comment
		{ get; set; }

		/// <summary>
		/// Album tag
		/// </summary>
		public string Album
		{ get; set; }

		/// <summary>
		/// Year tag
		/// </summary>
		public ushort? Year
		{
			get => this.year;
			set
			{
				// Has to be four bytes long
				if (value.HasValue)
					Guard.IsBetweenOrEqualTo(value.Value, (ushort)1, (ushort)9999, nameof(value));

				this.year = value;
			}
		}

		/// <summary>
		/// Artist tag
		/// </summary>
		public string Artist
		{ get; set; }

		/// <summary>
		/// Title tag
		/// </summary>
		public string Title
		{ get; set; }

		/// <summary>
		/// Genre tag
		/// </summary>
		public Id3Genre? Genre
		{ get; set; }

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			Id3Tag newTag = new Id3Tag();
			newTag.Comment = this.Comment;
			newTag.Album = this.Album;
			newTag.Year = this.Year;
			newTag.Artist = this.Artist;
			newTag.Title = this.Title;
			newTag.Genre = this.Genre;

			return newTag;
		}
	}
}

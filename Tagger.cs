/*
 * Part of the ID3-AutoGen project
 * Copyright (c) 2012-2022 Michael Bemmerl
 *
 * SPDX-License-Identifier: MIT
 */

namespace ID3_AutoGen
{
	using CommunityToolkit.Diagnostics;
	using Id3;
	using Id3.Frames;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Provides methods for tagging MP3 files while automatically detecting ID3 data from the filename.
	/// </summary>
	internal class Tagger
	{
		private readonly Regex regex = new Regex(@"^([\w\s\.\',\+\-&]+?) - ([\(\)\w\s\.\',\-\!&]+)", RegexOptions.Compiled);
		private readonly List<string> filters;

		private readonly Id3Tag id3Tag;

		/// <summary>
		/// TRUE if no files should be changed.
		/// </summary>
		public bool DryRun
		{ get; set; }

		/// <summary>
		/// List of words that should be filtered out of the artist and title tag.
		/// </summary>
		public IList<string> Filters => this.filters;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public Tagger() : this(new Id3Tag())
		{ }

		/// <summary>
		/// Creates a new instance. The <see cref="Tag"/> method uses the predefined data specified in <paramref name="id3Tag"/>.
		/// </summary>
		/// <param name="id3Tag">Predefined ID3 tag data.</param>
		/// <exception cref="NullReferenceException">Predefined ID3 tag is NULL.</exception>
		public Tagger(Id3Tag id3Tag)
		{
			Guard.IsNotNull(id3Tag, nameof(id3Tag));

			this.id3Tag = id3Tag;
			this.filters = new List<string>();
		}

		/// <summary>
		/// Detects the artist and title tag from the filename specified in <paramref name="file"/> and writes that data to the file.
		/// Existing ID3 data will be overwritten.
		/// </summary>
		/// <param name="file">File that should be tagged.</param>
		/// <param name="tagWritten">Resulting ID3 tag.</param>
		/// <returns>TRUE on success.</returns>
		/// <exception cref="NullReferenceException"><see cref="file"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Artist and title could not be detected.</exception>
		public bool Tag(FileInfo file, out Id3Tag tagWritten)
		{
			Guard.IsNotNull(file, nameof(file));

			Match match = this.regex.Match(Path.GetFileNameWithoutExtension(file.Name));

			Guard.IsTrue(match.Success, nameof(match.Success), $"Could not detect artist & title for '{file.FullName}'.");

			string artist = match.Groups[1].Value;
			string title = match.Groups[2].Value;

			artist = filter(artist).Trim();
			title = filter(title).Trim();

			Id3Tag tag = (Id3Tag)this.id3Tag.Clone();

			// Use the artist extracted by the RegEx if it was not set from the caller.
			tag.Artist ??= artist;
			tag.Title = title;

			bool success = writeTag(file, ref tag);

			tagWritten = tag;

			return success;
		}

		/// <summary>
		/// Detects the artist and title tag from the filename specified in <paramref name="file"/> and writes that data to the file.
		/// Existing ID3 data will be overwritten.
		/// </summary>
		/// <param name="file">File that should be tagged.</param>
		/// <returns>TRUE on success.</returns>
		public bool Tag(FileInfo file)
		{
			return Tag(file, out _);
		}

		private string filter(string text)
		{
			if (this.filters.Count == 0)
				return text;

			foreach (string filter in this.filters)
			{
				Guard.IsNotNullOrWhiteSpace(filter, nameof(filter));

				text = Regex.Replace(text, Regex.Escape(filter), string.Empty, RegexOptions.IgnoreCase);
			}

			return text;
		}

		private bool writeTag(FileInfo file, ref Id3Tag tag)
		{
			Guard.IsNotNull(file);
			Guard.IsNotNull(tag);

			tag.Artist = tag.Artist.Trim();
			tag.Title = tag.Title.Trim();

			using Mp3 mp3 = new Mp3(file, Mp3Permissions.ReadWrite);

			Id3.Id3Tag fileTag = new Id3.Id3Tag();

			fileTag.Artists = new ArtistsFrame();
			fileTag.Artists.Value.Add(tag.Artist);
			fileTag.Title = new TitleFrame(tag.Title);

			if (tag.Year.HasValue)
				fileTag.Year = new YearFrame(tag.Year.Value);

			if (tag.Comment != null)
			{
				tag.Comment = tag.Comment.Trim();
				fileTag.Comments.Add(tag.Comment);
			}

			if (tag.Album != null)
			{
				tag.Album = tag.Album.Trim();
				fileTag.Album = new AlbumFrame(tag.Album);
			}

			// Don't write changes to the file if DryRun is active.
			return DryRun || mp3.WriteTag(fileTag, Id3Version.V1X);
		}
	}
}

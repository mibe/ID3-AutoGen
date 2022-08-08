namespace ID3_AutoGen
{
	using CommunityToolkit.Diagnostics;
	using Id3;
	using Id3.Frames;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;

	class Tagger
	{
		private Regex regex = new Regex(@"^([\w\s\.\',\+\-&]+?) - ([\(\)\w\s\.\',\-\!&]+)", RegexOptions.Compiled);
		private List<string> filters;

		private readonly Id3Tag id3Tag;

		public bool DryRun
		{ get; set; }

		public IList<string> Filters => this.filters;

		public Tagger() : this(new Id3Tag())
		{
			this.filters = new List<string>();
		}

		public Tagger(Id3Tag id3Tag)
		{
			Guard.IsNotNull(id3Tag, nameof(id3Tag));

			this.id3Tag = id3Tag;
		}

		public bool Tag(FileInfo file, out Id3Tag tagWritten)
		{
			Guard.IsNotNull(file, nameof(file));

			Match match = this.regex.Match(Path.GetFileNameWithoutExtension(file.Name));

			Guard.IsTrue(match.Success, nameof(match.Success), $"Could not detect artist & title for '{file.FullName}'.");

			string artist = match.Groups[1].Value;
			string title = match.Groups[2].Value;

			artist = filter(artist);
			title = filter(title);

			// FIXME: Copy
			Id3Tag tag = this.id3Tag;

			// Use the artist extracted by the RegEx if it was not set from the caller.
			tag.Artist ??= artist;
			tag.Title = title;

			bool success = writeTag(file, ref tag);

			tagWritten = tag;

			return success;
		}

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

			if (DryRun)
				return true;

			return mp3.WriteTag(fileTag, Id3Version.V1X);
		}
	}
}

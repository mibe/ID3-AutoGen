namespace ID3_AutoGen
{
	using CommunityToolkit.Diagnostics;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	class Tagger
	{
		private Regex regex = new Regex(@"^([\w\s\.\',\+\-&]+?) - ([\(\)\w\s\.\',\-\!&]+)", RegexOptions.Compiled);
		private List<string> filters;

		private readonly Id3Data id3Data;

		public bool DryRun
		{ get; set; }

		public List<string> Filters
		{
			get { return this.filters; }
			set { this.filters = value; }
		}

		public Tagger() : this(new Id3Data())
		{
			this.filters = new List<string>();
		}

		public Tagger(Id3Data id3Data)
		{
			Guard.IsNotNull(id3Data, nameof(id3Data));

			this.id3Data = id3Data;
		}

		public Id3Data Tag(string text)
		{
			Guard.IsNotNullOrWhiteSpace(text, nameof(text));

			Match match = this.regex.Match(text);

			Guard.IsTrue(match.Success, "Oops");

			string artist = match.Groups[1].Value;
			string title = match.Groups[2].Value;

			artist = filter(artist);
			title = filter(title);

			// Use the artist extracted by the RegEx if it was not set from the caller.
			this.id3Data.Artist ??= artist;

			this.id3Data.Title = title;

			return this.id3Data;
		}

		private string filter(string text)
		{
			if (this.filters.Count == 0)
				return text;

			foreach (string filter in this.filters)
				text = Regex.Replace(text, Regex.Escape(filter), string.Empty, RegexOptions.IgnoreCase);

			return text;
		}
	}
}

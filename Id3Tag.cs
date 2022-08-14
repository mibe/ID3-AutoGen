namespace ID3_AutoGen
{
	using System;
	using CommunityToolkit.Diagnostics;

	class Id3Tag : ICloneable
	{
		ushort? year;

		public string Comment
		{ get; set; }

		public string Album
		{ get; set; }

		public ushort? Year
		{
			get => this.year;
			set
			{
				if (value.HasValue)
					Guard.IsBetweenOrEqualTo(value.Value, (ushort)1, (ushort)9999, nameof(value));

				this.year = value;
			}
		}

		public string Artist
		{ get; set; }

		public string Title
		{ get; set; }

		public object Clone()
		{
			Id3Tag newTag = new Id3Tag();
			newTag.Comment = this.Comment;
			newTag.Album = this.Album;
			newTag.Year = this.Year;
			newTag.Artist = this.Artist;
			newTag.Title = this.Title;

			return newTag;
		}
	}
}

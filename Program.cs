namespace ID3_AutoGen
{
	using System;
	using System.Collections.Generic;
	using CommandLine;
	using System.IO;

	class Program
	{
		static void Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<Options>(args);

			if (!(result is NotParsed<Options>))
			{
				Options opt = result.Value;

				if (opt.DryRun)
					Console.WriteLine("Dry run active. No file will be changed.");

				Id3Tag tag = new Id3Tag();
				tag.Album = opt.Album;
				tag.Artist = opt.Artist;
				tag.Comment = opt.Comment;
				tag.Year = opt.Year;

				Tagger tagger = new Tagger(tag);
				tagger.DryRun = opt.DryRun;
				
				foreach(string filter in opt.Filter)
					tagger.Filters.Add(filter);

				DirectoryInfo dir = new DirectoryInfo(opt.Directory);

				if (dir.Exists)
				{
					foreach (FileInfo file in dir.EnumerateFiles(opt.Pattern ?? "*.mp3"))
					{
						processFile(tagger, file, opt.Verbose);
					}
				}
				else
				{
					FileInfo singleFile = new FileInfo(opt.Directory);

					if (singleFile.Exists)
						processFile(tagger, singleFile, opt.Verbose);
					else
						Console.Error.WriteLine("Given file or directory not found.");
				}
			}
		}

		static void processFile(Tagger tagger, FileInfo file, bool verbose)
		{
			bool success = tagger.Tag(file, out var writtenTag);

			if (!success)
				Console.Error.WriteLine("Tagging \"{0}\" failed.", file.Name);

			if (!verbose)
				Console.WriteLine("Tag for \"{0}\" set.", file.Name);
			else
			{
				Console.WriteLine("Tag for \"{0}\" set.", file.Name);
				Console.WriteLine("\tArtist: {0}", writtenTag.Artist);
				Console.WriteLine("\tTitle: {0}", writtenTag.Title);
				Console.WriteLine("\tAlbum: {0}", writtenTag.Album);
				Console.WriteLine("\tYear: {0}", writtenTag.Year);
				Console.WriteLine("\tComment: {0}", writtenTag.Comment);
			}
		}

		class Options
		{
			[Value(0, MetaName = "DIR", Required = true, HelpText = "Directory which contains the MP3 files.")]
			public string Directory
			{ get; set; }

			[Option('c', "comment", HelpText = "Content of 'comment' field.")]
			public string Comment
			{ get; set; }

			[Option('a', "album", HelpText = "Content of 'album' field.")]
			public string Album
			{ get; set; }

			[Option('y', "year", HelpText = "Content of 'year' field.")]
			public ushort? Year
			{ get; set; }

			[Option('p', "pattern", HelpText = "Process only files matching this pattern.")]
			public string Pattern
			{ get; set; }

			[Option("artist", HelpText = "Overwrite artist detection.")]
			public string Artist
			{ get; set; }

			[Option("dry-run", HelpText = "Do not change any file.")]
			public bool DryRun
			{ get; set; }

			[Option("verbose", HelpText = "More detailed output.")]
			public bool Verbose
			{ get; set; }

			[Option("filter", HelpText = "Filter these words from artist / title.")]
			public IEnumerable<string> Filter
			{ get; set; }
		}
	}
}

﻿/*
 * Part of the ID3-AutoGen project
 * Copyright (c) 2012-2022 Michael Bemmerl
 *
 * SPDX-License-Identifier: MIT
 */

namespace ID3_AutoGen
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using CommandLine;
	using System.IO;
	using System.Linq;
	using CommandLine.Text;

	class Program
	{
		static void Main(string[] args)
		{
			Parser parser = new Parser(x => x.HelpWriter = null);

			var result = parser.ParseArguments<Options>(args);

			if (result is NotParsed<Options>)
			{
				// Special case for showing license info or genres
				if (args.Select(y => y.ToLower()).Contains("--license"))
					Console.Write(Properties.Resources.licenses);
				else if (args.Select(y => y.ToLower()).Contains("--genres"))
					displayGenres();
				else
					displayHelp(result);
			}
			else
			{
				Options opt = result.Value;

				if (opt.DryRun)
					Console.WriteLine("Dry run active. No file will be changed.");

				Id3Tag tag = new Id3Tag();
				tag.Album = opt.Album;
				tag.Artist = opt.Artist;
				tag.Comment = opt.Comment;
				tag.Year = opt.Year;
				tag.Genre = opt.Genre;

				Tagger tagger = new Tagger(tag);
				tagger.DryRun = opt.DryRun;
				tagger.RemoveId3V2Tags = opt.RemoveId3V2Tags;

				foreach (string filter in opt.Filter)
					tagger.Filters.Add(filter);

				DirectoryInfo dir = new DirectoryInfo(opt.Directory);

				// Check if the given path is a file or a directory.
				if (dir.Exists)
				{
					// It's a directory, so enumerate all files matching the given pattern or default to *.mp3.
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
				Console.Error.WriteLine("Tagging of \"{0}\" failed.", file.Name);

			if (!verbose)
				Console.WriteLine("Tag for \"{0}\" set.", file.Name);
			else
			{
				Console.WriteLine("Tag for \"{0}\" set:", file.Name);
				Console.WriteLine("\tArtist: {0}", writtenTag.Artist);
				Console.WriteLine("\tTitle: {0}", writtenTag.Title);
				Console.WriteLine("\tAlbum: {0}", writtenTag.Album);
				Console.WriteLine("\tYear: {0}", writtenTag.Year);
				Console.WriteLine("\tComment: {0}", writtenTag.Comment);
				Console.WriteLine("\tGenre: {0}", writtenTag.Genre.HasValue ? writtenTag.Genre.Value.ToString() : "None");
			}
		}

		static void displayHelp(ParserResult<Options> result)
		{
			var helpText = HelpText.AutoBuild(result, h =>
			{
				h.MaximumDisplayWidth = Console.WindowWidth;
				h.Heading += Environment.NewLine + "This program is free software.";
				h.Copyright = "Copyright (c) 2012-2022 Michael Bemmerl" + Environment.NewLine;
				h.Copyright += "This program uses CommandLineParser, ID3.NET and CommunityToolkit. Use the license switch for more information.";
				return HelpText.DefaultParsingErrorsHandler(result, h);
			}, e => e);

			Console.Error.WriteLine(helpText);
		}

		static void displayGenres()
		{
			Console.WriteLine("Available genres are (sorted):");
			Console.WriteLine();

			IEnumerable<string> genres = Enum.GetNames(typeof(Id3Genre)).ToList();

			foreach (string genre in genres.OrderBy(x => x))
			{
				Console.Write("\t");
				Console.WriteLine(genre);
			}
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		class Options
		{
			[Value(0, MetaName = "DIR", Required = true, HelpText = "Directory which contains the MP3 files or path to a single MP3 file.")]
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

			[Option('g', "genre", HelpText = "Genre of the song.")]
			public Id3Genre? Genre
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

			[Option("remove-id3v2", HelpText = "Remove ID3v2 tags.")]
			public bool RemoveId3V2Tags
			{ get; set; }

			[Option("license", HelpText = "Display license information.")]
			public bool ShowLicense
			{ get; set; }

			[Option("genres", HelpText = "Display available genres.")]
			public bool ShowGenres
			{ get; set; }

			[Usage()]
			public static IEnumerable<Example> Examples
			{
				get
				{
					yield return new Example("Tag all files in the specified directory. Set the album and year tag accordingly",
						new Options { Directory = "C:\\Music\\Metal", Album = "Master of Puppets", Year = 1986 });
					yield return new Example("Tag a single file and set a comment tag. The artist detection is overwritten",
						new Options { Directory = "~/woodstock/Jimi - Hey Joe.mp3", Artist = "Jimi Hendrix", Comment = "Bootleg at Woodstock" });
				}
			}
		}
	}
}

ID3-AutoGen
===========
A program for generating ID3v1 tags from an MP3 filename.

For example, the file "MyBand0815 - Freaking Great Song.mp3" would result in a
ID3v1 tag with "MyBand0815" in the 'artist' field and "Freaking Great Song"
in the 'title' field.

Usage
-----
    ID3-AutoGen 2.0.0
    This program is free software.
    Copyright (c) 2012-2022 Michael Bemmerl
    This program uses CommandLineParser, ID3.NET and CommunityToolkit. Use the license switch for more information.

    USAGE:
    Tag all files in the specified directory. Set the album and year tag accordingly:
      ID3-AutoGen --album "Master of Puppets" --year 1986 C:\Music\Metal
    Tag a single file and set a comment tag. The artist detection is overwritten:
      ID3-AutoGen --artist "Jimi Hendrix" --comment "Bootleg at Woodstock" "~/woodstock/Jimi - Hey Joe.mp3"

      -c, --comment    Content of 'comment' field.

      -a, --album      Content of 'album' field.

      -y, --year       Content of 'year' field.

      -p, --pattern    Process only files matching this pattern.

      --artist         Overwrite artist detection.

      --dry-run        Do not change any file.

      --verbose        More detailed output.

      --filter         Filter these words from artist / title.

      --license        Display license information.

      --help           Display this help screen.

      --version        Display version information.

      DIR (pos. 0)     Required. Directory which contains the MP3 files.

Requirements
-----
* .NET Core 3.1 runtime
* 64 bit Operating System
* The filenames have to be in ```Artist - Title``` pattern.

Tested on Windows 7 and Debian 11.

Example
-----
    ID3-AutoGen -y 2019 -a "Night of the Proms" -c "Live on BR" --verbose --filter (live) D:\Music
This will process all ```.mp3``` files in the directory ```D:\Music```. The value ```Night of the Proms``` will be used as content for the album tag and ```2019``` for the year tag; the comment tag will contain ```Live on BR```. This will be used on all files processed. The command will list all tags on all files due to ```--verbose```. The text "```(live)```" will be filtered out of the artist and title tag, if the filename does contain it.

Used libraries
-----
* [CommandLineParser](https://github.com/commandlineparser/commandline) (© 2005-2015 Giacomo Stelluti Scala & Contributors; MIT license)
* [ID3.NET](https://github.com/JeevanJames/Id3) (© 2005-2019 Jeevan James; Apache 2.0 license)
* [CommunityToolkit](https://github.com/CommunityToolkit/dotnet) (© .NET Foundation and Contributors; MIT license)

License
-----
MIT License (see COPYING)

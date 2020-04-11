id3-autogen
===========

Simple script for generating ID3v1 tags from filename

For example, the file "MyBand0815 - Freaking Great Song.mp3" would result in a
ID3v1 tag with "MyBand0815" in the 'artist' field and "Freaking Great Song"
in the 'songname' field.

Usage
-----
    id3-autogen.py [-h] [-c COMMENT] [-a ALBUM] [-y YEAR] [-p PATTERN] DIR

    Simple script for generating ID3v1 tags from filename

    positional arguments:
      DIR                   Directory which contains the MP3 files.

    optional arguments:
      -h, --help            show this help message and exit
      -c COMMENT, --comment COMMENT
                            Content of 'comment' field.
      -a ALBUM, --album ALBUM
                            Content of 'album' field.
      -y YEAR, --year YEAR  Content of 'year' field.
      -p PATTERN, --pattern PATTERN
                            Process only files matching this pattern.
      --artist ARTIST       Overwrite artist detection.
      --verbose             More detailed output.
      --filter FILTER [FILTER ...]
                            Filter this words from artist / title.
Requirements
-----

* Python (well, obviously ;-)
* mp3-tagger (https://pypi.org/project/mp3-tagger/)

Tested with Python 3.8.2 & mp3-tagger 1.0.

Installation
-----
Run ```pip install -r requirements.txt``` to install the dependencies with pip.

License
-----

MIT License (see COPYING)

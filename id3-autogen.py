#!/usr/bin/python

""" Simple script for generating ID3v1 tags from filename patterns.
For example, the file "MyBand0815 - Freaking Great Song.mp3" would result in a
ID3v1 tag with "MyBand0815" in the 'artist' field and "Freaking Great Song"
in the 'songname' field.

Copyright: (C) 2012-2015 Michael Bemmerl
License: MIT License (see COPYING)

Requirements:
- Python (well, obviously ;-)
- pytagger (http://www.liquidx.net/pytagger/)

Tested with Python 2.7.2 & pytagger 0.5.
"""

from tagger import *
import argparse, os, fnmatch, re

parser = argparse.ArgumentParser(description="Simple script for generating ID3v1 tags from filename")
parser.add_argument('DIR', help="Directory which contains the MP3 files.")
parser.add_argument('-c', '--comment', help="Content of 'comment' field.")
parser.add_argument('-a', '--album', help="Content of 'album' field.")
parser.add_argument('-y', '--year', help="Content of 'year' field.")
parser.add_argument('-p', '--pattern', help="Process only files matching this pattern.")
parser.add_argument('--artist', help="Overwrite artist detection.")
parser.add_argument('--dry-run', action='store_true', help="Do not change any file.")
parser.add_argument('--verbose', action='store_true', help="More detailed output.")
parser.add_argument('--filter', nargs='+', help='Filter these words from artist / title.')

args = parser.parse_args()
dir = args.DIR

if args.filter is None:
    filter_regex = None
else:
    filter_regex = re.compile('|'.join(map(re.escape, args.filter)))

def set_file_fields(path, artist, title):
    """ Set the ID3v1 tag with the field data """
    directory, filename = os.path.split(path)

    id3 = ID3v1(path)
    id3.artist = artist
    id3.songname = title

    # Set additional fields, if available
    if args.comment is not None:
        id3.comment = args.comment
    if args.album is not None:
        id3.album = args.album
    if args.year is not None:
        id3.year = args.year

    if not args.dry_run:
        id3.commit()

    if not args.verbose:
        print "Tag for \"%s\" set." % filename
    else:
        print "Tag for \"%s\" set:%s\tArtist: '%s'%s\tTitle: '%s'" % (filename, os.linesep, artist, os.linesep, title)
        print "\tAlbum: '%s'%s\tYear: '%s'%s\tComment: '%s'" % (id3.album, os.linesep, id3.year, os.linesep, id3.comment)


def get_artist_title(path):
    """ Return artist & title information from filename """
    directory, filename = os.path.split(path)
    name, extension = os.path.splitext(filename)

    # Splitting out artist & title with regular expression
    result = re.search("^([\w\s\.\'\+\-&]+) - ([\(\)\w\s\.\',\-\!]+)", name)

    if result is None:
        raise ValueError("Could not detect artist & title for '%s'." % filename)
    else:
        artist = result.group(1)
        title = result.group(2)
        if filter_regex is not None:
            artist = filter_regex.sub('', artist)
            title = filter_regex.sub('', title)
        return artist.strip(), title.strip()


def do_file(path):
    """ Process the file given in the argument """
    try:
        artist, title = get_artist_title(path)

        # Check if artist is overwritten from the command line
        if args.artist is not None:
            artist = args.artist

        set_file_fields(path, artist, title)
    except ValueError, e:
        print str(e)
    except ID3Exception, e:
        print "ID3v1 exception '%s' while working with %s" % (str(e), filename)

# Check if it's a file or a directory
if os.path.isdir(dir) is False:
    do_file(dir)
else:
    pattern = "*.mp3"
    if args.pattern is not None:
        pattern = args.pattern

    for filename in fnmatch.filter(os.listdir(dir), pattern):
        path = os.path.join(dir, filename)
        if os.path.isfile(path):
            do_file(path)
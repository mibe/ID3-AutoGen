#!/usr/bin/python

""" Simple script for generating ID3v1 tags from filename patterns.
For example, the file "MyBand0815 - Freaking Great Song.mp3" would result in a
ID3v1 tag with "MyBand0815" in the 'artist' field and "Freaking Great Song"
in the 'songname' field.

Copyright: (C) 2012 Michael Bemmerl
License: MIT License (see COPYING)

Requirements:
- Python (well, obvious ;-)
- pytagger (http://www.liquidx.net/pytagger/)

Tested with Python 2.7.2 & pytagger 0.5.
"""

from tagger import *
import argparse, os, fnmatch

parser = argparse.ArgumentParser(description="Simple script for generating ID3v1 tags from filename")
parser.add_argument('DIR', help="Directory which contains the MP3 files.")
parser.add_argument('-c', '--comment', help="Content of 'comment' field.")
parser.add_argument('-a', '--album', help="Content of 'album' field.")
parser.add_argument('-y', '--year', help="Content of 'year' field.", type=int)

args = parser.parse_args()
dir = args.DIR

def set_file_fields(path, artist, title):
    directory, filename = os.path.split(path)

    try:
        id3 = ID3v1(path)
        id3.artist = artist
        id3.songname = title

        if args.comment is not None:
            id3.comment = args.comment
        if args.album is not None:
            id3.album = args.album
        if args.year is not None:
            id3.year = year

        id3.commit()

        print "Tag for %s set." % filename

    except ID3Exception, e:
        print "ID3v1 exception '%s' while working with %s" % (str(e), filename)

if not os.path.isdir(dir):
    set_file_fields(dir, "Testinter", "Testtitl")
else:
    for filename in fnmatch.filter(os.listdir(dir), '*.mp3'):
        path = os.path.join(dir, filename)
        if os.path.isfile(path):
            set_file_fields(path, "Testidir", "Testtdir")
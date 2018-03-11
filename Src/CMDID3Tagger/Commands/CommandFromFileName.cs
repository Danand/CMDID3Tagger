﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using TagLib;

using static CMDID3Tagger.Constants;

namespace CMDID3Tagger.Commands
{
    public sealed class CommandFromFileName : CommandBase
    {
        private const string COMMAND_STRING = "fromfilename";

        public override string CommandString
        {
            get { return COMMAND_STRING; }
        }

        public override void Execute(string[] args)
        {
            if (ArgsParser.Parse(args) == Args.PathAndString)
                ChangeTags(args[0], args[1]);
            else
                throw new ArgumentException();
        }

        private static void ChangeTags(string path, string pattern)
        {
            if (Path.HasExtension(path))
                ChangeTagsInFile(path, pattern);
            else
                ChangeTagsInDirectory(path, pattern);
        }

        private static void ChangeTagsInDirectory(string path, string pattern)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory '{path}' doesn't exist! Check it and try again.");
                return;
            }

            var trackPaths = Directory.GetFiles(path);

            foreach (var trackPath in trackPaths)
                ChangeTagsInFile(trackPath, pattern);
        }

        private static void ChangeTagsInFile(string path, string pattern)
        {
            var tagPlaceholdersMatches = Regex.Matches(pattern, "(%([^%]*)%)");

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"File '{path}' doesn't exist! Check it and try again.");
                return;
            }

            if (tagPlaceholdersMatches.Count == 0)
            {
                Console.WriteLine($"Invalid pattern '{pattern}'! Check it and try again.");
                return;
            }

            TagLib.File tagLibFile;

            try
            {
                tagLibFile = TagLib.File.Create(path);

                if (tagLibFile.Properties.MediaTypes != MediaTypes.Audio)
                {
                    Console.WriteLine($"'{path}' is not audio! Skipping it...");
                    return;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{nameof(TagLib.File)} for '{path}' doesn't created!");
                return;
            }

            RemoveAllTags(path);

            using (tagLibFile)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);

                if (fileName == null)
                    throw new NullReferenceException();

                var tagPattern = pattern;

                tagPattern = Regex.Escape(tagPattern);

                foreach (Match placeholderMatch in tagPlaceholdersMatches)
                    tagPattern = tagPattern.Replace(placeholderMatch.Groups[1].Value, $"(?<{placeholderMatch.Groups[2].Value}>.+)");

                var tagMatch = Regex.Match(fileName, tagPattern);

                foreach (Match placeholderMatch in tagPlaceholdersMatches)
                {
                    var tagName = placeholderMatch.Groups[2].Value;

                    AssignTag(
                        tagLibFile: tagLibFile,
                        name:       tagName,
                        value:      tagMatch.Groups[tagName].Value);
                }

                tagLibFile.Save();
            }
        }

        private static void RemoveAllTags(string path)
        {
            using (var tagLibFile = TagLib.File.Create(path))
            {
                tagLibFile.RemoveTags(TagTypes.AllTags);
                tagLibFile.Save();
            }
        }

        private static void AssignTag(TagLib.File tagLibFile, string name, string value)
        {
            // Tag names are case-insensitive:
            name = name.ToLower();

            switch (name)
            {
                case TRACK_KEY:
                    tagLibFile.Tag.Track = uint.Parse(value);
                    break;

                case ARTIST_KEY:
                    tagLibFile.Tag.Performers = new[] { value };
                    break;

                case ALBUM_ARTIST_KEY:
                    tagLibFile.Tag.AlbumArtists = new[] { value };
                    break;

                case TITLE_KEY:
                    tagLibFile.Tag.Title = value;
                    break;

                case ALBUM_KEY:
                    tagLibFile.Tag.Album = value;
                    break;

                case YEAR_KEY:
                    tagLibFile.Tag.Year = uint.Parse(value);
                    break;

                default:
                    Console.WriteLine($"Unrecognized tag '{name}' with value '{value}'!");
                    break;
            }

            Console.WriteLine($"'{value}' {name} tag added to '{tagLibFile.Name}'.");
        }
    }
}
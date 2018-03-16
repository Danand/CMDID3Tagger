﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using TagLib;

using File = System.IO.File;
using TagLibFile = TagLib.File;

namespace CMDID3Tagger.Commands
{
    internal sealed class CommandFromTags : CommandBase
    {
        private const string COMMAND_STRING = "fromtags";

        public override string CommandString
        {
            get { return COMMAND_STRING; }
        }

        public override void Execute(string[] args)
        {
            if (ArgsParser.Parse(args) == Args.PathAndStringAndPath)
                RenameFiles(args[0], args[1], args[2]);
            else if (ArgsParser.Parse(args) == Args.PathAndString)
                RenameFiles(args[0], args[1]);
            else
                throw new ArgumentException();
        }

        private static void RenameFiles(string path, string pattern)
        {
            var resultDirectory
                = File.GetAttributes(path).HasFlag(FileAttributes.Directory)
                    ? path
                    : Path.GetDirectoryName(path);

            if (Path.HasExtension(path))
                RenameFile(path, pattern, resultDirectory);
            else
                RenameFilesInDirectory(path, pattern, resultDirectory);
        }

        private static void RenameFiles(string path, string pattern, string resultDirectory)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                RenameFilesInDirectory(path, pattern, resultDirectory);
            else
                RenameFile(path, pattern, resultDirectory);
        }

        private static void RenameFilesInDirectory(string path, string pattern, string resultDirectory)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory '{path}' doesn't exist! Check it and try again.");
                return;
            }

            var trackPaths = Directory.GetFiles(path);

            foreach (var trackPath in trackPaths)
                RenameFile(trackPath, pattern, resultDirectory);
        }

        private static void RenameFile(string path, string pattern, string resultDirectory)
        {
            var tagPlaceholdersMatches = Regex.Matches(pattern, "(%([^%]*)%)");

            if (!File.Exists(path))
            {
                Console.WriteLine($"File '{path}' doesn't exist! Check it and try again.");
                return;
            }

            if (tagPlaceholdersMatches.Count == 0)
            {
                Console.WriteLine($"Invalid pattern '{pattern}'! Check it and try again.");
                return;
            }

            TagLibFile tagLibFile;

            try
            {
                tagLibFile = TagLibFile.Create(path);

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

            var newFileName = pattern;

            using (tagLibFile)
            {
                foreach (Match placeholderMatch in tagPlaceholdersMatches)
                {
                    var tagValuePlaceholder = placeholderMatch.Groups[1].Value;
                    var tagName = placeholderMatch.Groups[2].Value;
                    var tagValue = TagPropertyWrapper.GetTagValue(tagLibFile, tagName);

                    tagValue = FilePath.ReplaceInvalidFileNameChars(tagValue, "-");

                    newFileName = newFileName.Replace(tagValuePlaceholder, tagValue);
                }
            }

            resultDirectory = resultDirectory.TrimEnd('/').TrimEnd('\\');

            var extension = Path.GetExtension(path);
            var newFilePath = $"{resultDirectory}\\{newFileName}{extension}";

            var newDirectory = Path.GetDirectoryName(newFilePath);

            if (newDirectory != null && !Directory.Exists(newDirectory))
                Directory.CreateDirectory(newDirectory);

            if (File.Exists(newFilePath))
                File.Delete(newFilePath);

            File.Move(path, newFilePath);
        }
    }
}
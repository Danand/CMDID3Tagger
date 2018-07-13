using System;
using System.IO;
using System.Text.RegularExpressions;

using CMDID3Tagger.Interfaces;
using CMDID3Tagger.Utils;

using TagLib;

using File = System.IO.File;
using TagLibFile = TagLib.File;

namespace CMDID3Tagger.Commands
{
    public sealed class CommandFromTags : ICommand
    {
        private readonly ITagPropertyEditor tagPropertyEditor;

        public CommandFromTags(ITagPropertyEditor tagPropertyEditor)
        {
            this.tagPropertyEditor = tagPropertyEditor;
        }

        void ICommand.Execute(params string[] args)
        {
            if (args.Length == 3)
                RenameFiles(args[0], args[1], args[2]);
            else if (args.Length == 2)
                RenameFiles(args[0], args[1]);
            else
                throw new ArgumentException();
        }

        private void RenameFiles(string path, string pattern)
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

        private void RenameFiles(string path, string pattern, string resultDirectory)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                RenameFilesInDirectory(path, pattern, resultDirectory);
            else
                RenameFile(path, pattern, resultDirectory);
        }

        private void RenameFilesInDirectory(string path, string pattern, string resultDirectory)
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory '{path}' doesn't exist!");

            var trackPaths = Directory.GetFiles(path);

            foreach (var trackPath in trackPaths)
                RenameFile(trackPath, pattern, resultDirectory);
        }

        private void RenameFile(string path, string pattern, string resultDirectory)
        {
            var tagPlaceholdersMatches = Regex.Matches(pattern, "(%([^%]*)%)");

            if (!File.Exists(path))
                throw new Exception($"File '{path}' doesn't exist!");

            if (tagPlaceholdersMatches.Count == 0)
                throw new Exception($"Invalid pattern '{pattern}'!");

            TagLibFile tagLibFile;

            try
            {
                tagLibFile = TagLibFile.Create(path);

                if (tagLibFile.Properties.MediaTypes != MediaTypes.Audio)
                    throw new Exception($"'{path}' is not audio! Skipping it...");

            }
            catch (Exception exception)
            {
                throw new Exception($"{nameof(TagLib.File)} for '{path}' doesn't created!", exception);
            }

            var newFileName = pattern;

            using (tagLibFile)
            {
                foreach (Match placeholderMatch in tagPlaceholdersMatches)
                {
                    var tagValuePlaceholder = placeholderMatch.Groups[1].Value;
                    var tagName = placeholderMatch.Groups[2].Value;
                    var tagValue = tagPropertyEditor.GetTagValue(tagLibFile, tagName);

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
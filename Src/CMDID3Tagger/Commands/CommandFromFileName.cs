using System;
using System.IO;
using System.Text.RegularExpressions;

using CMDID3Tagger.Interfaces;

using TagLib;

using File = System.IO.File;
using TagLibFile = TagLib.File;

namespace CMDID3Tagger.Commands
{
    public sealed class CommandFromFileName : ICommand
    {
        private readonly ITagPropertyEditor tagPropertyEditor;

        public CommandFromFileName(ITagPropertyEditor tagPropertyEditor)
        {
            this.tagPropertyEditor = tagPropertyEditor;
        }

        void ICommand.Execute(params string[] args)
        {
            if (args.Length == 2)
                ChangeTags(args[0], args[1]);
            else
                throw new ArgumentException();
        }

        private void ChangeTags(string path, string pattern)
        {
            if (Path.HasExtension(path))
                ChangeTagsInFile(path, pattern);
            else
                ChangeTagsInDirectory(path, pattern);
        }

        private void ChangeTagsInDirectory(string path, string pattern)
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory '{path}' doesn't exist! Check it and try again.");

            var trackPaths = Directory.GetFiles(path);

            foreach (var trackPath in trackPaths)
                ChangeTagsInFile(trackPath, pattern);
        }

        private void ChangeTagsInFile(string path, string pattern)
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

                    tagPropertyEditor.AssignTag(
                        tagLibFile: tagLibFile,
                        name:       tagName,
                        value:      tagMatch.Groups[tagName].Value);
                }

                tagLibFile.Save();
            }
        }

        private void RemoveAllTags(string path)
        {
            using (var tagLibFile = TagLibFile.Create(path))
            {
                tagLibFile.RemoveTags(TagTypes.AllTags);
                tagLibFile.Save();
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace CMDID3Tagger.Tests
{
    [TestFixture]
    public class TaggerTests
    {
        [TestCase(
            "fromtags",
            "sample.mp3",
            "%artist%\\%year% - %album%\\%track% - %title%",
            "Unknown Artist\\2018 - Untitled\\1 - Untitled.mp3")]
        [TestCase(
            "fromtags",
            "sample.mp3",
            "%artist%\\%album% (%year%)\\%track%.%title%",
            "Unknown Artist\\Untitled (2018)\\1.Untitled.mp3")]
        [TestCase(
            "fromtags",
            "sample.mp3",
            "%artist% - %title%",
            "Unknown Artist - Untitled.mp3")]
        public void Start_FromTags_File_ToUnspecifiedDirectory(
            string  command,
            string  fileName,
            string  pattern,
            string  expectedResultPath)
        {
            var applicationDirectory = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
            var projectPath = Directory.GetParent(applicationDirectory).Parent.FullName;
            var samplePath = Path.Combine(projectPath, "Data\\", fileName);
            var sampleCopyPath = $"{applicationDirectory}\\{fileName}";

            if (File.Exists(sampleCopyPath))
                File.Delete(sampleCopyPath);

            File.Copy(samplePath, sampleCopyPath);

            Tagger.Start(new [] { command, sampleCopyPath, pattern });

            var expectedFilePath = $"{applicationDirectory}\\{expectedResultPath}";
            var expectedFileExists = File.Exists(expectedFilePath);

            var expectedTopDirectory = Directory.GetDirectories(applicationDirectory).FirstOrDefault(dir => dir.EndsWith(expectedResultPath.Split('\\').First()));

            if (expectedTopDirectory != null)
            {
                Directory.Delete(expectedTopDirectory, true);
            }
            else if (expectedFileExists)
            {
                File.Delete(expectedFilePath);
            }

            Assert.IsTrue(expectedFileExists);
        }

        [TestCase(
            "fromtags",
            "sample.mp3",
            "%album%/%artist% - %title%",
            "Untitled\\Unknown Artist - Untitled.mp3")]
        [TestCase(
            "fromtags",
            "sample.mp3",
            "%artist% - %title%",
            "Unknown Artist - Untitled.mp3")]
        public void Start_FromTags_Directory_ToUnspecifiedDirectory(
            string  command,
            string  sampleFileName, // It's not argument for command.
            string  pattern,
            string  expectedResultPath)
        {
            var applicationDirectory = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
            var projectPath = Directory.GetParent(applicationDirectory).Parent.FullName;
            var samplePath = Path.Combine(projectPath, "Data\\", sampleFileName);
            var sampleCopyPath = $"{applicationDirectory}\\{sampleFileName}";

            if (File.Exists(sampleCopyPath))
                File.Delete(sampleCopyPath);

            File.Copy(samplePath, sampleCopyPath);

            Tagger.Start(new [] { command, applicationDirectory, pattern });

            var expectedFilePath = $"{applicationDirectory}\\{expectedResultPath}";
            var expectedFileExists = File.Exists(expectedFilePath);

            var expectedTopDirectory = Directory.GetDirectories(applicationDirectory).FirstOrDefault(dir => dir.EndsWith(expectedResultPath.Split('\\').First()));

            if (expectedTopDirectory != null)
            {
                Directory.Delete(expectedTopDirectory, true);
            }
            else if (expectedFileExists)
            {
                File.Delete(expectedFilePath);
            }

            Assert.IsTrue(expectedFileExists);
        }
    }
}
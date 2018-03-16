using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace CMDID3Tagger.Tests
{
    [TestFixture]
    public class TaggerTests
    {
        private const string SAMPLE_FOLDER = "TestSamples";

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
            var sampleDirectory = $"{applicationDirectory}\\{SAMPLE_FOLDER}";
            var sampleCopyPath = $"{sampleDirectory}\\{fileName}";
            var expectedFilePath = $"{sampleDirectory}\\{expectedResultPath}";

            if (!Directory.Exists(sampleDirectory))
                Directory.CreateDirectory(sampleDirectory);

            if (File.Exists(sampleCopyPath))
                File.Delete(sampleCopyPath);

            File.Copy(samplePath, sampleCopyPath);

            Tagger.Start(new [] { command, sampleCopyPath, pattern });

            var createdFilePath = Directory.GetFiles(sampleDirectory, "*.*", SearchOption.AllDirectories).FirstOrDefault();

            Directory.Delete($"{sampleDirectory}", true);

            Assert.AreEqual(expectedFilePath, createdFilePath);
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
            var sampleDirectory = $"{applicationDirectory}\\{SAMPLE_FOLDER}";
            var sampleCopyPath = $"{sampleDirectory}\\{sampleFileName}";
            var expectedFilePath = $"{sampleDirectory}\\{expectedResultPath}";

            if (!Directory.Exists(sampleDirectory))
                Directory.CreateDirectory(sampleDirectory);

            if (File.Exists(sampleCopyPath))
                File.Delete(sampleCopyPath);

            File.Copy(samplePath, sampleCopyPath);

            Tagger.Start(new [] { command, sampleDirectory, pattern });

            var createdFilePath = Directory.GetFiles(sampleDirectory, "*.*", SearchOption.AllDirectories).FirstOrDefault();

            Directory.Delete($"{sampleDirectory}", true);

            Assert.AreEqual(expectedFilePath, createdFilePath);
        }
    }
}
using CMDID3Tagger.Commands;
using CMDID3Tagger.Interfaces;
using CMDID3Tagger.Outputs;

using MicroResolver;
using NUnit.Framework;

namespace CMDID3Tagger.Tests
{
    [TestFixture]
    public class CommandFromTagsTests
    {
        [TestCase(
            "sample.mp3",
            "%artist%\\%year% - %album%\\%track% - %title%",
            "Unknown Artist\\2018 - Untitled\\1 - Untitled.mp3")]
        [TestCase(
            "sample.mp3",
            "%artist%\\%album% (%year%)\\%track%.%title%",
            "Unknown Artist\\Untitled (2018)\\1.Untitled.mp3")]
        [TestCase(
            "sample.mp3",
            "%artist% - %title%",
            "Unknown Artist - Untitled.mp3")]
        public void Execute_File_ToUnspecifiedDirectory(
            string  fileName,
            string  pattern,
            string  expectedResultPath)
        {
            var sample = new Sample(fileName, expectedResultPath);
            sample.Initialize();

            var resolver = ObjectResolver.Create();

            resolver.Register<ICommand, CommandFromTags>(Lifestyle.Singleton);
            resolver.Register<ITagPropertyEditor, TagPropertyEditor>(Lifestyle.Singleton);
            resolver.Register<IOutput, OutputDebug>(Lifestyle.Singleton);

            resolver.Compile();

            var command = resolver.Resolve<ICommand>();

            command.Execute(sample.DummyFilePath, pattern);

            var actualFilePath = sample.GetFirstFilePath();

            sample.DeleteDirectory();

            Assert.AreEqual(sample.ExpectedFilePath, actualFilePath);
        }

        [TestCase(
            "sample.mp3",
            "%album%/%artist% - %title%",
            "Untitled\\Unknown Artist - Untitled.mp3")]
        [TestCase(
            "sample.mp3",
            "%artist% - %title%",
            "Unknown Artist - Untitled.mp3")]
        public void Execute_Directory_ToUnspecifiedDirectory(
            string  sampleFileName, // This argument is not for command.
            string  pattern,
            string  expectedResultPath)
        {
            var sample = new Sample(sampleFileName, expectedResultPath);
            sample.Initialize();

            var resolver = ObjectResolver.Create();

            resolver.Register<ICommand, CommandFromTags>(Lifestyle.Singleton);
            resolver.Register<ITagPropertyEditor, TagPropertyEditor>(Lifestyle.Singleton);
            resolver.Register<IOutput, OutputDebug>(Lifestyle.Singleton);

            resolver.Compile();

            var command = resolver.Resolve<ICommand>();

            command.Execute(sample.Directory, pattern);

            var actualFilePath = sample.GetFirstFilePath();

            sample.DeleteDirectory();

            Assert.AreEqual(sample.ExpectedFilePath, actualFilePath);
        }
    }
}
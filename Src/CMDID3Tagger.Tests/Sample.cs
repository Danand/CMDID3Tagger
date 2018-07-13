using System;
using System.IO;
using System.Linq;

namespace CMDID3Tagger.Tests
{
    internal class Sample
    {
        private readonly string fileName;
        private readonly string expectedResultPath;
        private const string SAMPLE_FOLDER = "TestSamples";

        public Sample(string fileName, string expectedResultPath)
        {
            this.fileName = fileName;
            this.expectedResultPath = expectedResultPath;
        }

        public string Directory { get; private set; }

        public string DummyFilePath { get; private set; }

        public object ExpectedFilePath { get; private set; }

        public void Initialize()
        {
            var applicationDirectory = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
            var projectPath = System.IO.Directory.GetParent(applicationDirectory).Parent.FullName;
            var samplePath = Path.Combine(projectPath, "Data\\", fileName);

            Directory = $"{applicationDirectory}\\{SAMPLE_FOLDER}";
            DummyFilePath = $"{Directory}\\{fileName}";
            ExpectedFilePath = $"{Directory}\\{expectedResultPath}";

            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);

            if (File.Exists(DummyFilePath))
                File.Delete(DummyFilePath);

            File.Copy(samplePath, DummyFilePath);
        }

        public void DeleteDirectory()
        {
            System.IO.Directory.Delete(Directory, true);
        }

        public string GetFirstFilePath()
        {
           return System.IO.Directory.GetFiles(Directory, "*.*", SearchOption.AllDirectories).FirstOrDefault();
        }
    }
}
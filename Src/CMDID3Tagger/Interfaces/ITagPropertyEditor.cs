using TagLib;

namespace CMDID3Tagger.Interfaces
{
    public interface ITagPropertyEditor
    {
        void AssignTag(File tagLibFile, string name, string value);
        string GetTagValue(File tagLibFile, string name);
    }
}
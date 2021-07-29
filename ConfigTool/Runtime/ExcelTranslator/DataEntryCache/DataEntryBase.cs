using System;

namespace NilanToolkit.ConfigTool
{
    [Serializable]
    public abstract class DataEntryBase
    {
        public string KEY;
        public abstract void DeSerialized(ExcelTranslatorBuffer buffer);
    }
}

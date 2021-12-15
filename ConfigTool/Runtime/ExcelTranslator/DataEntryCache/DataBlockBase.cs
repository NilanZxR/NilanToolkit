using System;

namespace NilanToolkit.ConfigTool
{
    [Serializable]
    public abstract class DataBlockBase
    {
        public string KEY;
        public abstract void DeSerialized(ExcelTranslatorBuffer buffer);
    }
}

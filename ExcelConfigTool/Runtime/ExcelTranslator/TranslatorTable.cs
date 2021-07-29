using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace NilanToolKit.ConfigTool
{
    /// <summary>
    /// 中转数据表
    /// </summary>
    public sealed class TranslatorTable
    {
        public const int START_ROW = 3;

        public readonly string sheetName;

        // public readonly int nRow = 0;

        // public readonly int needReadColsCount = 0;

        private readonly List<int> validCols = new List<int>();

        private readonly List<int> validRows = new List<int>();

        private readonly string[] keys;

        private readonly string[] mAttrNames;

        private readonly ValueType[] mAttrTypes;

        private readonly List<byte[]>[,] mCellValues;

        public readonly int validColCount;

        public readonly int validRowCount;

        /// <summary>
        /// Excel表转换为Table. 1-2两行没用
        /// 注：
        ///     第3行是读取标记，默认0（始终读取）,否则检查对应位
        ///     第4行是数据类型: 见TranslatorTable.ExcelValueToValueType方法
        ///     第5行是字段名
        ///     第6行开始是数据
        ///     第1列必须为id: string.
        /// </summary>
        public TranslatorTable(ExcelWorksheet excelSheet, int readMask)
        {
            try
            {
                //解析数据表有效数据行列
                sheetName = excelSheet.Name;
                var strings = sheetName.Split('|');
                if (strings.Length == 2) {
                    sheetName = strings[1];
                }
                int maxRow = 0, maxCol = 0;
                if (excelSheet.Dimension != null)
                {
                    maxRow = excelSheet.Dimension.Rows;
                    maxCol = excelSheet.Dimension.Columns;
                }

                //第3-5行是读取方式 Int（确定列数）、值类型、值属性名称
                var valueTypes = new List<ValueType>();
                var attrNames = new List<string>();

                for (var col = 1; col <= maxCol; col++)
                {
                    var readTagObj = excelSheet.Cells[START_ROW, col].Value;
                    var valueTypeObj = excelSheet.Cells[START_ROW + 1, col].Value;
                    var attrNameObj = excelSheet.Cells[START_ROW + 2, col].Value;
                    var readMaskString = readTagObj != null ? readTagObj.ToString() : string.Empty;
                    var valueTypeString = valueTypeObj != null ? valueTypeObj.ToString() : string.Empty;
                    var attrNameString = attrNameObj != null ? attrNameObj.ToString() : string.Empty;
                    if (attrNameString.Length > 0 && attrNameString[0] == '#') {
                        //ignore comment col
                        continue;
                    }
                    if (!string.IsNullOrEmpty(readMaskString) && 
                        !string.IsNullOrEmpty(valueTypeString) && 
                        !string.IsNullOrEmpty(attrNameString))
                    {
                        int.TryParse(readMaskString, out var sheetReadMask);
                        if (sheetReadMask == 0 || (readMask & sheetReadMask) == readMask) 
                        {
                            //check is array
                            var isArray = false;
                            if (valueTypeString.EndsWith("[]")) {
                                valueTypeString = valueTypeString.Substring(0, valueTypeString.Length - 2);
                                isArray = true;
                            }
                            
                            attrNames.Add(attrNameString);
                            if (col == 1) valueTypeString = "string";
                            valueTypes.Add(ExcelValueToValueType(valueTypeString, isArray));
                            validCols.Add(col);
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarningFormat("解析Excel：[{0}]表，第{1}列表头为空! 忽略了该列。", sheetName, col);
                    }
                }
                mAttrNames = attrNames.ToArray();
                mAttrTypes = valueTypes.ToArray();

                //检测哪些行数据是有效可以读取的
                for (int rowNum = START_ROW + 3; rowNum <= maxRow; rowNum++)
                {
                    var idValue = excelSheet.Cells[rowNum, 1].Value;
                    var id = idValue != null ? idValue.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (id[0] != '#') validRows.Add(rowNum);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarningFormat("解析Excel：[{0}]表，第{1}行为空! 忽略了该行。", sheetName, rowNum);
                    }
                }

                validRowCount = validRows.Count;
                validColCount = validCols.Count;
                
                //从第6行开始才是数据内容
                keys = new string[validRows.Count];
                mCellValues = new List<byte[]>[validRows.Count, validCols.Count];
                for (int setRow = 0; setRow < validRows.Count; setRow++)
                {
                    var rowRef = validRows[setRow];
                    for (int setCol = 0; setCol < validCols.Count; setCol++)
                    {
                        var colRef = validCols[setCol];
                        var value = string.Empty;
                        if (excelSheet.Cells[rowRef, colRef].Value != null)
                        {
                            value = excelSheet.Cells[rowRef, colRef].Value.ToString();
                        }
                        mCellValues[setRow, setCol] = StringToByteList(mAttrTypes[setCol], value);
                    }
                    keys[setRow] = Encoding.UTF8.GetString(mCellValues[setRow, 0][0]);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("解析Excel：[{0}]表错误! Msg = {1}!", sheetName, ex.Message);
            }
        }

        /// <summary>
        /// bytes字节 转换为Table
        /// </summary>
        public TranslatorTable(byte[] bytes)
        {
            var tempBuffer = new ExcelTranslatorBuffer(bytes, (uint)bytes.Length);

            //写读出格的行列数
            tempBuffer.Out(out sheetName);
            tempBuffer.Out(out validRowCount);
            tempBuffer.Out(out validColCount);

            //读取属性字段 和属性数据类型
            mAttrNames = new string[validColCount];
            mAttrTypes = new ValueType[validColCount];
            for (int index = 0; index < validColCount; index++)
            {
                tempBuffer.Out(out string attrName);
                mAttrNames[index] = attrName;

                tempBuffer.Out(out int attrType);
                mAttrTypes[index] = (ValueType)attrType;
            }

            //读取数据
            keys = new string[validRowCount];
            mCellValues = new List<byte[]>[validRowCount, validColCount];
            for (int rowIdx = 0; rowIdx < validRowCount; rowIdx++)
            {
                for (int colIdx = 0; colIdx < validColCount; colIdx++)
                {
                    tempBuffer.Out(mAttrTypes[colIdx], out var value);
                    mCellValues[rowIdx, colIdx] = value;
                }
                keys[rowIdx] = Encoding.UTF8.GetString(mCellValues[rowIdx, 0][0]);
            }
        }

        public string ID(int rowIndex)
        {
            rowIndex = AssertRow(rowIndex);
            return keys[rowIndex];
        }

        public string Attr(int colIndex)
        {
            colIndex = AssertCol(colIndex);
            return mAttrNames[colIndex];
        }

        public ValueType Type(int colIndex)
        {
            colIndex = AssertCol(colIndex);
            return mAttrTypes[colIndex];
        }

        public List<byte[]> Value(int rowIndex, int colIndex)
        {
            return mCellValues[AssertRow(rowIndex), AssertCol(colIndex)];
        }

        private int AssertRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= validRowCount)
            {
                // rowIndex = Math.Max(0, Math.Min(rowIndex, validRowCount - 1));
                throw new Exception("assert_row 索引越界!");
            }
            return rowIndex;
        }

        private int AssertCol(int colIndex)
        {
            if (colIndex < 0 || colIndex >= validColCount)
            {
                // colIndex = Math.Max(0, Math.Min(colIndex, needReadColsCount - 1));
                throw new Exception("assert_col 索引越界!");
            }
            return colIndex;
        }

        #region 文件转换接口

        public string ToJson()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\n");
            for (int rowIndex = 0; rowIndex < validRowCount; rowIndex++)
            {
                string id = ID(rowIndex);
                stringBuilder.Append("  \"");
                stringBuilder.Append(id);
                stringBuilder.Append("\":{");
                for (int colIndex = 0; colIndex < validColCount; colIndex++)
                {
                    stringBuilder.Append("\"");
                    stringBuilder.Append(Attr(colIndex));
                    stringBuilder.Append("\":");

                    var valueType = mAttrTypes[colIndex];
                    List<byte[]> cellValue = mCellValues[rowIndex, colIndex];

                    //非数组
                    if (valueType == ValueType.Int32 || valueType == ValueType.Bool || 
                        valueType == ValueType.Float || valueType == ValueType.String)
                    {
                        stringBuilder.Append("\"");
                        string str = string.Empty;
                        switch (valueType)
                        {
                            case ValueType.Int32:
                                str = BitConverter.ToInt32(cellValue[0], 0).ToString();
                                break;
                            case ValueType.Bool:
                                str = BitConverter.ToBoolean(cellValue[0], 0).ToString();
                                break;
                            case ValueType.Float:
                                str = BitConverter.ToSingle(cellValue[0], 0).ToString(CultureInfo.InvariantCulture);
                                break;
                            case ValueType.String:
                                str = Encoding.UTF8.GetString(cellValue[0]);
                                str = str.Replace("\"", "\\\"");
                                break;
                        }
                        stringBuilder.Append(str);
                        stringBuilder.Append("\"");
                    }
                    else
                    {
                        stringBuilder.Append("[");
                        int length = cellValue.Count;
                        for (int i = 0; i < length; i++)
                        {
                            stringBuilder.Append("\"");
                            string str = string.Empty;
                            switch (valueType)
                            {
                                case ValueType.Int32Array:
                                    str = BitConverter.ToInt32(cellValue[i], 0).ToString();
                                    break;
                                case ValueType.BoolArray:
                                    str = BitConverter.ToBoolean(cellValue[i], 0).ToString();
                                    break;
                                case ValueType.FloatArray:
                                    str = BitConverter.ToSingle(cellValue[i], 0).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case ValueType.StringArray:
                                    str = Encoding.UTF8.GetString(cellValue[i]);
                                    str = str.Replace("\"", "\\\"");
                                    break;
                            }
                            stringBuilder.Append(str);
                            stringBuilder.Append("\"");
                            if (i < length - 1) stringBuilder.Append(",");
                        }
                        stringBuilder.Append("]");
                    }
                    if (colIndex < validColCount - 1) stringBuilder.Append(",");
                }
                stringBuilder.Append("}");
                if (rowIndex < validRowCount - 1) stringBuilder.Append(",");
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public byte[] ToDataEntryBytes()
        {
            var buffer = new ExcelTranslatorBuffer();
            buffer.Reset();

            //写入表格的行列数
            buffer.In(sheetName);
            buffer.In(validRowCount);
            buffer.In(validColCount);

            //属性写入字节流
            for (int i = 0; i < validColCount; i++)
            {
                buffer.In(mAttrNames[i]);
                buffer.In((int)mAttrTypes[i]);
            }
            //遍历所有的数据行，写入字节流
            for (int i = 0; i < validRowCount; i++)
            {
                for (int j = 0; j < validColCount; j++)
                {
                    buffer.In(mAttrTypes[j], mCellValues[i, j]);
                }
            }
            byte[] bytes = new byte[buffer.Size];
            Array.Copy(buffer.GetBuffer(), bytes, buffer.Size);
            return bytes;
        }

        public string ToDataEntryClass()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("//File Generate By ExcelTranslator, Don't Modify It!\n");
            stringBuilder.Append("using System;\n");
            stringBuilder.Append("using Nave.ConfigTool;\n\n");
            stringBuilder.Append("namespace data\n");
            stringBuilder.Append("{\n");
            stringBuilder.Append(
                $"\tpublic class {ExcelTranslatorUtility.SheetNameToDataEntryClassName(sheetName)} : DataEntryBase\n");
            stringBuilder.Append("\t{\n");

            stringBuilder.Append($"\t\tpublic static string sheetName = \"{sheetName}\";\n");

            //字段
            for (int i = 0; i < validColCount; i++)
            {
                string attrName = Attr(i);
                string typeName = GetCSharpTypeName(Type(i));
                stringBuilder.Append($"\t\tpublic {typeName} {attrName};\n");
            }

            //bytes反序列化函数
            stringBuilder.Append("\n\t\tpublic override void DeSerialized(ExcelTranslatorBuffer buffer)\n");
            stringBuilder.Append("\t\t{\n");
            for (int i = 0; i < validColCount; i++)
            {
                string attrName = Attr(i);
                stringBuilder.Append($"\t\t\tbuffer.Out(out {attrName});\n");
            }
            stringBuilder.Append($"\t\t\tKEY = {Attr(0)}.ToString();\n");
            stringBuilder.Append("\t\t}\n");

            stringBuilder.Append("\t}\n");
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private string GetCSharpTypeName(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int32:
                    return "int";
                case ValueType.Bool:
                    return "bool";
                case ValueType.Float:
                    return "float";
                case ValueType.String:
                    return "string";
                case ValueType.Int32Array:
                    return "int[]";
                case ValueType.BoolArray:
                    return "bool[]";
                case ValueType.FloatArray:
                    return "float[]";
                case ValueType.StringArray:
                    return "string[]";
                default:
                    throw new Exception("ExcelTranslatorBuffer.OutDynamicValue() 不存在的类型！ " + valueType);
            }
        }

        public string ToLuaTable()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("-- File Generate By ExcelTranslator\n");
            stringBuilder.Append($"local {sheetName} = \n");
            stringBuilder.Append("{\n");

            for (int rowIndex = 0; rowIndex < validRowCount; rowIndex++)
            {
                stringBuilder.Append($"  [\"{ID(rowIndex)}\"] = ");
                stringBuilder.Append("{");

                for (int colIndex = 0; colIndex < validColCount; colIndex++)
                {
                    string attrName = Attr(colIndex);
                    var valueType = mAttrTypes[colIndex];
                    List<byte[]> cellValue = mCellValues[rowIndex, colIndex];

                    //非数组
                    if (valueType == ValueType.Int32 || valueType == ValueType.Bool ||
                        valueType == ValueType.Float || valueType == ValueType.String)
                    {
                        var str = string.Empty;
                        switch (valueType)
                        {
                            case ValueType.Int32:
                                str = BitConverter.ToInt32(cellValue[0], 0).ToString();
                                break;
                            case ValueType.Bool:
                                var b = BitConverter.ToBoolean(cellValue[0], 0);
                                str = b ? "true" : "false";
                                break;
                            case ValueType.Float:
                                str = BitConverter.ToSingle(cellValue[0], 0).ToString(CultureInfo.InvariantCulture);
                                break;
                            case ValueType.String:
                                str = Encoding.UTF8.GetString(cellValue[0]);
                                str = str.Replace("\"", "\\\"");
                                str = $"\"{str}\"";
                                break;
                        }
                        stringBuilder.Append($"{attrName}={str}");
                    }
                    else
                    {
                        stringBuilder.Append(attrName + "={");
                        int length = cellValue.Count;
                        for (int i = 0; i < length; i++)
                        {
                            string str = string.Empty;
                            switch (valueType)
                            {
                                case ValueType.Int32Array:
                                    str = BitConverter.ToInt32(cellValue[i], 0).ToString();
                                    break;
                                case ValueType.BoolArray:
                                    var b = BitConverter.ToBoolean(cellValue[i], 0);
                                    str = b ? "true" : "false";
                                    break;
                                case ValueType.FloatArray:
                                    str = BitConverter.ToSingle(cellValue[i], 0).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case ValueType.StringArray:
                                    str = Encoding.UTF8.GetString(cellValue[i]);
                                    str = str.Replace("\"", "\\\"");
                                    str = $"\"{str}\"";
                                    break;
                            }
                            stringBuilder.Append(str);
                            if (i < length - 1) stringBuilder.Append(",");
                        }
                        stringBuilder.Append("}");
                    }
                    if (colIndex < validColCount - 1) stringBuilder.Append(",");
                }
                stringBuilder.Append("}");
                if (rowIndex < validRowCount - 1) stringBuilder.Append(",");
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("}\n");
            stringBuilder.Append($"return {sheetName};");
            return stringBuilder.ToString();
        }

        #endregion

        #region 数据表 静态方法

        private static readonly ExcelTranslatorBuffer TempBuffer = new ExcelTranslatorBuffer();

        public static DataEntryCache ToTableCache(byte[] bytes, Type type)
        {
            TempBuffer.Reset();
            TempBuffer.Append(bytes, (uint)bytes.Length);

            //1 读出格的行列数
            TempBuffer.Out(out string sheetName).Out(out int nRow).Out(out int nCol);

            //new一个实例
            var tableCache = new DataEntryCache(sheetName, nRow, nCol);

            //2 读出属性字段 和属性数据类型
            for (int index = 0; index < nCol; index++)
            {
                TempBuffer.Out(out string _).Out(out int _);
            }

            //反序列化表对象
            for (int index = 0; index < nRow; index++)
            {
                var entry = Activator.CreateInstance(type) as DataEntryBase;
                if (entry == null) throw new InvalidCastException();
                entry.DeSerialized(TempBuffer);
                tableCache[entry.KEY] = entry;
            }
            return tableCache;
        }

        public static DataEntryCache ToTableCache(ExcelWorksheet excelSheet, int readMask, Type type)
        {
            TranslatorTable table = new TranslatorTable(excelSheet, readMask);
            return ToTableCache(table.ToDataEntryBytes(), type);
        }

        public static string ToJson(byte[] bytes)
        {
            TranslatorTable table = new TranslatorTable(bytes);
            return table.ToJson();
        }

        public static string ToJson(ExcelWorksheet excelSheet, int readMask)
        {
            TranslatorTable table = new TranslatorTable(excelSheet, readMask);
            return table.ToJson();
        }

        public static string ToLuaTable(byte[] bytes)
        {
            TranslatorTable table = new TranslatorTable(bytes);
            return table.ToLuaTable();
        }

        public static string ToLuaTable(ExcelWorksheet excelSheet, int readMask)
        {
            TranslatorTable table = new TranslatorTable(excelSheet, readMask);
            return table.ToLuaTable();
        }

        public static ValueType StringToValueType(string typeString)
        {
            typeString = typeString.ToLower();
            if (typeString == "int")
                return ValueType.Int32;
            else if (typeString == "bool")
                return ValueType.Bool;
            else if (typeString == "float")
                return ValueType.Float;
            else if (typeString == "string")
                return ValueType.String;
            else if (typeString == "int[]")
                return ValueType.Int32Array;
            else if (typeString == "bool[]")
                return ValueType.BoolArray;
            else if (typeString == "float[]")
                return ValueType.FloatArray;
            else if (typeString == "string[]")
                return ValueType.StringArray;
            else
                throw new Exception("StringToValueType(): 属性的值类型错误!" + typeString);
        }

        public static List<byte[]> StringToByteList(ValueType type, string value)
        {
            try {
                switch (type) {
                    case ValueType.Int32: {
                        int.TryParse(value, out var vInt32);
                        return new List<byte[]>{BitConverter.GetBytes(vInt32)};
                    }
                    case ValueType.Float: {
                        float.TryParse(value, out var vFloat);
                        return new List<byte[]> { BitConverter.GetBytes(vFloat) };
                    }
                    case ValueType.Bool: {
                        bool.TryParse(value, out var vBool);
                        return new List<byte[]> { BitConverter.GetBytes(vBool) };
                    }
                    case ValueType.String:
                        value = value.Trim();
                        return new List<byte[]> { Encoding.UTF8.GetBytes(value) };
                    default: {
                        var valueArray = string.IsNullOrEmpty(value) ? new string[0] : value.Split('|');
                        for (var index = 0; index < valueArray.Length; index++) {
                            valueArray[index] = valueArray[index].Trim();
                        }

                        switch (type) {
                            case ValueType.Int32Array: {
                                var resultArray = new List<byte[]>();
                                foreach (var t in valueArray) {
                                    int.TryParse(t, out var vInt32);
                                    resultArray.Add(BitConverter.GetBytes(vInt32));
                                }
                                return resultArray;
                            }
                            case ValueType.FloatArray: {
                                var resultArray = new List<byte[]>();
                                foreach (var s in valueArray) {
                                    float.TryParse(s, out var vFloat);
                                    resultArray.Add(BitConverter.GetBytes(vFloat));
                                }
                                return resultArray;
                            }
                            case ValueType.BoolArray: {
                                var resultArray = new List<byte[]>();
                                foreach (var s in valueArray) {
                                    bool.TryParse(s, out var vBool);
                                    resultArray.Add(BitConverter.GetBytes(vBool));
                                }
                                return resultArray;
                            }
                            case ValueType.StringArray: {
                                return valueArray.Select(t => Encoding.UTF8.GetBytes(t)).ToList();
                            }
                        }

                        break;
                    }
                }

                throw new Exception($"属性类型[{type}]错误！");
            }
            catch (Exception e)
            {
                throw new Exception($"属性类型[{type}]错误！Error Msg = {e.Message}");
            }
        }

        public static ValueType ExcelValueToValueType(string type, bool isArray) {
            switch (type) {
                case "int":
                    return isArray ? ValueType.Int32Array : ValueType.Int32;
                case "float":
                    return isArray ? ValueType.FloatArray : ValueType.Float;
                case "bool":
                    return isArray ? ValueType.BoolArray : ValueType.Bool;
                case "string":
                    return isArray ? ValueType.StringArray : ValueType.String;
                default :
                    throw new Exception("invalid value type");
            }
        }

        #endregion
    }
}

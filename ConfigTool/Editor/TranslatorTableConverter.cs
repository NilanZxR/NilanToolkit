using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NilanToolkit.ConfigTool.Editor {
    public static class TranslatorTableConverter {
        
        public static string ToDataBlockCSharpFile(TranslatorTable table, CSharpClassFileGenerateInfo info)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("//File Generate By ExcelTranslator, Don't Modify It!\n");
            stringBuilder.Append("using System;\n");
            stringBuilder.Append("using NilanToolkit.ConfigTool;\n \n");
            stringBuilder.Append($"namespace {info.@namespace}\n");
            stringBuilder.Append("{\n");
            stringBuilder.Append(
                $"\tpublic class {string.Format(info.classNameFormat,table.sheetName)} : DataBlockBase\n");
            stringBuilder.Append("\t{\n");

            stringBuilder.Append($"\t\tpublic static string sheetName = \"{table.sheetName}\";\n");

            //字段
            for (int i = 0; i < table.validColCount; i++)
            {
                string attrName = table.GetPropertyName(i);
                string typeName = GetCSharpTypeName(table.GetType(i));
                stringBuilder.Append($"\t\tpublic {typeName} {attrName};\n");
            }

            //bytes反序列化函数
            stringBuilder.Append("\n\t\tpublic override void DeSerialized(ExcelTranslatorBuffer buffer)\n");
            stringBuilder.Append("\t\t{\n");
            for (int i = 0; i < table.validColCount; i++)
            {
                string propertyName = table.GetPropertyName(i);
                stringBuilder.Append($"\t\t\tbuffer.Read(out {propertyName});\n");
            }
            stringBuilder.Append($"\t\t\tKEY = {table.GetPropertyName(0)}.ToString();\n");
            stringBuilder.Append("\t\t}\n");

            stringBuilder.Append("\t}\n");
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
        
        public static string ToJson(TranslatorTable table)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{\n");
            for (int rowIndex = 0; rowIndex < table.validRowCount; rowIndex++)
            {
                string id = table.GetID(rowIndex);
                stringBuilder.Append("  \"");
                stringBuilder.Append(id);
                stringBuilder.Append("\":{");
                for (int colIndex = 0; colIndex < table.validColCount; colIndex++)
                {
                    stringBuilder.Append("\"");
                    stringBuilder.Append(table.GetPropertyName(colIndex));
                    stringBuilder.Append("\":");

                    var valueType = table.propertyTypes[colIndex];
                    List<byte[]> cellValue = table.cellValues[rowIndex, colIndex];

                    //非数组
                    if (valueType == DataValueType.Int32 || valueType == DataValueType.Bool || 
                        valueType == DataValueType.Float || valueType == DataValueType.String)
                    {
                        stringBuilder.Append("\"");
                        string str = string.Empty;
                        switch (valueType)
                        {
                            case DataValueType.Int32:
                                str = BitConverter.ToInt32(cellValue[0], 0).ToString();
                                break;
                            case DataValueType.Bool:
                                str = BitConverter.ToBoolean(cellValue[0], 0).ToString();
                                break;
                            case DataValueType.Float:
                                str = BitConverter.ToSingle(cellValue[0], 0).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataValueType.String:
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
                                case DataValueType.Int32Array:
                                    str = BitConverter.ToInt32(cellValue[i], 0).ToString();
                                    break;
                                case DataValueType.BoolArray:
                                    str = BitConverter.ToBoolean(cellValue[i], 0).ToString();
                                    break;
                                case DataValueType.FloatArray:
                                    str = BitConverter.ToSingle(cellValue[i], 0).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case DataValueType.StringArray:
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
                    if (colIndex < table.validColCount - 1) stringBuilder.Append(",");
                }
                stringBuilder.Append("}");
                if (rowIndex < table.validRowCount - 1) stringBuilder.Append(",");
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
        
        public static byte[] ToDataBlockBytes(TranslatorTable table)
        {
            var buffer = new ExcelTranslatorBuffer();
            buffer.Reset();

            //写入表格的行列数
            buffer.Write(table.sheetName);
            buffer.Write(table.validRowCount);
            buffer.Write(table.validColCount);

            //属性写入字节流
            for (int i = 0; i < table.validColCount; i++)
            {
                buffer.Write(table.propertyNames[i]);
                buffer.Write((int)table.propertyTypes[i]);
            }
            //遍历所有的数据行，写入字节流
            for (int i = 0; i < table.validRowCount; i++)
            {
                for (int j = 0; j < table.validColCount; j++)
                {
                    buffer.Write(table.propertyTypes[j], table.cellValues[i, j]);
                }
            }
            byte[] bytes = new byte[buffer.Size];
            Array.Copy(buffer.Buffer, bytes, buffer.Size);
            return bytes;
        }
        
        public static string ToLuaTable(TranslatorTable table)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("-- File Generate By ExcelTranslator\n");
            stringBuilder.Append($"local {table.sheetName} = \n");
            stringBuilder.Append("{\n");

            for (int rowIndex = 0; rowIndex < table.validRowCount; rowIndex++)
            {
                stringBuilder.Append($"  [\"{table.GetID(rowIndex)}\"] = ");
                stringBuilder.Append("{");

                for (int colIndex = 0; colIndex < table.validColCount; colIndex++)
                {
                    string attrName = table.GetPropertyName(colIndex);
                    var valueType = table.propertyTypes[colIndex];
                    List<byte[]> cellValue = table.cellValues[rowIndex, colIndex];

                    //非数组
                    if (valueType == DataValueType.Int32 || valueType == DataValueType.Bool ||
                        valueType == DataValueType.Float || valueType == DataValueType.String)
                    {
                        var str = string.Empty;
                        switch (valueType)
                        {
                            case DataValueType.Int32:
                                str = BitConverter.ToInt32(cellValue[0], 0).ToString();
                                break;
                            case DataValueType.Bool:
                                var b = BitConverter.ToBoolean(cellValue[0], 0);
                                str = b ? "true" : "false";
                                break;
                            case DataValueType.Float:
                                str = BitConverter.ToSingle(cellValue[0], 0).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DataValueType.String:
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
                                case DataValueType.Int32Array:
                                    str = BitConverter.ToInt32(cellValue[i], 0).ToString();
                                    break;
                                case DataValueType.BoolArray:
                                    var b = BitConverter.ToBoolean(cellValue[i], 0);
                                    str = b ? "true" : "false";
                                    break;
                                case DataValueType.FloatArray:
                                    str = BitConverter.ToSingle(cellValue[i], 0).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case DataValueType.StringArray:
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
                    if (colIndex < table.validColCount - 1) stringBuilder.Append(",");
                }
                stringBuilder.Append("}");
                if (rowIndex < table.validRowCount - 1) stringBuilder.Append(",");
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("}\n");
            stringBuilder.Append($"return {table.sheetName};");
            return stringBuilder.ToString();
        }

        private static string GetCSharpTypeName(DataValueType valueType)
        {
            switch (valueType)
            {
                case DataValueType.Int32:
                    return "int";
                case DataValueType.Bool:
                    return "bool";
                case DataValueType.Float:
                    return "float";
                case DataValueType.String:
                    return "string";
                case DataValueType.Int32Array:
                    return "int[]";
                case DataValueType.BoolArray:
                    return "bool[]";
                case DataValueType.FloatArray:
                    return "float[]";
                case DataValueType.StringArray:
                    return "string[]";
                default:
                    throw new Exception("ExcelTranslatorBuffer.OutDynamicValue() 不存在的类型！ " + valueType);
            }
        }

    }
}

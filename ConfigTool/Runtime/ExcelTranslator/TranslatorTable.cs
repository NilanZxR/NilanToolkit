using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace NilanToolkit.ConfigTool
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

        public readonly List<int> validCols = new List<int>();

        public readonly List<int> validRows = new List<int>();

        public readonly string[] keys;

        public readonly string[] propertyNames;

        public readonly DataValueType[] propertyTypes;

        public readonly List<byte[]>[,] cellValues;

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
                var valueTypes = new List<DataValueType>();
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
                propertyNames = attrNames.ToArray();
                propertyTypes = valueTypes.ToArray();

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
                cellValues = new List<byte[]>[validRows.Count, validCols.Count];
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
                        cellValues[setRow, setCol] = StringToByteList(propertyTypes[setCol], value);
                    }
                    keys[setRow] = Encoding.UTF8.GetString(cellValues[setRow, 0][0]);
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
            tempBuffer.Read(out sheetName);
            tempBuffer.Read(out validRowCount);
            tempBuffer.Read(out validColCount);

            //读取属性字段 和属性数据类型
            propertyNames = new string[validColCount];
            propertyTypes = new DataValueType[validColCount];
            for (int index = 0; index < validColCount; index++)
            {
                tempBuffer.Read(out string attrName);
                propertyNames[index] = attrName;

                tempBuffer.Read(out int attrType);
                propertyTypes[index] = (DataValueType)attrType;
            }

            //读取数据
            keys = new string[validRowCount];
            cellValues = new List<byte[]>[validRowCount, validColCount];
            for (int rowIdx = 0; rowIdx < validRowCount; rowIdx++)
            {
                for (int colIdx = 0; colIdx < validColCount; colIdx++)
                {
                    tempBuffer.Read(propertyTypes[colIdx], out var value);
                    cellValues[rowIdx, colIdx] = value;
                }
                keys[rowIdx] = Encoding.UTF8.GetString(cellValues[rowIdx, 0][0]);
            }
        }

        public string GetID(int rowIndex)
        {
            rowIndex = AssertRow(rowIndex);
            return keys[rowIndex];
        }

        public string GetPropertyName(int colIndex)
        {
            colIndex = AssertCol(colIndex);
            return propertyNames[colIndex];
        }

        public DataValueType GetType(int colIndex)
        {
            colIndex = AssertCol(colIndex);
            return propertyTypes[colIndex];
        }

        public List<byte[]> GetValue(int rowIndex, int colIndex)
        {
            return cellValues[AssertRow(rowIndex), AssertCol(colIndex)];
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

        #region 数据表 静态方法

        private static readonly ExcelTranslatorBuffer TempBuffer = new ExcelTranslatorBuffer();

        public static DataSheet<T> ToTableCache<T>(byte[] bytes) where T : DataBlockBase
        {
            TempBuffer.Reset();
            TempBuffer.Append(bytes, (uint)bytes.Length);

            //1 读出格的行列数
            TempBuffer.Read(out string sheetName).Read(out int nRow).Read(out int nCol);

            //new一个实例
            var tableCache = new DataSheet<T>(sheetName, nRow, nCol);

            //2 读出属性字段 和属性数据类型
            for (int index = 0; index < nCol; index++)
            {
                TempBuffer.Read(out string _).Read(out int _);
            }

            //反序列化表对象
            for (int index = 0; index < nRow; index++)
            {
                var entry = Activator.CreateInstance<T>();
                if (entry == null) throw new InvalidCastException();
                entry.DeSerialized(TempBuffer);
                tableCache[entry.KEY] = entry;
            }
            return tableCache;
        }

        public static List<byte[]> StringToByteList(DataValueType type, string value)
        {
            try {
                switch (type) {
                    case DataValueType.Int32: {
                        int.TryParse(value, out var vInt32);
                        return new List<byte[]>{BitConverter.GetBytes(vInt32)};
                    }
                    case DataValueType.Float: {
                        float.TryParse(value, out var vFloat);
                        return new List<byte[]> { BitConverter.GetBytes(vFloat) };
                    }
                    case DataValueType.Bool: {
                        bool.TryParse(value, out var vBool);
                        return new List<byte[]> { BitConverter.GetBytes(vBool) };
                    }
                    case DataValueType.String:
                        value = value.Trim();
                        return new List<byte[]> { Encoding.UTF8.GetBytes(value) };
                    default: {
                        var valueArray = string.IsNullOrEmpty(value) ? new string[0] : value.Split('|');
                        for (var index = 0; index < valueArray.Length; index++) {
                            valueArray[index] = valueArray[index].Trim();
                        }

                        switch (type) {
                            case DataValueType.Int32Array: {
                                var resultArray = new List<byte[]>();
                                foreach (var t in valueArray) {
                                    int.TryParse(t, out var vInt32);
                                    resultArray.Add(BitConverter.GetBytes(vInt32));
                                }
                                return resultArray;
                            }
                            case DataValueType.FloatArray: {
                                var resultArray = new List<byte[]>();
                                foreach (var s in valueArray) {
                                    float.TryParse(s, out var vFloat);
                                    resultArray.Add(BitConverter.GetBytes(vFloat));
                                }
                                return resultArray;
                            }
                            case DataValueType.BoolArray: {
                                var resultArray = new List<byte[]>();
                                foreach (var s in valueArray) {
                                    bool.TryParse(s, out var vBool);
                                    resultArray.Add(BitConverter.GetBytes(vBool));
                                }
                                return resultArray;
                            }
                            case DataValueType.StringArray: {
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

        public static DataValueType ExcelValueToValueType(string type, bool isArray) {
            switch (type) {
                case "int":
                    return isArray ? DataValueType.Int32Array : DataValueType.Int32;
                case "float":
                    return isArray ? DataValueType.FloatArray : DataValueType.Float;
                case "bool":
                    return isArray ? DataValueType.BoolArray : DataValueType.Bool;
                case "string":
                    return isArray ? DataValueType.StringArray : DataValueType.String;
                default :
                    throw new Exception("invalid value type");
            }
        }

        #endregion
    }
}

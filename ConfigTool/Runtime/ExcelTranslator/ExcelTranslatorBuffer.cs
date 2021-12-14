using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

namespace NilanToolkit.ConfigTool {
    public class ExcelTranslatorBuffer {
        public const uint MAXSIZE = 0xffffffff;
        protected const int DEFAULT_BUFFER_SIZE = 256 << 3;

        private readonly byte[] _rwBuffer = new byte[128];
        protected byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];


        public byte[] Buffer => buffer;
        public uint BufferSize { get; protected set; }

        public uint Size { get; set; }
        public uint ReadPosition { get; private set; }

        public uint WritePosition { get; private set; }

        public ExcelTranslatorBuffer() {
            _Resize(DEFAULT_BUFFER_SIZE);
        }

        public ExcelTranslatorBuffer(ExcelTranslatorBuffer origin) {
            Resize(origin.Size);
        }

        public ExcelTranslatorBuffer(byte[] buffer, uint size) {
            if (buffer != null) {
                Resize((uint) buffer.Length);
                Array.Copy(buffer, this.buffer, buffer.Length);
            }
            else {
                _Resize(size);
            }
        }

        public void Reset() {
            BufferSize = default;
            ReadPosition = default;
            WritePosition = default;
        }

        private void _Resize(uint newSize) {
            if (newSize > BufferSize) {
                newSize = Math.Max(newSize, BufferSize * 2);
                if (newSize > buffer.Length * sizeof(byte)) {
                    if (Size > 0) {
                        Array.Resize(ref buffer, (int) newSize);
                    }
                    else {
                        buffer = new byte[newSize];
                    }
                }
                BufferSize = newSize;
            }
        }

        public void Resize(uint newSize) {
            _Resize(newSize);
            if (newSize > Size)
                Size = newSize;
        }

        //读写接口
        private void _Write(byte[] value, uint size) {
            Resize(WritePosition + size);
            Array.Copy(value, 0, buffer, WritePosition, size);
            WritePosition += size;
        }

        private void _Write(byte[] value, uint offset, uint size) {
            Resize(WritePosition + size);
            Array.Copy(value, offset, buffer, WritePosition, size);
            WritePosition += size;
        }

        protected void _Read(byte[] dest, uint size) {
            Array.Copy(buffer, ReadPosition, dest, 0, size);
            ReadPosition += size;
        }

        public int _Read(byte[] dest, int offset, int size) {
            if (offset < 0) {
                return 0;
            }
            var dSize = (uint) dest.Length;
            if (offset > dSize - 1) {
                return 0;
            }
            if (Size - 1 < ReadPosition) {
                return 0;
            }

            var rSize = (uint) size;
            if (dSize - 1 < offset + rSize) {
                rSize = (uint) (dSize - offset - 1);
            }
            var lSize = Size - ReadPosition;

            if (lSize < rSize) {
                rSize = lSize;
            }

            if (rSize == 0) {
                return 0;
            }
            Array.Copy(buffer, ReadPosition, dest, offset, rSize);
            ReadPosition += rSize;
            return (int) rSize;
        }

        public void Append(byte[] src, uint size) { _Write(src, size); }

        public ExcelTranslatorBuffer Write(bool value) {
            if (value)
                _rwBuffer[0] = 1;
            else
                _rwBuffer[0] = 0;
            _Write(_rwBuffer, sizeof(bool));
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out bool value) {
            _Read(_rwBuffer, sizeof(bool));
            value = BitConverter.ToBoolean(_rwBuffer, 0);
            return this;
        }
        
        public ExcelTranslatorBuffer Write(bool[] valueArray) {
            Write(valueArray.Length);
            foreach (var val in valueArray) {
                Write(val);
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out bool[] valueArray) {
            Read(out int length);
            valueArray = new bool[length];
            for (var i = 0; i < length; i++) {
                Read(out bool value);
                valueArray[i] = value;
            }
            return this;
        }

        public ExcelTranslatorBuffer Write(int value) {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(_rwBuffer, 0);
            _Write(_rwBuffer, sizeof(int));
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out int value) {
            _Read(_rwBuffer, sizeof(int));
            value = BitConverter.ToInt32(_rwBuffer, 0);
            return this;
        }
        
        public ExcelTranslatorBuffer Write(int[] valueArray) {
            Write(valueArray.Length);
            foreach (var val in valueArray) {
                Write(val);
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out int[] valueArray) {
            Read(out int length);
            valueArray = new int[length];
            for (var i = 0; i < length; i++) {
                Read(out int value);
                valueArray[i] = value;
            }
            return this;
        }

        public ExcelTranslatorBuffer Write(string value) {
            var bytes = Encoding.UTF8.GetBytes(value);
            var size = bytes.Length;
            Write(size);
            if (size > 0) _Write(bytes, (uint) size);
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out string value) {
            Read(out int size);
            if (size > 0) {
                _Read(_rwBuffer, (uint) size);
                value = Encoding.UTF8.GetString(_rwBuffer, 0, size);
            }
            else {
                value = string.Empty;
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Write(string[] valueArray) {
            Write(valueArray.Length);
            foreach (var val in valueArray) {
                Write(val);
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out string[] valueArray) {
            Read(out int length);
            valueArray = new string[length];
            for (var i = 0; i < length; i++) {
                Read(out string value);
                valueArray[i] = value;
            }
            return this;
        }

        public ExcelTranslatorBuffer Write(float value) {
            var bytes = BitConverter.GetBytes(value);
            bytes.CopyTo(_rwBuffer, 0);
            _Write(_rwBuffer, sizeof(float));
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out float value) {
            _Read(_rwBuffer, sizeof(float));
            value = BitConverter.ToSingle(_rwBuffer, 0);
            return this;
        }
        
        public ExcelTranslatorBuffer Write(float[] valueArray) {
            Write(valueArray.Length);
            foreach (var val in valueArray) {
                Write(val);
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Read(out float[] valueArray) {
            Read(out int length);
            valueArray = new float[length];
            for (var i = 0; i < length; i++) {
                Read(out float value);
                valueArray[i] = value;
            }
            return this;
        }

        public ExcelTranslatorBuffer Write(DataValueType valueType, List<byte[]> values) {
            //读取数组长度
            var length = values.Count;
            if (valueType == DataValueType.BoolArray ||
                valueType == DataValueType.Int32Array ||
                valueType == DataValueType.FloatArray ||
                valueType == DataValueType.StringArray) {
                Write(length);
            }

            foreach (var value in values) {
                switch (valueType) {
                    case DataValueType.Int32:
                    case DataValueType.Int32Array:
                    case DataValueType.Bool:
                    case DataValueType.BoolArray:
                    case DataValueType.Float:
                    case DataValueType.FloatArray:
                        _Write(value, (uint) value.Length);
                        break;
                    case DataValueType.String:
                    case DataValueType.StringArray:
                        var size = value != null ? value.Length : 0;
                        _Write(BitConverter.GetBytes(size), sizeof(int));
                        if (size > 0) _Write(value, (uint) size);
                        break;
                    default:
                        throw new Exception("ExcelTranslatorBuffer.Write() 不存在的类型！ " + valueType);
                }
            }
            return this;
        }
        
        public ExcelTranslatorBuffer Read(DataValueType valueType, out List<byte[]> value) {
            value = new List<byte[]>();

            //读取数组长度
            var length = 1;
            if (valueType == DataValueType.BoolArray ||
                valueType == DataValueType.Int32Array ||
                valueType == DataValueType.FloatArray ||
                valueType == DataValueType.StringArray) {
                Read(out length);
            }

            for (var i = 0; i < length; i++) {
                int size;
                switch (valueType) {
                    case DataValueType.Int32:
                    case DataValueType.Int32Array:
                        size = sizeof(int);
                        break;

                    case DataValueType.Bool:
                    case DataValueType.BoolArray:
                        size = sizeof(bool);
                        break;

                    case DataValueType.Float:
                    case DataValueType.FloatArray:
                        size = sizeof(float);
                        break;

                    case DataValueType.String:
                    case DataValueType.StringArray:
                        Read(out size);
                        break;
                    default:
                        throw new Exception("ExcelTranslatorBuffer.OutDynamicValue() 不存在的类型！ " + valueType);
                }
                var bytes = new byte[size];
                if (size > 0) _Read(bytes, (uint) size);
                value.Add(bytes);
            }
            return this;
        }
    }
}

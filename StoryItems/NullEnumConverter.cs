using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CSC.StoryItems
{
    internal class NullEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private static readonly TypeCode s_enumTypeCode = Type.GetTypeCode(typeof(T));
        public sealed override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(T);

        //borrowed and adapted from the original .net jsonstringenumconverter
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (TryParseEnumFromString(ref reader, out T result))
                    {
                        return result;
                    }
                    break;

                case JsonTokenType.Number:
                    switch (s_enumTypeCode)
                    {
                        case TypeCode.Int32 when reader.TryGetInt32(out int int32):
                            return (T)(object)int32;
                        case TypeCode.UInt32 when reader.TryGetUInt32(out uint uint32):
                            return (T)(object)uint32;
                        case TypeCode.Int64 when reader.TryGetInt64(out long int64):
                            return (T)(object)int64;
                        case TypeCode.UInt64 when reader.TryGetUInt64(out ulong uint64):
                            return (T)(object)uint64;
                        case TypeCode.Byte when reader.TryGetByte(out byte ubyte8):
                            return (T)(object)ubyte8;
                        case TypeCode.SByte when reader.TryGetSByte(out sbyte byte8):
                            return (T)(object)byte8;
                        case TypeCode.Int16 when reader.TryGetInt16(out short int16):
                            return (T)(object)int16;
                        case TypeCode.UInt16 when reader.TryGetUInt16(out ushort uint16):
                            return (T)(object)uint16;
                    }
                    break;
            }

            return default;
        }

        private bool TryParseEnumFromString(ref Utf8JsonReader reader, out T result)
        {
            int bufferLength = reader.ValueSpan.Length;

            Span<char> charBuffer = bufferLength <= 128
                ? stackalloc char[128]
                : throw new ArgumentException("Enum value of type " + typeof(T).Name + " value name at " + reader.Position.ToString() + " was longer than 127 characters");

            int charsWritten = reader.CopyString(charBuffer);
            charBuffer = charBuffer[..charsWritten];

            string source = ((ReadOnlySpan<char>)charBuffer).Trim().ToString();

            if (Enum.TryParse<T>(source, out result))
            {
                return true;
            }

            return false;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

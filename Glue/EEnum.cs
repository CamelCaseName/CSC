namespace CSC.Glue
{
    public static class EEnum
    {
        public static string StringParse<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
            {
                return result.ToString()!;
            }
            else
            {
                return string.Empty;
            }
        }

        public static T CastParse<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
            {
                return result;
            }
            else
            {
                return default;
            }
        }
        public static string StringParse<T>(int value) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return Enum.GetName(typeof(T), value) ?? string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static T CastParse<T>(int value) where T : struct, Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                var name = Enum.GetName(typeof(T), value) ?? string.Empty;
                if (!string.IsNullOrEmpty(name))
                {
                    return Enum.Parse<T>(name);
                }
                return default;
            }
            else
            {
                return default;
            }
        }
    }
}

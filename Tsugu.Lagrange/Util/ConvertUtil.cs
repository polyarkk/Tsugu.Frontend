namespace Tsugu.Lagrange.Util;

public static class ConvertUtil {
    /// <summary>
    /// 根据所给类型转换为指定类型的值，对于已有的 Convert 方法，直接调用自带的 Convert 方法
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="o">需要转换的实例</param>
    /// <returns>转换后的实例，对于不支持的类型，抛出异常</returns>
    public static object? To(Type type, object? o) {
        if (o == null) {
            return null;
        }
        
        object? converted = null;

        if (type == typeof(string)) {
            converted = Convert.ToString(o);
        } else if (type == typeof(bool)) {
            converted = Convert.ToBoolean(o);
        } else if (type == typeof(byte)) {
            converted = Convert.ToByte(o);
        } else if (type == typeof(char)) {
            converted = Convert.ToChar(o);
        } else if (type == typeof(decimal)) {
            converted = Convert.ToDecimal(o);
        } else if (type == typeof(double)) {
            converted = Convert.ToDouble(o);
        } else if (type == typeof(short)) {
            converted = Convert.ToInt16(o);
        } else if (type == typeof(int)) {
            converted = Convert.ToInt32(o);
        } else if (type == typeof(long)) {
            converted = Convert.ToUInt64(o);
        } else if (type == typeof(float)) {
            converted = Convert.ToSingle(o);
        } else if (type == typeof(DateTime)) {
            converted = Convert.ToDateTime(o);
        } else if (type == typeof(sbyte)) {
            converted = Convert.ToSByte(o);
        } else if (type == typeof(ushort)) {
            converted = Convert.ToUInt16(o);
        } else if (type == typeof(uint)) {
            converted = Convert.ToUInt32(o);
        } else if (type == typeof(ulong)) {
            converted = Convert.ToUInt64(o);
        } else if (type.IsEnum) {
            string str = Convert.ToString(o) ?? "null";
            
            if (!Enum.TryParse(type, str, true, out converted)) {
                throw new FormatException($"failed to convert [{str}] to enum [{type.Name}]");
            }
        } else {
            throw new NotSupportedException();
        }

        return converted;
    }
}

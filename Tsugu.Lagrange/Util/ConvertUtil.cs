namespace Tsugu.Lagrange.Util;

public static class ConvertUtil {
    /// <summary>
    /// 根据所给类型转换为指定类型的值
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="o">需要转换的实例</param>
    /// <returns>转换后的实例，对于不支持的类型，抛出异常</returns>
    public static object? To(Type type, object? o) {
        if (o == null) {
            return null;
        }

        object? converted;

        if (type.IsEnum) {
            string str = Convert.ToString(o) ?? "null";

            if (!Enum.TryParse(type, str, true, out converted)) {
                throw new FormatException($"failed to convert [{str}] to enum [{type.Name}]");
            }
        } else {
            converted = Convert.ChangeType(o, type);
        }

        return converted;
    }
}

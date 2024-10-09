using System.Collections.Specialized;
using Tsugu.Lagrange.Util;

namespace Tsugu.Lagrange.Command;

public class ParsedArgs {
    /// <summary>
    /// 所有参数连接得到的字符串
    /// </summary>
    public string ConcatenatedArgs { get; }

    private readonly OrderedDictionary _parsedArgs;

    public Element this[string key] => new(_parsedArgs[key]);

    /// <summary>
    /// 批量获取参数（vararg）
    /// </summary>
    /// <param name="r">范围</param>
    public Element[] this[Range r] {
        get {
            List<Element> elements = [];

            int start = r.Start.IsFromEnd ? _parsedArgs.Count - r.Start.Value : r.Start.Value;
            int end = r.End.IsFromEnd ? _parsedArgs.Count - r.End.Value : r.End.Value;

            for (int i = start; i < end; i++) {
                elements.Add(new Element(_parsedArgs[i]));
            }

            return elements.ToArray();
        }
    }

    public ParsedArgs(ArgumentMeta[] metas, string[] tokens) {
        _parsedArgs = new OrderedDictionary();

        if (tokens.Length <= 1) {
            ConcatenatedArgs = "";
            
            return;
        }

        string[] args = tokens[1..];

        ConcatenatedArgs = string.Join(" ", args);

        int i = 0;

        for (; i < metas.Length; i++) {
            (string simpleName, string name, Type type, bool optional) = metas[i];

            string? arg = i < 0 || i >= args.Length ? null : args[i];

            if (!optional && arg == null) {
                throw new CommandParseException($"未提供关键参数 [{name}]！");
            }

            try {
                _parsedArgs[simpleName] = ConvertUtil.To(type, arg);
            } catch (Exception) {
                if (type.IsEnum) {
                    string candidates = string.Join("|", Enum.GetNames(type).Select(n => n.ToLower()));

                    throw new CommandParseException($"参数 [{name}] 非法，需要 {type.Name}！({candidates})");
                }

                throw new CommandParseException($"参数 [{name}] 非法，需要 {type.Name}！");
            }
        }

        (_, string lastName, Type lastType, _) = metas[^1];
            
        // 解析剩余参数
        for (; i < args.Length; i++) {
            try {
                _parsedArgs[$"{i}_orphan"] = ConvertUtil.To(lastType, args[i]);
            } catch (Exception) {
                if (lastType.IsEnum) {
                    string candidates = string.Join("|", Enum.GetNames(lastType).Select(n => n.ToLower()));

                    throw new CommandParseException($"参数 [{lastName}] 非法，需要 {lastType.Name}！({candidates})");
                }

                throw new CommandParseException($"参数 [{lastName}] 非法，需要 {lastType.Name}！");
            }
        }
    }

    public class Element {
        private readonly dynamic? _item;

        public Element(dynamic? item) {
            _item = item;
        }

        /// <summary>
        /// 获取指定类型的元素
        /// </summary>
        /// <typeparam name="TTo">指定类型</typeparam>
        /// <returns>值</returns>
        /// <exception cref="NullReferenceException">值为 null 时将报错</exception>
        public TTo Get<TTo>() where TTo : struct {
            if (_item == null) {
                throw new NullReferenceException();
            }

            return (TTo)_item;
        }

        /// <summary>
        /// <inheritdoc cref="Get{TTo}"/>，若为 null 则使用指定值替代
        /// </summary>
        /// <param name="fallback">替代值</param>
        /// <typeparam name="TTo"><inheritdoc cref="Get{TTo}"/></typeparam>
        /// <returns><inheritdoc cref="Get{TTo}"/></returns>
        public TTo GetOr<TTo>(Func<TTo> fallback) where TTo : struct {
            if (_item == null) {
                return fallback.Invoke();
            }

            return (TTo)_item;
        }

        /// <summary>
        /// <inheritdoc cref="Get{TTo}"/>，得到的元素可能为 null
        /// </summary>
        /// <typeparam name="TTo"><inheritdoc cref="Get{TTo}"/></typeparam>
        /// <returns><inheritdoc cref="Get{TTo}"/></returns>
        public TTo? GetOrNull<TTo>() where TTo : struct {
            if (_item == null) {
                return null;
            }

            return (TTo)_item;
        }
    }
}

using Microsoft.AspNetCore.Components;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace easy_blazor_bulma_demo.Services;

/// <summary>
/// Reads XML documentation comments from the easy-blazor-bulma library.
/// </summary>
public static class XmlDocs
{
    /// <summary>
    /// Lazily loads and caches the members dictionary the first time it is accessed.
    /// </summary>
    /// <remarks>The value is initialized on first use and then reused for subsequent access.</remarks>
    private static readonly Lazy<Dictionary<string, XElement>> Members = new(Load);

    /// <summary>
    /// Loads the XML documentation members from the embedded XML documentation resource for the assembly.
    /// </summary>
    /// <returns>A dictionary of member entries keyed by member name, or an empty dictionary when the embedded resource is not available.</returns>
    private static Dictionary<string, XElement> Load()
    {
        using var stream = typeof(XmlDocs).Assembly.GetManifestResourceStream("easy-blazor-bulma.xml");

        if (stream == null)
            return [];

        return XDocument.Load(stream)
            .Descendants("member")
            .Where(x => x.Attribute("name") != null)
            .ToDictionary(x => x.Attribute("name")!.Value);
    }

    /// <summary>
    /// Gets the XML documentation element for the specified type.
    /// </summary>
    /// <param name="type">The type to get the XML documentation for.</param>
    /// <returns>The XML documentation element for the specified type, or null if no documentation is available.</returns>
    public static XElement? GetTypeDoc(Type type) =>
        Members.Value.GetValueOrDefault("T:" + GetName(type));

    /// <summary>
    /// Gets the XML documentation element for the specified property.
    /// </summary>
    /// <param name="property">The property to retrieve the XML documentation element for.</param>
    /// <returns>The XML documentation element for the property, or null if no documentation exists.</returns>
    public static XElement? GetPropertyDoc(PropertyInfo property) =>
        Members.Value.GetValueOrDefault("P:" + GetName(property.DeclaringType!) + "." + property.Name);

    /// <summary>
    /// Gets the [Parameter] properties declared directly on the component.
    /// </summary>
    public static IEnumerable<PropertyInfo> GetParameters(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null);

    /// <summary>
    /// Gets the fully qualified name of the specified type.
    /// </summary>
    /// <remarks>Generic types are resolved to their generic type definition before obtaining the name, and
    /// nested types are represented with dot separators instead of '+' separators.</remarks>
    /// <param name="type">The type to get the name for. Generic type definitions are normalized before the name is returned.</param>
    /// <returns>The fully qualified type name with nested type separators normalized to periods.</returns>
    private static string GetName(Type type)
    {
        if (type.IsGenericType)
            type = type.GetGenericTypeDefinition();

        return type.FullName!.Replace('+', '.');
    }


    /// <summary>
    /// Converts an XML doc element (e.g. summary, remarks) into HTML.
    /// </summary>
    public static MarkupString ToHtml(XElement? element)
    {
        if (element == null)
            return default;

        var html = new StringBuilder();
        AppendNodes(element.Nodes(), html);

        return new MarkupString(html.ToString().Trim());
    }

    /// <summary>
    /// Appends the rendered HTML for a sequence of XML nodes to a StringBuilder.
    /// </summary>
    /// <param name="nodes">The XML nodes to convert and append as HTML.</param>
    /// <param name="html">The StringBuilder that receives the rendered HTML output.</param>
    private static void AppendNodes(IEnumerable<XNode> nodes, StringBuilder html)
    {
        foreach (var node in nodes)
        {
            if (node is XText text)
            {
                html.Append(WebUtility.HtmlEncode(text.Value));
                continue;
            }

            if (node is not XElement element)
                continue;

            switch (element.Name.LocalName)
            {
                case "para":
                    html.Append("<p class=\"block\">");
                    AppendNodes(element.Nodes(), html);
                    html.Append("</p>");
                    break;

                case "c":
                    html.Append("<code>");
                    AppendNodes(element.Nodes(), html);
                    html.Append("</code>");
                    break;

                case "see" when element.Attribute("href") != null:
                    var href = WebUtility.HtmlEncode(element.Attribute("href")!.Value);
                    html.Append($"<a href=\"{href}\" target=\"_blank\">");

                    if (element.Nodes().Any())
                        AppendNodes(element.Nodes(), html);
                    else
                        html.Append(href);

                    html.Append("</a>");
                    break;

                case "see" or "seealso" when element.Attribute("cref") != null:
                    html.Append($"<code>{WebUtility.HtmlEncode(GetShortName(element.Attribute("cref")!.Value))}</code>");
                    break;

                case "see" when element.Attribute("langword") != null:
                    html.Append($"<code>{element.Attribute("langword")!.Value}</code>");
                    break;

                case "paramref" or "typeparamref":
                    html.Append($"<code>{WebUtility.HtmlEncode(element.Attribute("name")?.Value)}</code>");
                    break;

                case "list":
                    html.Append("<ul>");

                    foreach (var item in element.Elements("item"))
                    {
                        html.Append("<li>");
                        AppendNodes((item.Element("description") ?? item).Nodes(), html);
                        html.Append("</li>");
                    }

                    html.Append("</ul>");
                    break;

                default:
                    AppendNodes(element.Nodes(), html);
                    break;
            }
        }
    }

    /// <summary>
    /// Turns a cref ID like "P:easy_blazor_bulma.InputAutocomplete`2.Items" into "Items".
    /// </summary>
    private static string GetShortName(string cref)
    {
        var name = cref[(cref.IndexOf(':') + 1)..];
        var paren = name.IndexOf('(');

        if (paren >= 0)
            name = name[..paren];

        name = name[(name.LastIndexOf('.') + 1)..];
        var tick = name.IndexOf('`');

        return tick >= 0 ? name[..tick] : name;
    }

    /// <summary>
    /// Gets a C#-style type name, e.g. Func&lt;TItem, string&gt; instead of Func`2.
    /// </summary>
    public static string GetTypeName(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);

        if (underlying != null)
            return GetTypeName(underlying) + "?";

        if (Aliases.TryGetValue(type, out var alias))
            return alias;

        if (type.IsGenericType)
        {
            var name = type.Name[..type.Name.IndexOf('`')];
            return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(GetTypeName))}>";
        }

        return type.Name;
    }

    /// <summary>
    /// Maps .NET types to their corresponding C# aliases.
    /// </summary>
    private static readonly Dictionary<Type, string> Aliases = new()
    {
        [typeof(string)] = "string",
        [typeof(int)] = "int",
        [typeof(bool)] = "bool",
        [typeof(decimal)] = "decimal",
        [typeof(double)] = "double",
        [typeof(object)] = "object",
    };
}
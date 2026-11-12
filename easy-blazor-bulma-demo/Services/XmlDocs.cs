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
    /// <param name="nodes">The XML nodes, e.g. summary, remarks, to convert and append as HTML.</param>
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

            // Handle known XML doc elements and convert them to HTML.
            switch (element.Name.LocalName)
            {
                // Render <para> elements as <p> blocks.
                case "para":
                    html.Append("<p class=\"block\">");
                    AppendNodes(element.Nodes(), html);
                    html.Append("</p>");
                    break;

                // Render <b> elements as <span> blocks with bold text.
                case "b":
                    html.Append("<span class=\"has-text-weight-bold\">");
                    AppendNodes(element.Nodes(), html);
                    html.Append("</span>");
                    break;

                // Render <c> elements as inline code blocks.
                case "c":
                    html.Append("<code>");
                    AppendNodes(element.Nodes(), html);
                    html.Append("</code>");
                    break;

                // Render <see> elements as <a> links when they have an href attribute, or as <code> blocks when they have a cref attribute.
                case "see" when element.Attribute("href") != null:
                    var href = WebUtility.HtmlEncode(element.Attribute("href")!.Value);
                    html.Append($"<a href=\"{href}\" target=\"_blank\">");

                    if (element.Nodes().Any())
                        AppendNodes(element.Nodes(), html);
                    else
                        html.Append(href);

                    html.Append("</a>");
                    break;

                // Render <see> and <seealso> elements with a cref attribute as inline code.
                case "see" or "seealso" when element.Attribute("cref") != null:
                    html.Append($"<code>{WebUtility.HtmlEncode(GetShortName(element.Attribute("cref")!.Value))}</code>");
                    break;

                // Render <see> elements with a langword attribute as inline code.
                case "see" when element.Attribute("langword") != null:
                    html.Append($"<code>{element.Attribute("langword")!.Value}</code>");
                    break;

                // Render <paramref> and <typeparamref> elements as inline code.
                case "paramref" or "typeparamref":
                    html.Append($"<code>{WebUtility.HtmlEncode(element.Attribute("name")?.Value)}</code>");
                    break;

                // Render <list> elements as unordered lists, with <item> elements as list items.
                case "list":
                    html.Append("<ul>");

                    foreach (var item in element.Elements("item"))
                    {
                        var term = item.Element("term");
                        var description = item.Element("description");

                        html.Append("<li>");

                        // If an <item> has both a <term> and a <description>, the term is rendered in bold followed by the description.
                        if (term != null && description != null)
                        {
                            html.Append("<strong>");
                            AppendNodes(term.Nodes(), html);
                            html.Append("</strong>: ");
                            AppendNodes(description.Nodes(), html);
                        }
                        else
                        {
                            AppendNodes((description ?? item).Nodes(), html);
                        }

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
    /// Extracts a clean, short display name from an XML documentation cref ID.
    /// e.g., <c>"P:easy_blazor_bulma.InputAutocomplete`2.Items"</c> becomes <c>"Items"</c>,
    /// and <c>"T:easy_blazor_bulma.InputAutocomplete`2"</c> becomes <c>"InputAutocomplete"</c>.
    /// </summary>
    private static string GetShortName(string cref)
    {
        // 1. Remove the member type prefix (e.g., "P:", "T:", "M:")
        var name = cref[(cref.IndexOf(':') + 1)..];

        // 2. Remove method parameter lists if present (e.g., "MyMethod(System.String)" -> "MyMethod")
        var parenIndex = name.IndexOf('(');
        if (parenIndex >= 0)
            name = name[..parenIndex];

        // 3. Keep only the final member or type segment after the last dot
        name = name[(name.LastIndexOf('.') + 1)..];

        // 4. Remove any generic suffix (e.g., "InputAutocomplete`2" -> "InputAutocomplete")
        return RemoveGenericSuffix(name);
    }

    /// <summary>
    /// Removes the generic type count suffix (e.g., "`1", "`2") from a .NET type or member name.
    /// </summary>
    /// <remarks>
    /// In .NET metadata, generic types include a backtick followed by the number of type parameters (e.g., <c>InputAutocomplete`2</c>).
    /// This method trims the backtick and suffix so the name is clean for UI display.
    /// </remarks>
    /// <param name="name">The raw .NET type or member name.</param>
    /// <returns>The clean name without the generic suffix, or the original name if none exists.</returns>
    public static string RemoveGenericSuffix(string name)
    {
        var backtickIndex = name.IndexOf('`');
        return backtickIndex >= 0 ? name[..backtickIndex] : name;
    }

    /// <summary>
    /// Converts a <see cref="Type"/> object into a clean, human-readable C# type string (like int? or Func&lt;TItem, string&gt;) rather than using .NET's raw reflection names (like Int32, Nullable'1, or Func'2)
    /// </summary>
    public static string GetTypeName(Type type)
    {
        // 1. Handle nullable types, e.g. int? instead of Nullable`1.
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
            return GetTypeName(underlying) + "?";

        // 2. Handle arrays, e.g., string[] or int[,] instead of String[]
        if (type.IsArray)
        {
            var elementType = GetTypeName(type.GetElementType()!);
            // Get the rank of the array (number of dimensions) and create a string of commas to represent it, e.g., [,] for a 2D array.
            var rank = new string(',', type.GetArrayRank() - 1);
            return $"{elementType}[{rank}]";
        }

        // 3. Use the C# keyword alias if it exists, e.g., "int" instead of "Int32"
        if (Aliases.TryGetValue(type, out var alias))
            return alias;

        // 4. Handle generic types (e.g., convert "Dictionary`2" to "Dictionary<string, int>")
        if (type.IsGenericType)
        {
            // Strip the backtick and parameter count from the metadata name (e.g., "List`1" -> "List")
            var name = RemoveGenericSuffix(type.Name);

            // Recursively format each generic type argument (e.g., [String, Int32] -> "string, int")
            var args = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));

            return $"{name}<{args}>";
        }

        return type.Name;
    }

    /// <summary>
    /// Maps .NET types to their corresponding C# aliases.
    /// </summary>
    private static readonly Dictionary<Type, string> Aliases = new()
    {
        [typeof(void)] = "void",
        [typeof(object)] = "object",
        [typeof(string)] = "string",
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(char)] = "char",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(nint)] = "nint",
        [typeof(nuint)] = "nuint",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
    };

    /// <summary>
    /// Gets the generic type parameters (e.g. TItem, TValue) of a component with their descriptions.
    /// </summary>
    /// <param name="type">The component type to get the generic type parameters for.</param>
    /// <returns>An enumerable of <see cref="TypeParameterDoc"/> representing the generic type parameters and their descriptions.</returns>
    public static IEnumerable<TypeParameterDoc> GetTypeParameters(Type type)
    {
        // Only generic type definitions have type parameters.
        if (type.IsGenericTypeDefinition == false)
            return [];

        var doc = GetTypeDoc(type);

        // For each generic type parameter, find its corresponding <typeparam> element in the XML documentation and create a TypeParameterDoc with the name and description.
        return type.GetGenericArguments()
            .Select(x => new TypeParameterDoc(
                x.Name,
                ToHtml(doc?.Elements("typeparam").FirstOrDefault(y => y.Attribute("name")?.Value == x.Name))))
            .ToList();
    }

}

/// <summary>
/// Describes a type parameter (e.g. TItem, TValue) of a generic component, including its name and description.
/// </summary>
/// <param name="Name">The name of the type parameter.</param>
/// <param name="Description">The description of the type parameter.</param>
public record TypeParameterDoc(string Name, MarkupString Description);
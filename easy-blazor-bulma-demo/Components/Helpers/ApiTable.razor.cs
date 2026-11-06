using easy_blazor_bulma_demo.Services;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Helpers;

/// <summary>
/// Displays a table of a component's [Parameter] properties using its XML documentation.
/// </summary>
public partial class ApiTable : ComponentBase
{
    /// <summary>
    /// The component type to document, e.g. typeof(InputAutocomplete&lt;,&gt;).
    /// </summary>
    [Parameter, EditorRequired]
    public Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Overrides the heading. Defaults to "{ComponentName} Parameters".
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// The heading text displayed above the table. Defaults to "{ComponentName} Parameters".
    /// </summary>
    private string Heading = string.Empty;

    /// <summary>
    /// The collection of API rows to display in the table, each representing a [Parameter] property of the component.
    /// </summary>
    private List<ApiRow> Rows = [];

    /// <summary>
    /// Sets the heading and documented parameter rows for the component.
    /// </summary>
    protected override void OnParametersSet()
    {
        var name = ComponentType.Name;
        var tick = name.IndexOf('`');

        Heading = Title ?? $"{(tick >= 0 ? name[..tick] : name)} Parameters";

        Rows = XmlDocs.GetParameters(ComponentType)
            .Select(x =>
            {
                var doc = XmlDocs.GetPropertyDoc(x);

                return new ApiRow(
                    x.Name,
                    XmlDocs.GetTypeName(x.PropertyType),
                    XmlDocs.ToHtml(doc?.Element("summary")),
                    XmlDocs.ToHtml(doc?.Element("remarks")));
            })
            .ToList();
    }

    /// <summary>
    /// Represents a row of API reference documentation.
    /// </summary>
    /// <param name="Name">The name of the API member.</param>
    /// <param name="Type">The type of the API member.</param>
    /// <param name="Description">The description of the API member.</param>
    /// <param name="Remarks">Additional remarks about the API member.</param>
    private record ApiRow(string Name, string Type, MarkupString Description, MarkupString Remarks);
}
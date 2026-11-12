using easy_blazor_bulma_demo.Services;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;

namespace easy_blazor_bulma_demo.Components.Helpers;

/// <summary>
/// Displays the XML documentation (summary, remarks, type parameters) for a component.
/// </summary>
public partial class ComponentInfo : ComponentBase
{
    /// <summary>
    /// The component type to document, e.g. typeof(InputAutocomplete&lt;,&gt;).
    /// </summary>
    [Parameter, EditorRequired]
    public Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Extra page-specific notes shown after the generated documentation.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Optional heading shown above the documentation.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// XML documentation for a type (e.g. InputAutocomplete&lt;,&gt;), including summary, remarks, and type parameters.
    /// </summary>
    private XElement? TypeDoc;

    /// <summary>
    /// Stores the type parameter elements from the XML documentation for the component type, if any.
    /// </summary>
    private List<TypeParameterDoc> TypeParams = [];

    /// <summary>
    /// Loads the component type's XML documentation and collects its type parameter documentation.
    /// </summary>
    protected override void OnParametersSet()
    {
        TypeDoc = XmlDocs.GetTypeDoc(ComponentType);
        TypeParams = XmlDocs.GetTypeParameters(ComponentType).ToList();
    }
}
using easy_blazor_bulma_demo.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace easy_blazor_bulma_demo.Components.Helpers;

/// <summary>
/// Displays a live example component with its title, description, and collapsible source code.
/// </summary>
public partial class ExampleSection : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; init; } = default!;

    /// <summary>
    /// The heading shown above the example.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The example component to render, e.g. typeof(AutocompleteBasicExample).
    /// </summary>
    [Parameter, EditorRequired]
    public Type ComponentType { get; set; } = default!;

    /// <summary>
    /// Optional description explaining what the example shows.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Identifies the code element.
    /// </summary>
    private ElementReference CodeElement;

    /// <summary>
    /// The source associated with the current item. Used to display the source code in the collapsible section.
    /// </summary>
    private string ComponentSourceCode = string.Empty;

    /// <summary>
    /// Sets the component source code when parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        ComponentSourceCode = ExampleSource.Get(ComponentType);
    }

    /// <summary>
    /// Highlights the code with Prism after the component's first render.
    /// </summary>
    /// <param name="firstRender">Whether this is the first time the component has been rendered.</param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntime.PrismHighlightCode(CodeElement);
    }
}

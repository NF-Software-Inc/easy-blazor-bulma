using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages;

/// <summary>
/// Provides shared behavior for demo pages that document Blazor components.
/// </summary>
public abstract class TestComponentBase : ComponentBase
{
    /// <summary>
    /// The component types documented on this test page.
    /// </summary>
    protected abstract Type[] ComponentTypes { get; }

    /// <summary>
    /// The referenced parameters section element. Used for scrolling to the section.
    /// </summary>
    protected ElementReference ParametersSection { get; set; }

    /// <summary>
    /// Moves focus to the parameters section.
    /// </summary>
    protected async Task JumpToParameters() => await ParametersSection.FocusAsync();
}

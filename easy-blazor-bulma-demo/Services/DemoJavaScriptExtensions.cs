using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace easy_blazor_bulma_demo.Services;

/// <summary>
/// JavaScript interop wrappers used only by the demo app.
/// </summary>
public static class DemoJavaScriptExtensions
{
    /// <summary>
    /// Applies Prism syntax highlighting to a code element.
    /// </summary>
    /// <param name="js">The JS runtime instance.</param>
    /// <param name="element">The element to highlight.</param>
    /// <param name="token">An optional cancellation token.</param>
    public static ValueTask PrismHighlightCode(this IJSRuntime js, ElementReference element, CancellationToken? token = null) =>
        js.InvokeVoidAsync("Prism.highlightElement", token ?? CancellationToken.None, element);
}
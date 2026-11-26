using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Use to create a single tab within a <see cref="Tabs"/> component.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/components/tabs/">Bulma Documentation</see>
/// </remarks>
public partial class Tab : ComponentBase, IAsyncDisposable
{
	/// <summary>
	/// The name to display this tab as on the tab bar.
	/// </summary>
	[Parameter]
	public string? Name { get; set; }

	/// <summary>
	/// An icon to display beside the name of the tab on the tab bar.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The content to display within the tab.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    private Tabs Parent { get; init; } = default!;

    /// <summary>
    /// Any additional attributes applied directly to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    internal readonly string[] Filter = ["class"];
	internal bool IsActive => Parent.ActiveIndex == Index;

	/// <summary>
	/// The index of this tab within the parent <see cref="Tabs"/> component.
	/// </summary>
	/// <remarks>
	/// Serves as a unique identifier for the tab within the parent component.
	/// </remarks>
	public int Index { get; internal set; }

	/// <inheritdoc/>
	protected async override Task OnInitializedAsync()
	{
		await Parent.AddChild(this);
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync()
	{
		await Parent.RemoveChild(this);
		GC.SuppressFinalize(this);
	}
}

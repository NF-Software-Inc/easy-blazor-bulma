using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Base class to create table cells with.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/table/">Bulma Documentation</see>
/// </remarks>
public abstract class TableCellBase : ComponentBase
{
	/// <summary>
	/// Content to display within the table cell.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	/// <summary>
	/// Contains HTTP attributes to remove from those applied to the resulting component.
	/// </summary>
	protected virtual string[] Filter { get; } = new[] { "class" };

	/// <summary>
	/// The full CSS class to assign to the button.
	/// </summary>
	protected string? MainCssClass => AdditionalAttributes.GetValue("class");
}

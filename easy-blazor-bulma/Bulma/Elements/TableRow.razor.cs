using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Use to create a row within a table component.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/table/">Bulma Documentation</see>
/// </remarks>
public partial class TableRow : ComponentBase
{
	/// <summary>
	/// Content to display within the table row.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = new[] { "class" };

	private string? MainCssClass => AdditionalAttributes.GetValue("class");
}

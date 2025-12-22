using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// The inevitable HTML table, with special case cells.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: thead-class and tbody-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/elements/table/">Bulma Documentation</see>
/// </remarks>
public partial class Table : ComponentBase
{
	/// <summary>
	/// Specifies whether the table should use striped rows.
	/// </summary>
	[Parameter]
	public bool IsStriped { get; set; } = true;

	/// <summary>
	/// The content to display in the header section of the table.
	/// </summary>
	[Parameter]
	public RenderFragment? TableHead { get; set; }

	/// <summary>
	/// The content to display in the body section of the table.
	/// </summary>
	[Parameter]
	public RenderFragment? TableBody { get; set; }

	/// <summary>
	/// Fallback for content to display within the table. Applies when neither <see cref="TableHead"/> nor <see cref="TableBody"/> are provided.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = new[] { "class", "thead-class", "tbody-class" };

	private string MainCssClass
	{
		get
		{
			var css = "table";

			if (IsStriped)
				css += " is-striped";

			return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
		}
	}
}

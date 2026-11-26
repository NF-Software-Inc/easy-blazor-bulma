using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a th element within a table.
/// </summary>
/// <remarks>
/// There are 2 additional attributes that can be used: data-tooltip and tooltip-class. The data-tooltip adds a hover tooltip to the element and the tooltip-class adds custom CSS to the corresponding element.
/// <see href="https://bulma.io/documentation/elements/table/">Bulma Documentation</see>
/// </remarks>
public partial class TableHeader : TableCellBase
{
	/// <summary>
	/// The text to display in the label.
	/// </summary>
	[Parameter]
	public string? DisplayText { get; set; }

	/// <summary>
	/// An icon to display in the left of the cell.
	/// </summary>
	[Parameter]
	public virtual string? Icon { get; set; }

	/// <summary>
	/// Sets the display mode for a tooltip when present.
	/// </summary>
	/// <remarks>
	/// Tooltips can be added by using the data-tooltip attibute.
	/// </remarks>
	[Parameter]
	public TooltipOptions TooltipMode { get; set; } = TooltipOptions.Default;

	/// <inheritdoc />
	protected override string[] Filter { get; } = new[] { "class", "data-tooltip", "tooltip-class" };

	private string? Tooltip;

	private string TooltipCssClass
	{
		get
		{
			var css = "";

			if (string.IsNullOrWhiteSpace(Tooltip) == false)
			{
				css += "is-cursor-help";

				if (TooltipMode.HasFlag(TooltipOptions.Top))
					css += " has-tooltip-top";
				else if (TooltipMode.HasFlag(TooltipOptions.Bottom))
					css += " has-tooltip-bottom";
				else if (TooltipMode.HasFlag(TooltipOptions.Left))
					css += " has-tooltip-left";
				else if (TooltipMode.HasFlag(TooltipOptions.Right))
					css += " has-tooltip-right";

				if (TooltipMode.HasFlag(TooltipOptions.HasArrow))
					css += " has-tooltip-arrow";

				if (TooltipMode.HasFlag(TooltipOptions.AlwaysActive))
					css += " has-tooltip-active";

				if (TooltipMode.HasFlag(TooltipOptions.Multiline))
					css += " has-tooltip-multiline";
			}

			return string.Join(' ', css, AdditionalAttributes.GetValue("tooltip-class"));
		}
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		Tooltip = AdditionalAttributes.GetValue("data-tooltip");
	}
}

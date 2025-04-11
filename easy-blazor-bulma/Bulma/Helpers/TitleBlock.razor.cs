using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a styled block to display a title at the top of a page.
/// </summary>
/// <remarks>
/// There are 4 additional attributes that can be used: body-class, title-class, data-tooltip and tooltip-class. The data-tooltip adds a hover tooltip to the element and the rest will apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class TitleBlock : ComponentBase
{
	/// <summary>
	/// The text to display in the bar.
	/// </summary>
	[Required]
	[Parameter]
	public required string Title { get; set; }

	/// <summary>
	/// Appends to the provided title in the browser tab name. Only applies when <see cref="UpdatePageTitle"/> is true.
	/// </summary>
	[Parameter]
	public string? ApplicationName { get; set; }

	/// <summary>
	/// An optional icon to display to the left of the title.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// An optional link to apply to the icon.
	/// </summary>
	[Parameter]
	public string? IconUrl { get; set; }

	/// <summary>
	/// Specifies whether the tiny sizing should be applied to the title bar.
	/// </summary>
	[Parameter]
	public bool IsTiny { get; set; } = true;

	/// <summary>
	/// Specifies whether the title bar text should be in bold.
	/// </summary>
	[Parameter]
	public bool IsBold { get; set; } = true;

	/// <summary>
	/// Specifies whether to update the browser tab name with the title.
	/// </summary>
	[Parameter]
	public bool UpdatePageTitle { get; set; } = true;

	/// <summary>
	/// Sets the display mode for a tooltip when present.
	/// </summary>
	/// <remarks>
	/// Tooltips can be added by using the data-tooltip attibute.
	/// </remarks>
	[Parameter]
	public TooltipOptions TooltipMode { get; set; } = TooltipOptions.Default;

	/// <summary>
	/// Additional content to display in the body section of the title block.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = new[] { "class", "body-class", "title-class", "data-tooltip", "tooltip-class" };

	private string? Tooltip;

	private string MainCssClass
	{
		get
		{
			var css = "hero is-primary";

			if (IsTiny)
				css += " is-tiny";

			if (IsBold)
				css += " is-bold";

			return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
		}
	}

	private string BodyCssClass => string.Join(' ', "hero-body pl-4", AdditionalAttributes.GetClass("body-class"));
	private string TitleCssClass => string.Join(' ', "title has-text-centered", AdditionalAttributes.GetClass("title-class"));

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

			return string.Join(' ', css, AdditionalAttributes.GetClass("tooltip-class"));
		}
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		Tooltip = AdditionalAttributes.GetValue("data-tooltip");
	}
}

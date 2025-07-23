using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for toggling boolean values. Supported types are <see cref="bool"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 4 additional attributes that can be used: div-class, label-class, tooltip-class, and data-tooltip. The first 3 apply CSS classes to the resulting elements as per their names. The last adds a hover tooltip to the element.
/// </remarks>
public partial class InputSwitch<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
    /// <summary>
    /// Text to display to the right of the switch. Automatically takes <see cref="DisplayAttribute.GetName"/> when available.
    /// </summary>
    [Parameter]
	public string? Label { get; set; }

    /// <summary>
    /// Specifies whether to display the resulting switch in a box.
    /// </summary>
    [Parameter]
	public bool IsBoxed { get; set; } = true;

	/// <summary>
	/// Sets the display mode for a tooltip when present.
	/// </summary>
	/// <remarks>
	/// Tooltips can be added either by using the data-tooltip attribute or if the bound value has a <see cref="DisplayAttribute.GetDescription"/>.
	/// </remarks>
	[Parameter]
	public TooltipOptions TooltipMode { get; set; } = TooltipOptions.Default;

    private readonly string[] Filter = new string[] { "class", "id", "data-tooltip", "div-class", "label-class", "tooltip-class" };

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

	private string? Id;
    private string? Tooltip;

    private string MainCssClass => string.Join(' ', "switch", CssClass);

	private string LabelCssClass => string.Join(' ', "is-unselectable", AdditionalAttributes.GetClass("label-class"));

	private string DivCssClass
	{
		get
		{
			var css = "checkbox";

			if (IsBoxed)
				css += " box";

			return string.Join(' ', css, AdditionalAttributes.GetClass("div-class"));
		}
	}

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
		// Type checks
		if (UnderlyingType != typeof(bool))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type bool.");

		// Extract details
		if (string.IsNullOrWhiteSpace(Id))
			Id = AdditionalAttributes.GetValue("id") ?? Guid.NewGuid().ToString("N");

		DisplayAttribute? attribute = null;

		if (ValueExpression != null)
			attribute = ValueExpression.GetPropertyAttribute<TValue, DisplayAttribute>();

		if (string.IsNullOrWhiteSpace(Label))
			Label = attribute?.GetName();

		Tooltip = AdditionalAttributes.GetValue("data-tooltip") ?? attribute?.GetDescription();
	}

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		bool boolValue => boolValue.ToString(),
		_ => string.Empty
	};
}

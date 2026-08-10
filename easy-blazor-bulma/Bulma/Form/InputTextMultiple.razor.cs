using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for adding multiple text values to a list.
/// </summary>
/// <remarks>
/// <para>
/// There are 2 additional attributes that can be used: button-class and tag-class.
/// Each of which apply CSS classes to the resulting elements as per their names.
/// </para>
///
/// <para>
/// By default the <c>is-info</c> color class is applied for button-class.
/// By default the <c>is-success</c> color class is applied for tag-class.
/// Providing another Bulma color class will suppress the default so it can take effect.
/// </para>
///
/// <para>
/// <see href="https://bulma.io/documentation/form/general/">Bulma Documentation</see>
/// </para>
/// </remarks>
public partial class InputTextMultiple : InputBase<List<string>>
{
	/// <summary>
	/// Specifies whether the component allows the same value to be added more than once.
	/// </summary>
	[Parameter]
	public bool AllowDuplicates { get; set; }

	/// <summary>
	/// Specifies the <see cref="StringComparer"/> to use when comparing values for duplicates.
	/// </summary>
	[Parameter]
	public StringComparer ComparisonMode { get; set; } = StringComparer.Ordinal;

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = ["class", "button-class", "tag-class"];
	private readonly string[] DefaultKeys = ["Enter", "NumpadEnter"];
	private string? InputValue;
	private bool OnKeyDownPreventDefault;

	private string MainCssClass => string.Join(' ', "input", CssClass);

	private string ButtonCssClass
	{
		get
		{
			var css = string.Join(' ', "button", AdditionalAttributes.GetValue("button-class"));

			if (CssClassHelper.ContainsColorClass(css) == false)
				css += " is-info";

			return css;
		}
	}

	private string TagCssClass
	{
		get
		{
			var css = string.Join(' ', "tag", "mr-1", "mb-1", AdditionalAttributes.GetValue("tag-class"));

			if (CssClassHelper.ContainsColorClass(css) == false)
				css += " is-success";

			return css;
		}
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out List<string> result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		result = Value ?? [];
		validationErrorMessage = null;

		return true;
	}

	private void OnInput(ChangeEventArgs args)
	{
		InputValue = args.Value?.ToString();
	}

	private void OnKeyUp(KeyboardEventArgs args)
	{
		if (args.Code == "Enter" || args.Code == "NumpadEnter")
			AddCurrentValue();
	}

	private void OnKeyDown(KeyboardEventArgs args)
	{
		OnKeyDownPreventDefault = DefaultKeys.Contains(args.Code);
	}

	private void OnAddClicked()
	{
		AddCurrentValue();
	}

	private void AddCurrentValue()
	{
		if (AdditionalAttributes.IsDisabled())
			return;

		var changed = InputValue?.Trim();

		if (string.IsNullOrWhiteSpace(changed))
			return;

		Value ??= [];

		if (AllowDuplicates || Value.Contains(changed, ComparisonMode) == false)
		{
			Value.Add(changed);
			InputValue = null;

			_ = ValueChanged.InvokeAsync(Value);
			EditContext.NotifyFieldChanged(FieldIdentifier);
		}
	}

	private void OnItemRemoved(int index)
	{
		if (Value == null || index < 0 || index >= Value.Count)
			return;

		Value.RemoveAt(index);
		_ = ValueChanged.InvokeAsync(Value);

		EditContext.NotifyFieldChanged(FieldIdentifier);
	}
}

﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a select list with the provided items. Supported types inherit class.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <see href="https://bulma.io/documentation/form/select/">Bulma Documentation</see>
/// </remarks>
public partial class InputSelectObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// The collection of items to display in the list.
	/// </summary>
	[Parameter]
	public IEnumerable<TValue> Items { get; set; }

	/// <summary>
	/// A function to return the values to display in the drop-down list.
	/// </summary>
	[Parameter]
	public Func<TValue, string> DisplayValue { get; set; }

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "list";

	/// <summary>
	/// Specifies the text to display for the null option.
	/// </summary>
	[Parameter]
	public string NullText { get; set; } = "Null";

    /// <summary>
    /// Specifies whether to allow null values to be selected.
    /// </summary>
    [Parameter]
    public bool IsNullable { get; set; } = true;

    /// <summary>
    /// Applies styles to the input.
    /// </summary>
    [Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <inheritdoc cref="InputDateTimeOptions.UseAutomaticStatusColors"/>
	[Parameter]
	public bool UseAutomaticStatusColors { get; set; } = true;

	private readonly Type UnderlyingType;

	private string FullCssClass
	{
		get
		{
			var css = "select";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	public InputSelectObject()
	{
		UnderlyingType = typeof(TValue);

		if (UnderlyingType.GetTypeInfo().IsClass == false)
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be a class.");
	}

	/// <inheritdoc/>
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		if (UseAutomaticStatusColors)
			ResetStatus();

        var match = Items.FirstOrDefault(x => string.Equals(DisplayValue(x), value, StringComparison.OrdinalIgnoreCase));

		if (match != null || IsNullable)
		{
            result = match!;

            if (UseAutomaticStatusColors)
                DisplayStatus |= InputStatus.BackgroundSuccess;

            validationErrorMessage = null;
            return true;
        }
		else
		{
            result = default;

            if (UseAutomaticStatusColors)
                DisplayStatus |= InputStatus.BackgroundDanger;

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "No match could be found in the {0} field.", DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		TValue selectedValue => DisplayValue(selectedValue),
		_ => string.Empty
	};

	private void OnSelectionChanged(ChangeEventArgs args)
	{
        if (IsNullable && (args.Value == null || string.IsNullOrWhiteSpace(args.Value.ToString())))
            CurrentValueAsString = null;
        else
            CurrentValueAsString = args.Value?.ToString() ?? "";
    }

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}
}

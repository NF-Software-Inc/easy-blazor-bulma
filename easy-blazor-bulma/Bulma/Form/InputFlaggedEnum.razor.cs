using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a checkbox list with the values of the provided flagged enum. Supported types must inherit <see cref="Enum"/>, but not <see cref="ulong"/>.
/// </summary>
/// <typeparam name="TEnum">The type of enum to use.</typeparam>
/// <remarks>
/// There are 2 additional attributes that can be used: switch-class and label-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/form/checkbox/">Bulma Documentation</see>
/// </remarks>
public partial class InputFlaggedEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEnum> : InputBase<TEnum>
{
    /// <summary>
    /// Wraps each item in the .box CSS class when true.
    /// </summary>
    [Parameter]
    public bool IsBoxed { get; set; } = true;

    /// <summary>
    /// Specifies whether to hide the option assigned to 0.
    /// </summary>
    [Parameter]
    public bool HideZeroOption { get; set; } = true;

	private readonly string[] Filter = new[] { "class", "switch-class", "label-class" };

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
	private bool IsNullable;
	private readonly string PropertyName = Guid.NewGuid().ToString("N");
	private long[] EnumValues = [];

    private string MainCssClass
    {
        get
        {
            var css = "checkbox";

            if (IsBoxed)
                css += " box";

			return string.Join(' ', css, CssClass);
        }
	}

    private string SwitchCssClass => string.Join(' ', "switch", AdditionalAttributes.GetValue("switch-class"));
    private string LabelCssClass => string.Join(' ', "is-unselectable", AdditionalAttributes.GetValue("label-class"));

	/// <inheritdoc/>
	protected override void OnInitialized()
	{
		IsNullable = Nullable.GetUnderlyingType(typeof(TEnum)) != null;

		if (typeof(Enum).IsAssignableFrom(UnderlyingType) == false)
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must inherit {nameof(Enum)}.");
		else if (Enum.GetUnderlyingType(UnderlyingType) == typeof(ulong))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Does not support enums based on ulong.");

        // Populate EnumValues with all non-zero enum values converted to long
		var rawValues = Enum.GetValues(UnderlyingType);
		var longValues = new List<long>(rawValues.Length);

		foreach (var v in rawValues)
		{
			long l = Convert.ToInt64(v);

			if (l != 0)
				longValues.Add(l);
		}

		EnumValues = longValues.ToArray();
	}

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TEnum result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (IsNullable == false && string.IsNullOrWhiteSpace(value))
        {
            result = default!;

            validationErrorMessage = null;
            return true;
        }
        else if (Enum.TryParse(UnderlyingType, value, true, out object? parsed))
        {
            result = (TEnum)parsed!;

            validationErrorMessage = null;
            return true;
        }
        else
        {
            result = default;

            validationErrorMessage = string.Format(CultureInfo.InvariantCulture, "The {0} field could not be parsed.", DisplayName ?? FieldIdentifier.FieldName);
            return false;
        }
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TEnum? value) => value switch
    {
        TEnum enumValue => enumValue.ToString() ?? string.Empty,
        _ => string.Empty
    };

    private void OnValueChanged(TEnum flag)
    {
        if (AdditionalAttributes.IsDisabled())
            return;

        long current = CurrentValue != null ? Convert.ToInt64(CurrentValue) : 0L;
        long update = Convert.ToInt64(flag);

        // if the flag is already set, unset it, also unset any flags that are dependent on it
        if ((current & update) == update)
        {
            long removeMask = 0;

            foreach (long enumValue in EnumValues)
            {
                // if the enum value has the bit represented by update toggled, add it to the remove mask
                if ((enumValue & update) != 0)
                    removeMask |= enumValue;
            }

            current &= ~removeMask; // unset the bits in the remove mask
        }
        else
        {
            current |= update;  // set the bit represented by update
        }

        CurrentValueAsString = Enum.ToObject(UnderlyingType, current).ToString();
    }

    private bool IsFlagChecked(TEnum flag)
    {
        return CurrentValue != null && (Convert.ToInt64(CurrentValue) & Convert.ToInt64(flag)) == Convert.ToInt64(flag);
    }

    private string GetEnumSwitchId(TEnum value) => $"switch-InputFlaggedEnum-{PropertyName}-{value}";
}

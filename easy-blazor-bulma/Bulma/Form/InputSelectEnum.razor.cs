using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a select list with the values of the provided enum. Supported types must inherit <see cref="Enum"/>.
/// </summary>
/// <typeparam name="TEnum"></typeparam>
/// <remarks>
/// <see href="https://bulma.io/documentation/form/select/">Bulma Documentation</see>
/// </remarks>
[Obsolete("Use InputSelectObject with DisplayValue='x => x.GetValueDisplayName()' Items='Enum.GetValues<TestEnum>()' instead.")]
public partial class InputSelectEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEnum> : InputBase<TEnum>
{
    /// <summary>
    /// A function to determine whether two items are equal.
    /// </summary>
    [Parameter]
    public Func<TEnum, TEnum?, bool> AreEqual { get; set; } = EqualityComparer<TEnum>.Default.Equals;

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
	/// Applies styles to the input.
	/// </summary>
	[Parameter]
    public InputStatus DisplayStatus { get; set; }

    /// <inheritdoc cref="InputDateTimeOptions.UseAutomaticStatusColors"/>
    [Parameter]
    public bool UseAutomaticStatusColors { get; set; } = true;

    /// <summary>
    /// Specifies whether to hide the option assigned to 0.
    /// </summary>
    [Parameter]
    public bool HideZeroOption { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = new[] { "class" };

	private readonly bool IsNullable;
    private readonly Type UnderlyingType;

    private string MainCssClass
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

    public InputSelectEnum()
    {
        var nullable = Nullable.GetUnderlyingType(typeof(TEnum));

        UnderlyingType = nullable ?? typeof(TEnum);
        IsNullable = nullable != null;

        if (typeof(Enum).IsAssignableFrom(UnderlyingType) == false)
            throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must inherit {nameof(Enum)}.");
    }

    /// <inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TEnum result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (UseAutomaticStatusColors)
            ResetStatus();

        if (IsNullable == false && string.IsNullOrWhiteSpace(value))
        {
            result = default!;

            if (UseAutomaticStatusColors)
                DisplayStatus |= InputStatus.BackgroundSuccess;

            validationErrorMessage = null;
            return true;
        }
        else if (Enum.TryParse(UnderlyingType, value, true, out object? parsed))
        {
            result = (TEnum)parsed!;

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

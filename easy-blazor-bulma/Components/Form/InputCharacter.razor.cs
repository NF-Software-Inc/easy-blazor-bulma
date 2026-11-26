using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for editing date and time values. Supported types are <see cref="char"/>.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// There are 3 additional attributes that can be used: columns-class, column-class, and button-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class InputCharacter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>, IAsyncDisposable
{
	/// <summary>
	/// Specifies the number of columns to divide the character selections into.
	/// </summary>
	[Parameter]
	[Range(1, 12)]
	public int Columns { get; set; } = 3;

	/// <summary>
	/// Specifies whether to display the case changing button.
	/// </summary>
	[Parameter]
	public bool ShowCaseChange { get; set; } = true;

	/// <summary>
	/// Specifies whether to round the character selection buttons.
	/// </summary>
	[Parameter]
	public bool IsRounded { get; set; } = true;

	/// <summary>
	/// Specifies whether to apply a border to the character selection buttons.
	/// </summary>
	[Parameter]
	public bool IsBordered { get; set; } = true;

	/// <summary>
	/// Specifies the color to highlight the selected value with.
	/// </summary>
	[Parameter]
	public BulmaColors ActiveColor { get; set; } = BulmaColors.Turquoise;

	/// <summary>
	/// The set of characters to generate buttons for.
	/// </summary>
	[Parameter]
	public char[] Characters { get; set; } = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

	/// <summary>
	/// Specifies whether the arrow keys change the selected character across rows and columns while a character button has focus.
	/// </summary>
	/// <remarks>
	/// Set to <see langword="false"/> when embedding the component inside a container, such as a table or grid, that handles arrow-key focus navigation.
	/// </remarks>
	[Parameter]
	public bool UseArrowKeyNavigation { get; set; } = true;

	/// <summary>
	/// The culture used to format and parse character values. Defaults to <see cref="CultureInfo.InvariantCulture"/>.
	/// </summary>
	[Parameter]
	public CultureInfo? Culture { get; set; }

	[Inject]
	private IJSRuntime JsRuntime { get; init; } = default!;

	private readonly string[] Filter = ["class", "columns-class", "column-class", "button-class"];

	private readonly Type UnderlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
	private bool IsNullable;

	private CultureInfo DisplayProvider => Culture ?? CultureInfo.CurrentCulture;
	private CultureInfo FormatProvider => Culture ?? CultureInfo.InvariantCulture;

	private bool IsUpperCase = true;
	private bool? RegisteredUseArrowKeyNavigation;

	private ElementReference? ButtonsElement;
	private static readonly string[] PassthroughKeys = ["Escape", "Tab", "Enter"];
	private static readonly string[] ArrowKeys = ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"];
	private static readonly string[] DefaultKeys = [.. PassthroughKeys, .. ArrowKeys];
	private string[] AllowedKeys => UseArrowKeyNavigation ? PassthroughKeys : DefaultKeys;

	private string ColumnsCssClass => string.Join(' ', "columns mb-0", AdditionalAttributes.GetValue("columns-class"));

	private string ColumnCssClass => string.Join(' ', "column pb-0", AdditionalAttributes.GetValue("column-class"));

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		// Type checks
		IsNullable = Nullable.GetUnderlyingType(typeof(TValue)) != null;

		if (UnderlyingType != typeof(char))
			throw new InvalidOperationException($"Unsupported type param '{UnderlyingType.Name}'. Must be of type char.");

		// Check casing on start value
		var current = CurrentValueAsString?.FirstOrDefault();

		if (current != null && current != '\0' && char.IsUpper(current.Value) == false)
			IsUpperCase = false;
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (ButtonsElement != null && ButtonsElement.Value.Id != null && RegisteredUseArrowKeyNavigation != UseArrowKeyNavigation)
		{
			await JsRuntime.RegisterKeyDownPreventDefault(ButtonsElement.Value, AllowedKeys, false, "button");

			RegisteredUseArrowKeyNavigation = UseArrowKeyNavigation;
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		if (ButtonsElement != null && ButtonsElement.Value.Id != null)
		{
			try
			{
				await JsRuntime.UnregisterKeyDownPreventDefault(ButtonsElement.Value);
			}
			catch (Exception e) when (e is JSDisconnectedException or TaskCanceledException or ObjectDisposedException) { }
		}

		((IDisposable)this).Dispose();
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
        if (IsNullable == false && string.IsNullOrWhiteSpace(value))
		{
			result = default!;

            validationErrorMessage = null;
            return true;
        }
		else if (BindConverter.TryConvertTo(value, FormatProvider, out result))
		{
			validationErrorMessage = null;
			return true;
		}
		else
		{
			result = default;

			validationErrorMessage = string.Format(DisplayProvider, "The {0} field must be a char.", DisplayName ?? FieldIdentifier.FieldName);
			return false;
		}
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		char charValue => charValue.ToString(FormatProvider),
		_ => string.Empty
	};

	private void OnCharacterClicked(char character)
	{
		var current = CurrentValueAsString?.FirstOrDefault();

		if (AdditionalAttributes.IsDisabled())
			return;
		else if (IsNullable == false && current != null && current != '\0' && char.ToUpper(character, FormatProvider) == char.ToUpper(current.Value, FormatProvider))
			return;
		else if (character == '\0' || (current != null && current != '\0' && char.ToUpper(character, FormatProvider) == char.ToUpper(current.Value, FormatProvider)))
			CurrentValueAsString = null;
		else if ((IsUpperCase && char.IsUpper(character)) || (IsUpperCase == false && char.IsLower(character)))
			CurrentValueAsString = character.ToString(FormatProvider);
		else if (IsUpperCase)
			CurrentValueAsString = char.ToUpper(character, FormatProvider).ToString(FormatProvider);
		else
			CurrentValueAsString = char.ToLower(character, FormatProvider).ToString(FormatProvider);
	}

	private void OnKeyDown(KeyboardEventArgs args)
	{
		if (AdditionalAttributes.IsDisabled() || UseArrowKeyNavigation == false || ArrowKeys.Contains(args.Key) == false)
			return;

		var current = CurrentValueAsString?.FirstOrDefault();

		if (current == null || current == '\0')
			return;

		var columns = Characters
			.Split(Columns)
			.Select((Options, IndexC) => new { IndexC, Options = Options.Select((Value, IndexR) => new { IndexR, Value }).ToList() })
			.ToList();

		var column = columns.FirstOrDefault(x => x.Options.Any(y => char.ToUpper(y.Value, FormatProvider) == char.ToUpper(current.Value, FormatProvider)));

		if (column == null)
			return;

		var row = column.Options.Single(x => char.ToUpper(x.Value, FormatProvider) == char.ToUpper(current.Value, FormatProvider));

		if (args.Key == "ArrowUp" && column.Options.Count > 1)
		{
			if (row.IndexR == 0)
				CurrentValueAsString = column.Options[^1].Value.ToString(FormatProvider);
			else
				CurrentValueAsString = column.Options[row.IndexR - 1].Value.ToString(FormatProvider);
		}
		else if (args.Key == "ArrowDown" && column.Options.Count > 1)
		{
			if (row.IndexR == column.Options.Count - 1)
				CurrentValueAsString = column.Options[0].Value.ToString(FormatProvider);
			else
				CurrentValueAsString = column.Options[row.IndexR + 1].Value.ToString(FormatProvider);
		}
		else if (args.Key == "ArrowLeft" && columns.Count > 1)
		{
			if (column.IndexC == 0 && columns[^1].Options.Count - 1 >= row.IndexR)
				CurrentValueAsString = columns[^1].Options[row.IndexR].Value.ToString(FormatProvider);
			else if (column.IndexC == 0)
				CurrentValueAsString = columns[^1].Options.Last().Value.ToString(FormatProvider);
			else if (columns[column.IndexC - 1].Options.Count - 1 >= row.IndexR)
				CurrentValueAsString = columns[column.IndexC - 1].Options[row.IndexR].Value.ToString(FormatProvider);
			else
				CurrentValueAsString = columns[column.IndexC - 1].Options.Last().Value.ToString(FormatProvider);
		}
		else if (args.Key == "ArrowRight" && columns.Count > 1)
		{
			if (column.IndexC == columns.Count - 1 && columns[0].Options.Count - 1 >= row.IndexR)
				CurrentValueAsString = columns[0].Options[row.IndexR].Value.ToString(FormatProvider);
			else if (column.IndexC == columns.Count - 1)
				CurrentValueAsString = columns[0].Options.Last().Value.ToString(FormatProvider);
			else if (columns[column.IndexC + 1].Options.Count - 1 >= row.IndexR)
				CurrentValueAsString = columns[column.IndexC + 1].Options[row.IndexR].Value.ToString(FormatProvider);
			else
				CurrentValueAsString = columns[column.IndexC + 1].Options.Last().Value.ToString(FormatProvider);
		}
	}

	private char GetCharacterDisplay(char character)
	{
		return IsUpperCase ? char.ToUpper(character, FormatProvider) : char.ToLower(character, FormatProvider);
	}

	private void OnCaseChanged()
	{
		IsUpperCase = !IsUpperCase;

		var current = CurrentValueAsString?.FirstOrDefault();

		if (AdditionalAttributes.IsDisabled())
			return;
		else if (current == null || current == '\0')
			return;
		else if (IsUpperCase && char.IsLower(current.Value))
			CurrentValueAsString = char.ToUpper(current.Value, FormatProvider).ToString(FormatProvider);
		else if (IsUpperCase == false && char.IsUpper(current.Value))
			CurrentValueAsString = char.ToLower(current.Value, FormatProvider).ToString(FormatProvider);
	}

	private string GetButtonCssClass(char character)
	{
		var current = CurrentValueAsString?.FirstOrDefault();
		var css = "button is-fullwidth mb-3";

		if (AdditionalAttributes.IsDisabled())
			css += " is-disabled";

		if (ActiveColor != BulmaColors.Default && character != '\0' && current != null && char.ToUpper(current.Value, FormatProvider) == char.ToUpper(character, FormatProvider))
			css += ' ' + BulmaColorHelper.GetColorCss(ActiveColor);

		if (IsRounded)
			css += " is-rounded";

		if (IsBordered)
			css += " is-bordered";

		return string.Join(' ', css, AdditionalAttributes.GetValue("button-class"));
	}
}

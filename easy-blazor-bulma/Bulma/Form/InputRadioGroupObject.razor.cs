using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Simplifies usage of the <see cref="InputRadioGroup{TValue}"/> and <see cref="InputRadio{TValue}"/> components.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>
/// <para>
/// There is 1 additional attribute that can be used: item-class.
/// It will apply CSS classes to the resulting element as per its name.
/// </para>
///
/// <para>
/// By default the <c>is-primary</c> class is applied for item-class.
/// Providing another Bulma color class will suppress the default so it can take effect.
/// </para>
/// </remarks>
public partial class InputRadioGroupObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<TValue>
{
	/// <summary>
	/// The options to generate radio buttons for. Keys are display text, values are any bindable object.
	/// </summary>
	/// <remarks>
	/// Supply either this dictionary or both <see cref="RadioOptions"/> and <see cref="DisplayValue"/>, but not both configurations.
	/// </remarks>
	[Parameter]
	public Dictionary<string, TValue?> Options { get; set; } = default!;

	/// <summary>
	/// The values to generate radio buttons for when using <see cref="DisplayValue"/> to provide their labels.
	/// </summary>
	/// <remarks>
	/// Requires <see cref="DisplayValue"/> and must not be supplied with <see cref="Options"/>.
	/// </remarks>
	[Parameter]
	public List<TValue>? RadioOptions { get; set; }

	/// <summary>
	/// Returns the display label for each value in <see cref="RadioOptions"/>.
	/// </summary>
	/// <remarks>
	/// Requires <see cref="RadioOptions"/> and must not be supplied with <see cref="Options"/>.
	/// </remarks>
	[Parameter]
	public Func<TValue, string>? DisplayValue { get; set; }

	/// <summary>
	/// A function to determine whether two items are equal.
	/// </summary>
	[Parameter]
	public Func<TValue?, TValue?, bool> AreEqual { get; set; } = EqualityComparer<TValue>.Default.Equals;

	private readonly string[] Filter = ["class", "item-class"];

	private readonly string PropertyName = Guid.NewGuid().ToHtmlId().ToString("N");
	private List<RadioItem> RadioItems = [];

	private string MainCssClass => CssClass;

	private string ItemCssClass
	{
		get
		{
			var css = string.Join(' ', "is-checkradio", AdditionalAttributes.GetValue("item-class"));

			if (CssClassHelper.ContainsColorClass(css) == false)
				css += " is-primary";

			return css;
		}
	}

	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Neither configuration is complete, or both configurations are supplied.</exception>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (Options != null)
		{
			if (RadioOptions != null || DisplayValue != null)
				throw new InvalidOperationException($"Supply either {nameof(Options)} or both {nameof(RadioOptions)} and {nameof(DisplayValue)}, but not both configurations.");

			RadioItems = Options.Select((x, index) => new RadioItem
			{
				Identifier = index.ToString(CultureInfo.InvariantCulture),
				Display = x.Key,
				Value = x.Value
			}).ToList();
		}
		else
		{
			if (RadioOptions == null || DisplayValue == null)
				throw new InvalidOperationException($"Supply either {nameof(Options)} or both {nameof(RadioOptions)} and {nameof(DisplayValue)}.");

			RadioItems = RadioOptions.Select((x, index) => new RadioItem
			{
				Identifier = index.ToString(CultureInfo.InvariantCulture),
				Display = DisplayValue(x),
				Value = x
			}).ToList();
		}
	}

	/// <inheritdoc/>
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	protected override string FormatValueAsString(TValue? value) => value switch
	{
		TValue currentValue => currentValue.ToString() ?? string.Empty,
		_ => string.Empty
	};

	private void OnCurrentChanged(TValue? current)
	{
		if (AdditionalAttributes.IsDisabled() == false)
			CurrentValue = current;
	}

	private string CurrentValueIdentifier
	{
		get
		{
			var match = RadioItems.FirstOrDefault(x => AreEqual(x.Value, Value));

			if (match != null)
				return match.Identifier;
			else
				return string.Empty;
		}
	}

	private string GetRadioOptionId(string identifier) => $"radio-InputRadioGroupObject-{PropertyName}-{identifier}";

	private sealed class RadioItem
	{
		/// <summary>
		/// Identifies an option independently of its display text within this component's current collection.
		/// </summary>
		public required string Identifier { get; init; }

		/// <summary>
		/// Provides the visible label, which may be shared by multiple options.
		/// </summary>
		public required string Display { get; init; }

		/// <summary>
		/// Holds the bound value assigned when this option is selected.
		/// </summary>
		public TValue? Value { get; init; }
	}
}

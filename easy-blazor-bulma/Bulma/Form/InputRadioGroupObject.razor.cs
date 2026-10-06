using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

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
	private IEnumerable<KeyValuePair<string, TValue?>> RadioItems = [];

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

			RadioItems = Options;
		}
		else
		{
			if (RadioOptions == null || DisplayValue == null)
				throw new InvalidOperationException($"Supply either {nameof(Options)} or both {nameof(RadioOptions)} and {nameof(DisplayValue)}.");

			RadioItems = RadioOptions.Select(x => new KeyValuePair<string, TValue?>(DisplayValue(x), x)).ToList();
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

	private string CurrentValueDisplay
	{
		get
		{
			var match = RadioItems.Select(x => new { x.Key, x.Value }).FirstOrDefault(x => AreEqual(x.Value, Value));

			if (match != null)
				return match.Key;
			else
				return string.Empty;
		}
	}

	private string GetRadioOptionId(string display) => $"radio-InputRadioGroupObject-{PropertyName}-{display.Replace(' ', '-')}";
}

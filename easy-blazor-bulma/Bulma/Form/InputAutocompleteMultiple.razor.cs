using easy_core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace easy_blazor_bulma;

/// <summary>
/// An input component for selecting multiple values from a list of options.
/// </summary>
/// <typeparam name="TItem">The type of suggestion displayed and searched.</typeparam>
/// <typeparam name="TValue">The type of value stored in the bound selection list.</typeparam>
/// <remarks>
/// <para>
/// There are 5 additional attributes that can be used: dropdown-class, dropdown-trigger-class, dropdown-menu-class, dropdown-item-class, and tag-class.
/// Each of which apply CSS classes to the resulting elements as per their names.
/// </para>
///
/// <para>
/// By default the <c>is-success</c> class is applied for tag-class.
/// Providing another Bulma color class will suppress the default so it can take effect.
/// </para>
///
/// <para>
/// <see href="https://bulma.io/documentation/components/dropdown/">Bulma Documentation</see>
/// </para>
/// </remarks>
public partial class InputAutocompleteMultiple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<List<TValue>>
{
	/// <summary>
	/// The collection of items to search when typing in the input.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required IEnumerable<TItem> Items { get; set; }

	/// <summary>
	/// Limits the number of items displayed in the drop-down list when set.
	/// </summary>
	[Parameter]
	public int? DisplayCount { get; set; }

	/// <summary>
	/// A function to return the values to display in the drop-down list.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required Func<TItem, string> DisplayValue { get; set; }

	/// <summary>
	/// Projects a suggestion from <see cref="Items"/> to the value stored in the bound selection list.
	/// </summary>
	/// <remarks>
	/// When omitted, <typeparamref name="TItem"/> and <typeparamref name="TValue"/> must be identical.
	/// </remarks>
	[Parameter]
	public Func<TItem, TValue>? ValueSelector { get; set; }

	/// <summary>
	/// A function to filter the display items.
	/// </summary>
	[Parameter]
	public Func<TItem, string?, bool>? DisplayFilter { get; set; }

	/// <summary>
	/// A function to determine whether two bound selection values are equal.
	/// </summary>
	[Parameter]
	public Func<TValue, TValue?, bool> AreEqual { get; set; } = EqualityComparer<TValue>.Default.Equals;

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; } = "search";

	/// <summary>
	/// Applies styles to the input according to the selected options.
	/// </summary>
	[Parameter]
	public InputStatus DisplayStatus { get; set; }

	/// <summary>
	/// The configuration options to apply to the component.
	/// </summary>
	[Parameter]
	public InputAutocompleteOptions Options { get; set; } =
		InputAutocompleteOptions.TypePopout |
		InputAutocompleteOptions.ClickPopout |
		InputAutocompleteOptions.PopoutBottom |
		InputAutocompleteOptions.PopoutLeft |
		InputAutocompleteOptions.UseAutomaticStatusColors;

	/// <summary>
	/// Fires with the oninput event just before updating the value of the input.
	/// </summary>
	[Parameter]
	public Func<string?, Task>? OnItemsRequested { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	[Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

	private readonly string[] Filter = ["class", "dropdown-class", "dropdown-trigger-class", "dropdown-menu-class", "dropdown-item-class", "tag-class"];

	private static readonly bool UsesLegacyIdentitySelection = typeof(TItem) == typeof(TValue);
	private ILogger<InputAutocompleteMultiple<TItem, TValue>>? Logger;

	private bool IsPopoutDisplayed;
	private TValue? HighlightedValue;
	private string? InputValue;

	private bool OnKeyDownPreventDefault;
	private bool OnBlurPreventDefault;
	private readonly string[] DefaultKeys = ["Escape", "ArrowDown", "ArrowUp", "Enter"];

	private string MainCssClass
	{
		get
		{
			var css = "input";

			if (DisplayStatus.HasFlag(InputStatus.BackgroundDanger))
				css += " is-danger";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundWarning))
				css += " is-warning";
			else if (DisplayStatus.HasFlag(InputStatus.BackgroundSuccess))
				css += " is-success";

			return string.Join(' ', css, CssClass);
		}
	}

	private string DropDownCssClass
	{
		get
		{
			var css = "dropdown dropdown-block";

			if (IsPopoutDisplayed && AdditionalAttributes.IsDisabled() == false)
				css += " is-active";

			if (Options.HasFlag(InputAutocompleteOptions.PopoutTop))
				css += " is-up";

			if (Options.HasFlag(InputAutocompleteOptions.PopoutRight))
				css += " is-right";

			if (Options.HasFlag(InputAutocompleteOptions.HoverPopout))
				css += " is-hoverable";

			return string.Join(' ', css, AdditionalAttributes.GetValue("dropdown-class"));
		}
	}

	private string DropDownTriggerCssClass => string.Join(' ', "dropdown-trigger", AdditionalAttributes.GetValue("dropdown-trigger-class"));

	private string DropDownMenuCssClass => string.Join(' ', "dropdown-menu p-0", AdditionalAttributes.GetValue("dropdown-menu-class"));

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
	protected override void OnInitialized()
	{
		// Get services
		Logger = ServiceProvider.GetService<ILogger<InputAutocompleteMultiple<TItem, TValue>>>();

		// Validation
		if (Options.HasAnyFlag(InputAutocompleteOptions.ClickPopout | InputAutocompleteOptions.TypePopout | InputAutocompleteOptions.HoverPopout) == false)
			Logger?.LogWarning("Must set at least one of ClickPopout, TypePopout, or HoverPopout for InputAutocompleteMultiple to work correctly.");

		// Set required options
		if (Options.HasAnyFlag(InputAutocompleteOptions.AutoSelectOnExit | InputAutocompleteOptions.AutoSelectOnInput) == false && Options.HasAnyFlag(InputAutocompleteOptions.AutoSelectCurrent | InputAutocompleteOptions.AutoSelectExact | InputAutocompleteOptions.AutoSelectClosest))
			Options |= InputAutocompleteOptions.AutoSelectOnExit;

		// Unset invalid options
		if (Options.HasFlag(InputAutocompleteOptions.PopoutLeft | InputAutocompleteOptions.PopoutRight))
			Options &= ~InputAutocompleteOptions.PopoutRight;

		if (Options.HasFlag(InputAutocompleteOptions.PopoutTop | InputAutocompleteOptions.PopoutBottom))
			Options &= ~InputAutocompleteOptions.PopoutTop;

		if (Options.HasFlag(InputAutocompleteOptions.AutoSelectCurrent | InputAutocompleteOptions.AutoSelectExact))
			Options &= ~InputAutocompleteOptions.AutoSelectCurrent;

		if (Options.HasFlag(InputAutocompleteOptions.AutoSelectCurrent | InputAutocompleteOptions.AutoSelectClosest))
			Options &= ~InputAutocompleteOptions.AutoSelectClosest;

		if (Options.HasFlag(InputAutocompleteOptions.AutoSelectExact | InputAutocompleteOptions.AutoSelectClosest))
			Options &= ~InputAutocompleteOptions.AutoSelectClosest;

		// Set starting values
		if (Value != null && Value.Count > 0)
			HighlightedValue = Value[^1];
	}

	/// <inheritdoc />
	/// <exception cref="InvalidOperationException">A selector is missing when item and value types differ.</exception>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (ValueSelector == null && UsesLegacyIdentitySelection == false)
			throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");
	}

	/// <inheritdoc />
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out List<TValue> result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		result = Value ?? [];
		validationErrorMessage = null;

		return true;
	}

	private void OnFocus(FocusEventArgs args)
	{
		if (Options.HasFlag(InputAutocompleteOptions.ClickPopout) && Items.Any())
			IsPopoutDisplayed = true;
	}

	private void OnBlur(FocusEventArgs args)
	{
		if (OnBlurPreventDefault)
			return;

		IsPopoutDisplayed = false;
	}

	private void OnClick(MouseEventArgs args)
	{
		if (Options.HasFlag(InputAutocompleteOptions.ClickPopout) && Items.Any())
			IsPopoutDisplayed = true;
	}

	private async Task OnInput(ChangeEventArgs args)
	{
		if (Options.HasFlag(InputAutocompleteOptions.TypePopout) && Items.Any())
			IsPopoutDisplayed = true;

		var changed = args.Value?.ToString();

		if (OnItemsRequested != null)
			await OnItemsRequested.Invoke(changed);

		InputValue = changed;
	}

	private void OnKeyUp(KeyboardEventArgs args)
	{
		if (args.Code == "Enter")
		{
			var match = GetMatch(InputValue, InputAutocompleteOptions.AutoSelectCurrent);
			OnItemSelected(match.match, success: match.success);
		}

		if (args.Code == "Enter" || args.Code == "Escape")
			IsPopoutDisplayed = false;
	}

	private void OnKeyDown(KeyboardEventArgs args)
	{
		OnKeyDownPreventDefault = DefaultKeys.Contains(args.Code);

		if (Options.HasFlag(InputAutocompleteOptions.TypePopout) && Items.Any() && (args.Code == "ArrowDown" || args.Code == "ArrowUp"))
			IsPopoutDisplayed = true;

		if (IsPopoutDisplayed && args.Code == "ArrowDown")
			HighlightNext();
		else if (IsPopoutDisplayed && args.Code == "ArrowUp")
			HighlightPrevious();

		if (IsPopoutDisplayed && args.Code == "Tab")
		{
			if (Options.HasFlag(InputAutocompleteOptions.AutoSelectOnExit))
			{
				var match = GetMatch(InputValue, InputAutocompleteOptions.AutoSelectCurrent);
				OnItemSelected(match.match, success: match.success);
			}
			else
			{
				IsPopoutDisplayed = false;
			}
		}
	}

	private void OnMouseDown(MouseEventArgs args)
	{
		OnBlurPreventDefault = true;
	}

	private void OnMouseUp(MouseEventArgs args)
	{
		OnBlurPreventDefault = false;
	}

	private void OnItemSelected(TValue? value, bool close = true, bool success = true)
	{
		var changed = false;

		if (value != null)
		{
			CurrentValue ??= [];

			if (Options.HasFlag(InputAutocompleteOptions.AllowSameItem) || Value!.Any(x => AreEqual(x, value)) == false)
			{
				Value!.Add(value);
				HighlightedValue = value;

				changed = true;
			}
		}

		InputValue = null;

		if (changed && Value != null)
			_ = ValueChanged.InvokeAsync(Value);

		EditContext.Validate();
		EditContext.NotifyFieldChanged(FieldIdentifier);

		if (close)
			IsPopoutDisplayed = false;

		if (Options.HasFlag(InputAutocompleteOptions.UseAutomaticStatusColors))
		{
			ResetStatus();
			DisplayStatus |= success ? InputStatus.BackgroundSuccess : InputStatus.BackgroundDanger;
		}
	}

	private void OnItemRemoved(int index)
	{
		if (Value == null || index < 0 || index >= Value.Count)
			return;

		Value.RemoveAt(index);
		_ = ValueChanged.InvokeAsync(Value);

		EditContext.NotifyFieldChanged(FieldIdentifier);

		if (Options.HasFlag(InputAutocompleteOptions.UseAutomaticStatusColors))
		{
			ResetStatus();
			DisplayStatus |= InputStatus.BackgroundSuccess;
		}
	}

	private IEnumerable<TItem> GetDisplayItems()
	{
		if (DisplayFilter != null && InputValue != null && DisplayCount > 0)
			return Items.Where(x => DisplayFilter.Invoke(x, InputValue)).Take(DisplayCount.Value);
		else if (DisplayFilter != null && InputValue != null)
			return Items.Where(x => DisplayFilter.Invoke(x, InputValue));
		else if (DisplayCount > 0)
			return Items.Take(DisplayCount.Value);
		else
			return Items;
	}

	private void HighlightNext()
	{
		if (HighlightedValue == null)
		{
			var highlighted = GetDisplayItems().FirstOrDefault();

			HighlightedValue = highlighted != null ? GetValue(highlighted) : default;
			return;
		}

		TItem? next = default;
		TItem? first = default;
		bool takeNext = false;

		foreach (var item in GetDisplayItems())
		{
			first ??= item;

			if (takeNext)
			{
				next = item;
				break;
			}

			if (AreEqual(GetValue(item), HighlightedValue))
				takeNext = true;
		}

		next ??= first;
		HighlightedValue = next != null ? GetValue(next) : default;
	}

	private void HighlightPrevious()
	{
		if (HighlightedValue == null)
		{
			var highlighted = GetDisplayItems().LastOrDefault();

			HighlightedValue = highlighted != null ? GetValue(highlighted) : default;
			return;
		}

		TItem? previous = default;

		foreach (var item in GetDisplayItems())
		{
			if (previous != null && AreEqual(GetValue(item), HighlightedValue))
				break;

			previous = item;
		}

		HighlightedValue = previous != null ? GetValue(previous) : default;
	}

	private void ResetStatus()
	{
		DisplayStatus &= ~InputStatus.BackgroundDanger;
		DisplayStatus &= ~InputStatus.BackgroundWarning;
		DisplayStatus &= ~InputStatus.BackgroundSuccess;
	}

	private (bool success, TValue? match) GetMatch(string? value, InputAutocompleteOptions? matchType = null)
	{
		if (matchType == null)
		{
			if (Options.HasFlag(InputAutocompleteOptions.AutoSelectCurrent))
				matchType = InputAutocompleteOptions.AutoSelectCurrent;
			else if (Options.HasFlag(InputAutocompleteOptions.AutoSelectExact))
				matchType = InputAutocompleteOptions.AutoSelectExact;
			else if (Options.HasFlag(InputAutocompleteOptions.AutoSelectClosest))
				matchType = InputAutocompleteOptions.AutoSelectClosest;
		}

		TItem? match = default;

		if (matchType == InputAutocompleteOptions.AutoSelectCurrent && HighlightedValue != null)
			return (true, HighlightedValue);
		else if (matchType == InputAutocompleteOptions.AutoSelectExact)
			match = GetDisplayItems().FirstOrDefault(x => string.Equals(DisplayValue(x), value, StringComparison.OrdinalIgnoreCase));
		else if (matchType == InputAutocompleteOptions.AutoSelectClosest)
			match = GetDisplayItems().OrderBy(x => string.Compare(DisplayValue(x), value, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

		if (match != null)
			return (true, GetValue(match));
		else
			return (false, default);
	}

	private string GetDropDownItemCssClass(TItem item)
	{
		var css = "dropdown-item is-clickable";

		if (HighlightedValue != null || CurrentValue != null)
		{
			var value = GetValue(item);

			if (HighlightedValue != null && AreEqual(value, HighlightedValue))
				css += " has-background-default";

			if (CurrentValue != null && CurrentValue.Any(x => AreEqual(value, x)) == true)
				css += " has-text-success";
		}

		return string.Join(' ', css, AdditionalAttributes.GetValue("dropdown-item-class"));
	}

	private TValue GetValue(TItem item)
	{
		if (ValueSelector != null)
			return ValueSelector(item);

		if (UsesLegacyIdentitySelection)
			return (TValue)(object?)item!;

		throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");
	}

	private string GetItemDisplayValue(TValue value)
	{
		if (UsesLegacyIdentitySelection)
			return DisplayValue((TItem)(object?)value!);

		foreach (var item in Items)
			if (AreEqual(GetValue(item), value))
				return DisplayValue(item);

		return string.Empty;
	}
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a select list with the provided items for selection of multiple objects.
/// </summary>
/// <typeparam name="TItem">The type of item displayed in the list.</typeparam>
/// <typeparam name="TValue">The type of value stored in the bound selection.</typeparam>
/// <remarks>
/// There is 1 additional attribute that can be used: button-class. This applies CSS classes to the "Select All" and "Clear All" buttons.
/// <see href="https://bulma.io/documentation/form/select/">Bulma Documentation</see>
/// </remarks>
public partial class InputSelectMultipleObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputBase<List<TValue>>
{
	/// <summary>
	/// The collection of items to display in the list.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required IEnumerable<TItem> Items { get; set; }
	private IReadOnlyList<TItem> ItemsList = [];

	/// <summary>
	/// A function to return the values to display in the drop-down list.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public required Func<TItem, string> DisplayValue { get; set; }

	/// <summary>
	/// A function to return the bound value for items provided in the <see cref="Items"/> collection.
	/// </summary>
	/// <remarks>
	/// When omitted, <typeparamref name="TItem"/> and <typeparamref name="TValue"/> must be identical.
	/// </remarks>
	[Parameter]
	public Func<TItem, TValue>? ValueSelector { get; set; }

	/// <summary>
	/// A function to determine an item is selected.
	/// </summary>
	[Parameter]
	public Func<ICollection<TValue>?, TValue, bool> Contains { get; set; } = (x, y) => x != null && x.Contains(y, EqualityComparer<TValue>.Default);

	/// <summary>
	/// An icon to display within the input.
	/// </summary>
	[Parameter]
	public string? Icon { get; set; }

	/// <summary>
	/// The number of items to display in the select box.
	/// </summary>
	[Parameter]
	[Range(2, 100)]
	public int Size { get; set; } = 8;

	/// <summary>
	/// Specifies whether to show the "Select All" and "Clear All" buttons.
	/// </summary>
	[Parameter]
	public bool ShowGroupButtons { get; set; }

	/// <summary>
	/// Gets or sets the associated <see cref="ElementReference"/>.
	/// <para>
	/// May be <see langword="null"/> if accessed before the component is rendered.
	/// </para>
	/// </summary>
	[DisallowNull]
	public ElementReference? Element { get; private set; }

	private readonly string[] Filter = ["class", "button-class"];

	private static readonly bool UsesLegacyIdentitySelection = typeof(TItem) == typeof(TValue);

	private string MainCssClass => string.Join(' ', "select is-multiple", CssClass);
	private string GroupButtonCssClass => string.Join(' ', "button is-small is-fullwidth mt-2", AdditionalAttributes.GetValue("button-class"));

	private string[] SelectedIndices
	{
		get
		{
			var indices = new List<string>();

			for (var i = 0; i < ItemsList.Count; i++)
				if (Contains(Value, GetValue(ItemsList[i])))
					indices.Add(i.ToString(CultureInfo.InvariantCulture));

			return indices.ToArray();
		}
		set
		{
			var selections = new List<TValue>();

			foreach (var index in value)
			{
				if (int.TryParse(index, NumberStyles.None, CultureInfo.InvariantCulture, out var i) == false || i < 0 || i >= ItemsList.Count)
					return;

				var selected = GetValue(ItemsList[i]);

				if (Contains(selections, selected) == false)
					selections.Add(selected);
			}

			CurrentValue = selections;
		}
	}

	/// <inheritdoc />
	/// <exception cref="NotSupportedException">A multiple selection is represented by an array of indices, not a single string.</exception>
	protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out List<TValue> result, [NotNullWhen(false)] out string? validationErrorMessage)
	{
		throw new NotSupportedException($"{GetType()} does not support parsing a single string. Bind to the selected indices instead.");
	}

	/// <inheritdoc />
	/// <exception cref="InvalidOperationException">A selector is missing when item and value types differ.</exception>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		if (ValueSelector == null && UsesLegacyIdentitySelection == false)
			throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");

		ItemsList = Items as IReadOnlyList<TItem> ?? Items.ToList();
	}

	/// <summary>
	/// Requests a render after the bound selection is modified in place outside the component.
	/// </summary>
	/// <remarks>
	/// Recalculates selected indices without replacing the select element or notifying the edit context.
	/// </remarks>
	/// <exception cref="InvalidOperationException">The component has not been attached to a renderer or the caller is not on its synchronization context.</exception>
	public void NotifySelectionChanged()
	{
		StateHasChanged();
	}

	/// <summary>
	/// Selects all items in the list.
	/// </summary>
	public void SelectAll()
	{
		SelectedIndices = Enumerable.Range(0, ItemsList.Count).Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
	}

	/// <summary>
	/// Clears all selections in the list.
	/// </summary>
	public void ClearAll()
	{
		CurrentValue = [];
	}

	private TValue GetValue(TItem item)
	{
		if (ValueSelector != null)
			return ValueSelector(item);

		if (UsesLegacyIdentitySelection)
			return (TValue)(object?)item!;

		throw new InvalidOperationException($"{nameof(ValueSelector)} is required when {nameof(TItem)} and {nameof(TValue)} are different types.");
	}
}

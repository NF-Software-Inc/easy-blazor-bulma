using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies multi-selection binds projected values without losing same-type selection support.
/// </summary>
public class InputSelectMultipleObjectTests
{
	/// <summary>
	/// Verifies native selection snapshots replace the bound values and notify once per change.
	/// </summary>
	[Fact]
	public void SelectionSnapshotsProjectValuesAndNotify()
	{
		var input = CreateProjectedInput();
		var notifications = 0;
		input.ValueChanged = EventCallback.Factory.Create<List<int>>(this, _ => notifications++);

		SetSelectedIndices(input, "1");
		Assert.Equal(new[] { 20 }, input.Value);

		SetSelectedIndices(input);
		Assert.Empty(input.Value!);

		SetSelectedIndices(input, "1", "2");
		Assert.Equal(new[] { 20, 30 }, input.Value);
		Assert.Equal(3, notifications);
	}

	/// <summary>
	/// Verifies selections are represented by indices independently of identical display labels.
	/// </summary>
	[Fact]
	public void BoundValuesFormatAsSelectedIndices()
	{
		var input = CreateProjectedInput();
		Assert.Empty(GetSelectedIndices(input));

		input.Value = [10, 30];
		Assert.Equal(new[] { "0", "2" }, GetSelectedIndices(input));

		input.Value[0] = 20;
		Assert.Equal(new[] { "1", "2" }, GetSelectedIndices(input));
	}

	/// <summary>
	/// Verifies selection replaces the list without mutating the previous collection or retaining hidden values.
	/// </summary>
	[Fact]
	public void SelectionReplacesPreviousCollection()
	{
		var input = CreateProjectedInput();
		var previousSelection = new List<int> { 10, 999 };
		input.Value = previousSelection;

		SetSelectedIndices(input, "1");
		Assert.Equal(new[] { 20 }, input.Value);
		Assert.NotSame(previousSelection, input.Value);
		Assert.Equal(new[] { 10, 999 }, previousSelection);
	}

	/// <summary>
	/// Verifies Select All and Clear All operate on the bound value collection.
	/// </summary>
	[Fact]
	public void GroupButtonsProjectValuesAndNotify()
	{
		var input = CreateProjectedInput();
		List<int>? notifiedValues = null;
		var notifications = 0;
		input.ValueChanged = EventCallback.Factory.Create<List<int>>(this, values =>
		{
			notifiedValues = values;
			notifications++;
		});

		input.SelectAll();
		Assert.Equal(new[] { 10, 20, 30 }, input.Value);
		Assert.Same(input.Value, notifiedValues);
		var selectedValues = input.Value;

		input.ClearAll();
		Assert.Empty(input.Value!);
		Assert.Same(input.Value, notifiedValues);
		Assert.NotSame(selectedValues, input.Value);
		Assert.Equal(new[] { 10, 20, 30 }, selectedValues);
		Assert.Equal(2, notifications);
	}

	/// <summary>
	/// Verifies a nullable projected value is a selectable value, not an absent item.
	/// </summary>
	[Fact]
	public void NullableProjectedValuesAreSupported()
	{
		var input = new TestInput<SelectionItem, int?>
		{
			Items = [new SelectionItem(10), new SelectionItem(20)],
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id == 10 ? null : x.Id
		};

		input.Initialize();
		input.SelectAll();

		Assert.Equal(new int?[] { null, 20 }, input.Value);
		Assert.Equal(new[] { "0", "1" }, GetSelectedIndices(input));
		SetSelectedIndices(input, "1");
		Assert.Equal(new int?[] { 20 }, input.Value);
	}

	/// <summary>
	/// Verifies unchanged item/value types preserve object identity without a selector.
	/// </summary>
	[Fact]
	public void SameTypesDoNotRequireSelector()
	{
		var item = new SelectionItem(10);
		var input = new TestInput<SelectionItem, SelectionItem>
		{
			Items = [item],
			DisplayValue = x => x.Name
		};
		input.Initialize();

		SetSelectedIndices(input, "0");
		Assert.Same(item, Assert.Single(input.Value!));
		SetSelectedIndices(input);
		Assert.Empty(input.Value!);
	}

	/// <summary>
	/// Verifies an explicit selector takes precedence even when generic types match.
	/// </summary>
	[Fact]
	public void SameTypesCanUseSelector()
	{
		var input = new TestInput<int, int>
		{
			Items = [1, 2],
			DisplayValue = x => x.ToString(),
			ValueSelector = x => x * 10
		};
		input.Initialize();
		input.SelectAll();

		Assert.Equal(new[] { 10, 20 }, input.Value);
	}

	/// <summary>
	/// Verifies mismatched types reject a missing selector during parameter validation.
	/// </summary>
	[Fact]
	public void DifferentTypesRequireSelector()
	{
		var input = new TestInput<SelectionItem, int>
		{
			Items = [],
			DisplayValue = x => x.Name
		};

		var exception = Assert.Throws<InvalidOperationException>(input.Initialize);
		Assert.Contains("ValueSelector is required", exception.Message);
	}

	/// <summary>
	/// Verifies custom membership checks receive projected values rather than items.
	/// </summary>
	[Fact]
	public void ContainsReceivesProjectedValue()
	{
		var input = CreateProjectedInput();
		input.Value = [120];
		input.Contains = (values, value) => values != null && values.Any(x => x % 100 == value);

		Assert.Equal(new[] { "1" }, GetSelectedIndices(input));
		SetSelectedIndices(input, "1");
		Assert.Equal(new[] { 20 }, input.Value);
	}

	/// <summary>
	/// Verifies separate instances retain their own selectors and selection state.
	/// </summary>
	[Fact]
	public void InstancesKeepIndependentSelectionState()
	{
		var first = CreateProjectedInput();
		var second = CreateProjectedInput();
		second.ValueSelector = x => x.Id + 100;
		second.Initialize();

		SetSelectedIndices(first, "0");
		SetSelectedIndices(second, "1");

		Assert.Equal(new[] { 10 }, first.Value);
		Assert.Equal(new[] { 120 }, second.Value);
	}

	/// <summary>
	/// Verifies malformed or out-of-range snapshots do not partially overwrite the previous selection.
	/// </summary>
	/// <param name="invalidIndex">An index that cannot identify an available option.</param>
	[Theory]
	[InlineData("invalid")]
	[InlineData("-1")]
	[InlineData("3")]
	[InlineData("")]
	[InlineData(" 1")]
	[InlineData("+1")]
	public void InvalidIndicesPreserveSelection(string invalidIndex)
	{
		var input = CreateProjectedInput();
		var previousSelection = new List<int> { 30 };
		input.Value = previousSelection;
		var notifications = 0;
		input.ValueChanged = EventCallback.Factory.Create<List<int>>(this, _ => notifications++);

		SetSelectedIndices(input, "0", invalidIndex);

		Assert.Same(previousSelection, input.Value);
		Assert.Equal(new[] { 30 }, input.Value);
		Assert.Equal(0, notifications);
	}

	/// <summary>
	/// Verifies repeated option indices do not create duplicate bound values.
	/// </summary>
	[Fact]
	public void RepeatedIndicesAreDeduplicated()
	{
		var input = CreateProjectedInput();

		SetSelectedIndices(input, "0", "0", "1");

		Assert.Equal(new[] { 10, 20 }, input.Value);
	}

	/// <summary>
	/// Verifies snapshots and group selection suppress duplicates using the configured membership function.
	/// </summary>
	[Fact]
	public void DuplicateValuesRespectContains()
	{
		var input = new TestInput<string, string>
		{
			Items = new[] { "alpha", "ALPHA", "beta" },
			DisplayValue = x => x,
			Contains = (values, value) => values != null && values.Contains(value, StringComparer.OrdinalIgnoreCase)
		};
		input.Initialize();

		SetSelectedIndices(input, "0", "1", "2");
		Assert.Equal(new[] { "alpha", "beta" }, input.Value);
		Assert.Equal(new[] { "0", "1", "2" }, GetSelectedIndices(input));

		input.SelectAll();
		Assert.Equal(new[] { "alpha", "beta" }, input.Value);
	}

	/// <summary>
	/// Verifies an empty item collection supports empty snapshots and group actions.
	/// </summary>
	[Fact]
	public void EmptyItemsSupportEmptySelection()
	{
		var input = CreateProjectedInput();
		input.Items = Array.Empty<SelectionItem>();
		input.Initialize();

		SetSelectedIndices(input);
		Assert.Empty(input.Value!);
		Assert.Empty(GetSelectedIndices(input));
		input.SelectAll();
		Assert.Empty(input.Value!);
		input.ClearAll();
		Assert.Empty(input.Value!);
	}

	private static TestInput<SelectionItem, int> CreateProjectedInput()
	{
		var input = new TestInput<SelectionItem, int>
		{
			Items = new[] { 10, 20, 30 }.Select(x => new SelectionItem(x)),
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id
		};
		input.Initialize();
		return input;
	}

	private static string[] GetSelectedIndices<TItem, TValue>(TestInput<TItem, TValue> input)
	{
		var property = typeof(InputSelectMultipleObject<TItem, TValue>).GetProperty("SelectedIndices", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(property);
		return Assert.IsType<string[]>(property.GetValue(input));
	}

	private static void SetSelectedIndices<TItem, TValue>(TestInput<TItem, TValue> input, params string[] indices)
	{
		var property = typeof(InputSelectMultipleObject<TItem, TValue>).GetProperty("SelectedIndices", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(property);
		property.SetValue(input, indices);
	}

	private sealed record SelectionItem(int Id)
	{
		// Deliberately identical display text: selection must not depend on labels.
		public string Name => "Applicant";
	}

	private sealed class TestInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputSelectMultipleObject<TItem, TValue>
	{
		public void Initialize() => OnParametersSet();
	}
}

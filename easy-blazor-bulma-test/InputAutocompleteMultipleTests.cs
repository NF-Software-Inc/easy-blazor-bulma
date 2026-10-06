using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies multi-autocomplete suggestion items can differ from the bound selection values.
/// </summary>
public class InputAutocompleteMultipleTests
{
	/// <summary>
	/// Verifies exact and closest matching return projected IDs using the existing matching rules.
	/// </summary>
	[Fact]
	public void MatchingProjectsSuggestionValues()
	{
		var input = CreateProjectedInput();

		var exact = Match(input, "beta", InputAutocompleteOptions.AutoSelectExact);
		Assert.True(exact.success);
		Assert.Equal(20, exact.match);

		var closest = Match(input, "Alpha", InputAutocompleteOptions.AutoSelectClosest);
		Assert.True(closest.success);
		Assert.Equal(10, closest.match);

		var missing = Match(input, "Missing", InputAutocompleteOptions.AutoSelectExact);
		Assert.False(missing.success);
	}

	/// <summary>
	/// Verifies display filtering and limits operate on suggestion objects rather than IDs.
	/// </summary>
	[Fact]
	public void DisplayFilterReceivesItems()
	{
		var input = CreateProjectedInput();
		input.DisplayFilter = (item, text) => item.Name.StartsWith(text ?? string.Empty, StringComparison.OrdinalIgnoreCase);
		input.DisplayCount = 1;
		SetPrivateField(input, "InputValue", "B");

		var items = Assert.IsAssignableFrom<IEnumerable<Suggestion>>(Invoke(input, "GetDisplayItems"));
		Assert.Equal(20, Assert.Single(items).Id);
		Assert.Equal(20, Match(input, "Beta", InputAutocompleteOptions.AutoSelectExact).match);
	}

	/// <summary>
	/// Verifies tag labels resolve projected values using the full item collection and configured equality.
	/// </summary>
	[Fact]
	public void TagLabelsResolveProjectedValues()
	{
		var input = CreateProjectedInput();
		input.AreEqual = (x, y) => x % 100 == y % 100;
		input.DisplayFilter = (item, _) => item.Id == 10;
		SetPrivateField(input, "InputValue", "Alpha");

		Assert.Equal("Beta", Label(input, 120));
		Assert.Equal(string.Empty, Label(input, 999));
	}

	/// <summary>
	/// Verifies suggestion selection mutates the bound ID list and notifies binding only for added values.
	/// </summary>
	[Fact]
	public void SelectionProjectsValuesAndSuppressesDuplicates()
	{
		var input = CreateProjectedInput();
		var original = input.Value;
		var notifications = 0;
		List<int>? notifiedValues = null;
		input.ValueChanged = EventCallback.Factory.Create<List<int>>(this, values =>
		{
			notifications++;
			notifiedValues = values;
		});

		var value = Assert.IsType<int>(Invoke(input, "GetValue", new Suggestion(20, "Beta")));
		Invoke(input, "OnItemSelected", value, true, true);
		Invoke(input, "OnItemSelected", value, true, true);

		Assert.Equal(new[] { 20 }, input.Value);
		Assert.Same(original, input.Value);
		Assert.Same(input.Value, notifiedValues);
		Assert.Equal(1, notifications);
	}

	/// <summary>
	/// Verifies AllowSameItem preserves repeated projected values in the bound list.
	/// </summary>
	[Fact]
	public void AllowSameItemPreservesDuplicateValues()
	{
		var input = CreateProjectedInput();
		input.Options |= InputAutocompleteOptions.AllowSameItem;

		Invoke(input, "OnItemSelected", 20, true, true);
		Invoke(input, "OnItemSelected", 20, true, true);

		Assert.Equal(new[] { 20, 20 }, input.Value);
	}

	/// <summary>
	/// Verifies duplicate detection and suggestion styling compare projected bound values.
	/// </summary>
	[Fact]
	public void EqualityUsesProjectedValues()
	{
		var input = CreateProjectedInput([120]);
		input.AreEqual = (x, y) => x % 100 == y % 100;

		Invoke(input, "OnItemSelected", 20, true, true);
		Assert.Equal(new[] { 120 }, input.Value);

		var css = Assert.IsType<string>(Invoke(input, "GetDropDownItemCssClass", new Suggestion(20, "Beta")));
		Assert.Contains("has-text-success", css);
		Assert.Contains("has-background-default", css);
	}

	/// <summary>
	/// Verifies removing a tag removes its bound value by index and notifies binding.
	/// </summary>
	[Fact]
	public void RemovalUsesBoundValueIndex()
	{
		var input = CreateProjectedInput([10, 20, 20]);
		var notifications = 0;
		input.ValueChanged = EventCallback.Factory.Create<List<int>>(this, _ => notifications++);

		Invoke(input, "OnItemRemoved", 1);
		Assert.Equal(new[] { 10, 20 }, input.Value);
		Invoke(input, "OnItemRemoved", -1);
		Invoke(input, "OnItemRemoved", 2);
		Assert.Equal(new[] { 10, 20 }, input.Value);
		Assert.Equal(1, notifications);
	}

	/// <summary>
	/// Verifies keyboard navigation projects suggestion values and preserves wraparound and duplicate suppression.
	/// </summary>
	[Fact]
	public void KeyboardSelectionProjectsAndWraps()
	{
		var input = CreateProjectedInput([99]);

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowDown" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(new[] { 99, 10 }, input.Value);

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowUp" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(new[] { 99, 10, 30 }, input.Value);

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowDown" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(new[] { 99, 10, 30 }, input.Value);
	}

	/// <summary>
	/// Verifies keyboard navigation can select zero as a projected value.
	/// </summary>
	[Fact]
	public void KeyboardSelectionSupportsProjectedZero()
	{
		var input = CreateProjectedInput([99]);
		input.Items = [new Suggestion(0, "Zero"), new Suggestion(20, "Beta")];

		Invoke(input, "HighlightNext");
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Invoke(input, "HighlightNext");
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });

		Assert.Equal(new[] { 99, 0, 20 }, input.Value);
	}

	/// <summary>
	/// Verifies current-highlight matching remains independent of the available suggestions.
	/// </summary>
	[Fact]
	public void CurrentHighlightRemainsAvailableWithoutSuggestions()
	{
		var input = CreateProjectedInput([99]);
		Invoke(input, "HighlightNext");
		input.Items = [];

		var match = Match(input, "Alpha", InputAutocompleteOptions.AutoSelectCurrent);
		Assert.True(match.success);
		Assert.Equal(10, match.match);
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(new[] { 99, 10 }, input.Value);
	}

	/// <summary>
	/// Verifies closest matching retains its existing failure result when the reference-type item collection is empty.
	/// </summary>
	[Fact]
	public void EmptyClosestMatchPreservesExistingFailureRule()
	{
		var input = CreateProjectedInput();
		input.Items = [];

		var match = Match(input, "Missing", InputAutocompleteOptions.AutoSelectClosest);
		Assert.False(match.success);
		Assert.Equal(0, match.match);
	}

	/// <summary>
	/// Verifies identical generic types need no selector and preserve direct labels for absent items.
	/// </summary>
	[Fact]
	public void SameTypesPreserveIdentityAndDirectLabels()
	{
		var item = new Suggestion(10, "Alpha");
		var input = new TestInput<Suggestion, Suggestion>
		{
			Items = [item],
			DisplayValue = x => x.Name,
			Value = []
		};
		input.Initialize();

		var match = Match(input, "Alpha", InputAutocompleteOptions.AutoSelectExact);
		Assert.True(match.success);
		Assert.Same(item, match.match);
		Invoke(input, "OnItemSelected", match.match, true, true);
		Assert.Same(item, Assert.Single(input.Value!));
		input.Items = [];
		Assert.Equal("Alpha", Label(input, item));
	}

	/// <summary>
	/// Verifies same-type selection uses ValueSelector while tag formatting directly displays the bound value.
	/// </summary>
	[Fact]
	public void SameTypesCanUseSelector()
	{
		var input = new TestInput<int, int>
		{
			Items = [1, 2],
			DisplayValue = x => x.ToString(),
			ValueSelector = x => x * 10,
			Value = []
		};
		input.Initialize();

		var match = Match(input, "2", InputAutocompleteOptions.AutoSelectExact);
		Assert.True(match.success);
		Assert.Equal(20, match.match);
		Assert.Equal("20", Label(input, 20));
	}

	/// <summary>
	/// Verifies value-type suggestions retain the existing default-item result for missing exact matches.
	/// </summary>
	[Fact]
	public void ValueTypeItemsRetainDefaultMatchingBehavior()
	{
		var input = new TestInput<int, int>
		{
			Items = [0, 1],
			DisplayValue = x => x.ToString(),
			Value = []
		};
		input.Initialize();

		var match = Match(input, "Missing", InputAutocompleteOptions.AutoSelectExact);
		Assert.True(match.success);
		Assert.Equal(0, match.match);
	}

	/// <summary>
	/// Verifies nullable projected values retain the existing rule that null selections are not added.
	/// </summary>
	[Fact]
	public void NullableProjectedValuesPreserveNullSelectionBehavior()
	{
		var input = new TestInput<Suggestion, int?>
		{
			Items = [new Suggestion(20, "Beta"), new Suggestion(30, "Null")],
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id == 30 ? null : x.Id,
			Value = []
		};
		input.Initialize();

		Invoke(input, "OnItemSelected", 20, true, true);
		Invoke(input, "OnItemSelected", null!, true, true);
		Assert.Equal(new int?[] { 20 }, input.Value);
		Assert.Equal("Beta", Label(input, (int?)20));
	}

	/// <summary>
	/// Verifies item requests update suggestions and search text without changing input-event selection behavior.
	/// </summary>
	[Fact]
	public async Task RequestedItemsAreAvailableForProjectedMatching()
	{
		var input = CreateProjectedInput();
		input.DisplayFilter = (item, text) => item.Name.StartsWith(text ?? string.Empty, StringComparison.OrdinalIgnoreCase);
		input.OnItemsRequested = async text =>
		{
			await Task.Yield();
			input.Items = [new Suggestion(40, text!)];
		};

		await Assert.IsAssignableFrom<Task>(Invoke(input, "OnInput", new ChangeEventArgs { Value = "Updated" }));

		Assert.Empty(input.Value!);
		var match = Match(input, "Updated", InputAutocompleteOptions.AutoSelectExact);
		Assert.True(match.success);
		Assert.Equal(40, match.match);
	}

	/// <summary>
	/// Verifies distinct item and value types require a projection selector.
	/// </summary>
	[Fact]
	public void DifferentTypesRequireSelector()
	{
		var input = new TestInput<Suggestion, int>
		{
			Items = [],
			DisplayValue = x => x.Name,
			Value = []
		};

		var error = Assert.Throws<InvalidOperationException>(input.Initialize);
		Assert.Contains("ValueSelector is required", error.Message);
	}

	/// <summary>
	/// Verifies selectors, bound lists, and highlight state are independent across instances.
	/// </summary>
	[Fact]
	public void InstancesHaveIndependentSelectionState()
	{
		var first = CreateProjectedInput([99]);
		var second = CreateProjectedInput([99]);
		second.ValueSelector = x => x.Id + 100;

		Invoke(first, "HighlightNext");
		Invoke(first, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Invoke(second, "HighlightPrevious");
		Invoke(second, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });

		Assert.Equal(new[] { 99, 10 }, first.Value);
		Assert.Equal(new[] { 99, 130 }, second.Value);
	}

	private static TestInput<Suggestion, int> CreateProjectedInput(List<int>? values = null)
	{
		var input = new TestInput<Suggestion, int>
		{
			Items = [new Suggestion(10, "Alpha"), new Suggestion(20, "Beta"), new Suggestion(30, "Charlie")],
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id,
			Value = values ?? []
		};
		input.Initialize();
		return input;
	}

	private static (bool success, TValue? match) Match<TItem, TValue>(TestInput<TItem, TValue> input, string text, InputAutocompleteOptions matchType)
	{
		return Assert.IsType<(bool success, TValue? match)>(Invoke(input, "GetMatch", text, matchType));
	}

	private static string Label<TItem, TValue>(TestInput<TItem, TValue> input, TValue value)
	{
		return Assert.IsType<string>(Invoke(input, "GetItemDisplayValue", value!));
	}

	private static object? Invoke<TItem, TValue>(TestInput<TItem, TValue> input, string methodName, params object[] arguments)
	{
		var method = typeof(InputAutocompleteMultiple<TItem, TValue>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		return method.Invoke(input, arguments);
	}

	private static void SetPrivateField<TItem, TValue>(TestInput<TItem, TValue> input, string fieldName, object value)
	{
		var field = typeof(InputAutocompleteMultiple<TItem, TValue>).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		field.SetValue(input, value);
	}

	private sealed record Suggestion(int Id, string Name);

	private sealed class EmptyServiceProvider : IServiceProvider
	{
		/// <summary>
		/// Supplies no optional services to the standalone test component.
		/// </summary>
		/// <param name="serviceType">The requested service type.</param>
		public object? GetService(Type serviceType) => null;
	}

	private sealed class TestInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputAutocompleteMultiple<TItem, TValue>
	{
		private readonly FieldModel Model = new();

		/// <summary>
		/// Supplies the edit context and field required by selection and removal before initializing the component.
		/// </summary>
		public void Initialize()
		{
			Model.Value = Value ?? [];
			EditContext = new EditContext(Model);
			FieldIdentifier = new FieldIdentifier(Model, nameof(FieldModel.Value));
			ValueExpression = () => Model.Value;

			var property = typeof(InputAutocompleteMultiple<TItem, TValue>).GetProperty("ServiceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.NotNull(property);
			property.SetValue(this, new EmptyServiceProvider());
			OnInitialized();
			OnParametersSet();
		}

		private sealed class FieldModel
		{
			/// <summary>
			/// Represents the selected-value list used for validation and field notifications.
			/// </summary>
			public List<TValue> Value { get; set; } = [];
		}
	}
}

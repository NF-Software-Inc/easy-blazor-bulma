using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies autocomplete suggestions can display items independently of their bound values.
/// </summary>
public class InputAutocompleteTests
{
	/// <summary>
	/// Verifies exact text matching projects an object suggestion to its ID.
	/// </summary>
	[Fact]
	public void ExactMatchProjectsSuggestionValue()
	{
		var input = CreateProjectedInput();

		Assert.True(input.Parse("beta", out var value, out var error), error);
		Assert.Equal(20, value);
		Assert.Equal("Beta", input.Format(value));
	}

	/// <summary>
	/// Verifies bound labels resolve through all items instead of the filtered dropdown.
	/// </summary>
	[Fact]
	public void SelectedLabelUsesItemsAndProjectedEquality()
	{
		var input = CreateProjectedInput();
		input.AreEqual = (x, y) => x % 100 == y % 100;
		input.DisplayFilter = (item, _) => item.Id == 10;
		SetPrivateField(input, "InputValue", "Alpha");

		Assert.Equal("Beta", input.Format(120));
		Assert.Equal(string.Empty, input.Format(999));
	}

	/// <summary>
	/// Verifies dropdown filtering and limits operate on suggestion objects.
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
		Assert.True(input.Parse("Beta", out var value, out var error), error);
		Assert.Equal(20, value);
	}

	/// <summary>
	/// Verifies selecting a projected value notifies binding with a supplied edit context.
	/// </summary>
	[Fact]
	public void SuggestionSelectionNotifiesWithProjectedValue()
	{
		var input = CreateProjectedInput();
		var notifications = 0;
		var notifiedValue = 0;
		input.ValueChanged = EventCallback.Factory.Create<int>(this, value =>
		{
			notifiedValue = value;
			notifications++;
		});

		var projectedValue = Assert.IsType<int>(Invoke(input, "GetValue", new Suggestion(20, "Beta")));
		Invoke(input, "OnItemSelected", projectedValue, true, true);

		Assert.Equal(20, input.Value);
		Assert.Equal(20, notifiedValue);
		Assert.Equal(1, notifications);
		Assert.Equal("Beta", input.Format(input.Value));
	}

	/// <summary>
	/// Verifies navigation and Enter commit projected values and wrap at either end.
	/// </summary>
	[Fact]
	public void KeyboardSelectionProjectsAndWraps()
	{
		var input = CreateProjectedInput();

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowDown" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(10, input.Value);

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowUp" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(30, input.Value);

		Invoke(input, "OnKeyDown", new KeyboardEventArgs { Code = "ArrowDown" });
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(10, input.Value);
	}

	/// <summary>
	/// Verifies a projected zero remains a valid highlighted value during navigation.
	/// </summary>
	[Fact]
	public void KeyboardSelectionSupportsProjectedZero()
	{
		var input = CreateProjectedInput();
		input.Items = [new Suggestion(0, "Zero"), new Suggestion(20, "Beta")];

		Invoke(input, "HighlightNext");
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(0, input.Value);

		Invoke(input, "HighlightNext");
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(20, input.Value);
	}

	/// <summary>
	/// Verifies nullable value types are detected from the bound type, not the suggestion type.
	/// </summary>
	[Fact]
	public void NullableBoundValuesCanBeCleared()
	{
		var input = new TestInput<Suggestion, int?>
		{
			Items = [new Suggestion(20, "Beta")],
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id,
			Value = 20
		};
		input.Initialize();

		Assert.Equal("Beta", input.Format(20));
		Assert.True(input.Parse(string.Empty, out var value, out var error), error);
		Assert.Null(value);

		Invoke(input, "OnItemSelected", null!, true, true);
		Assert.Null(input.Value);
		Assert.Equal(string.Empty, input.Format(null));
	}

	/// <summary>
	/// Verifies same-type selection needs no selector and preserves labels for absent items.
	/// </summary>
	[Fact]
	public void SameTypesPreserveIdentityAndAbsentItemLabels()
	{
		var item = new Suggestion(10, "Alpha");
		var input = new TestInput<Suggestion, Suggestion>
		{
			Items = [item],
			DisplayValue = x => x.Name
		};
		input.Initialize();

		Assert.True(input.Parse("Alpha", out var value, out var error), error);
		Assert.Same(item, value);
		input.Items = Array.Empty<Suggestion>();
		Assert.Equal("Alpha", input.Format(item));
	}

	/// <summary>
	/// Verifies same-type selection uses the selector while formatting displays the bound value directly.
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

		Assert.True(input.Parse("2", out var value, out var error), error);
		Assert.Equal(20, value);
		Assert.Equal("20", input.Format(20));
	}

	/// <summary>
	/// Verifies missing text follows the default-item matching behavior for value-type suggestions.
	/// </summary>
	[Fact]
	public void ValueTypeItemsUseDefaultMatchForMissingText()
	{
		var input = new TestInput<int, int>
		{
			Items = [0, 1],
			DisplayValue = x => x.ToString()
		};
		input.Initialize();

		Assert.True(input.Parse("missing", out var value, out var error), error);
		Assert.Null(error);
		Assert.Equal(0, value);
		Assert.True(input.Parse("0", out value, out error), error);
		Assert.Equal(0, value);
	}

	/// <summary>
	/// Verifies closest matching projects a suggestion and rejects an empty collection.
	/// </summary>
	[Fact]
	public void ClosestMatchProjectsValuesAndRejectsEmptyItems()
	{
		var input = CreateProjectedInput();
		input.Options |= InputAutocompleteOptions.AutoSelectClosest;

		Assert.True(input.Parse("Alpha", out var value, out var error), error);
		Assert.Equal(10, value);

		input.Items = [];
		Assert.False(input.Parse("Alpha", out _, out error));
		Assert.NotNull(error);
	}

	/// <summary>
	/// Verifies current-highlight matching commits the stored highlight independently of available suggestions.
	/// </summary>
	[Fact]
	public void CurrentHighlightCanBeCommittedWithoutSuggestions()
	{
		var input = CreateProjectedInput();
		Invoke(input, "HighlightNext");
		input.Items = [];

		var match = Assert.IsType<(bool success, int match)>(Invoke(input, "GetMatch", "Alpha", InputAutocompleteOptions.AutoSelectCurrent));
		Assert.True(match.success);
		Assert.Equal(10, match.match);
		Invoke(input, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Assert.Equal(10, input.Value);
	}

	/// <summary>
	/// Verifies selected and highlighted styling compares projected values.
	/// </summary>
	[Fact]
	public void SuggestionStylingUsesProjectedValues()
	{
		var input = CreateProjectedInput();
		input.Value = 20;
		Invoke(input, "HighlightNext");

		var selectedCss = Assert.IsType<string>(Invoke(input, "GetDropDownItemCssClass", new Suggestion(20, "Beta")));
		var highlightedCss = Assert.IsType<string>(Invoke(input, "GetDropDownItemCssClass", new Suggestion(10, "Alpha")));
		Assert.Contains("has-text-success", selectedCss);
		Assert.Contains("has-background-default", highlightedCss);
	}

	/// <summary>
	/// Verifies requested suggestions are refreshed before automatic input selection projects a value.
	/// </summary>
	[Fact]
	public async Task RequestedItemsAreProjectedAfterRefresh()
	{
		var input = CreateProjectedInput();
		input.Options |= InputAutocompleteOptions.AutoSelectOnInput | InputAutocompleteOptions.AutoSelectExact;
		input.DisplayFilter = (item, text) => item.Name.StartsWith(text ?? string.Empty, StringComparison.OrdinalIgnoreCase);
		SetPrivateField(input, "InputValue", "Old");
		input.OnItemsRequested = async text =>
		{
			await Task.Yield();
			input.Items = [new Suggestion(40, text!)];
		};

		await Assert.IsAssignableFrom<Task>(Invoke(input, "OnInput", new ChangeEventArgs { Value = "Updated" }));

		Assert.Equal(40, input.Value);
		Assert.Equal("Updated", input.Format(input.Value));
	}

	/// <summary>
	/// Verifies distinct item and value types require a selector during parameter validation.
	/// </summary>
	[Fact]
	public void DifferentTypesRequireSelector()
	{
		var input = new TestInput<Suggestion, int>
		{
			Items = [],
			DisplayValue = x => x.Name
		};

		var error = Assert.Throws<InvalidOperationException>(input.Initialize);
		Assert.Contains("ValueSelector is required", error.Message);
	}

	/// <summary>
	/// Verifies component instances retain independent selectors and highlight state.
	/// </summary>
	[Fact]
	public void InstancesHaveIndependentSelectionState()
	{
		var first = CreateProjectedInput();
		var second = CreateProjectedInput();
		second.ValueSelector = x => x.Id + 100;

		Invoke(first, "HighlightNext");
		Invoke(first, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });
		Invoke(second, "HighlightPrevious");
		Invoke(second, "OnKeyUp", new KeyboardEventArgs { Code = "Enter" });

		Assert.Equal(10, first.Value);
		Assert.Equal(130, second.Value);
	}

	private static TestInput<Suggestion, int> CreateProjectedInput()
	{
		var input = new TestInput<Suggestion, int>
		{
			Items = [new Suggestion(10, "Alpha"), new Suggestion(20, "Beta"), new Suggestion(30, "Charlie")],
			DisplayValue = x => x.Name,
			ValueSelector = x => x.Id,
			Value = 99
		};
		input.Initialize();
		return input;
	}

	private static object? Invoke<TItem, TValue>(TestInput<TItem, TValue> input, string methodName, params object[] arguments)
	{
		var method = typeof(InputAutocomplete<TItem, TValue>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		return method.Invoke(input, arguments);
	}

	private static void SetPrivateField<TItem, TValue>(TestInput<TItem, TValue> input, string fieldName, object value)
	{
		var field = typeof(InputAutocomplete<TItem, TValue>).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
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

	private sealed class TestInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputAutocomplete<TItem, TValue>
	{
		private readonly FieldModel Model = new();

		/// <summary>
		/// Supplies an edit context and field before initializing the component without a renderer.
		/// </summary>
		public void Initialize()
		{
			Model.Value = Value;
			EditContext = new EditContext(Model);
			FieldIdentifier = new FieldIdentifier(Model, nameof(FieldModel.Value));
			ValueExpression = () => Model.Value;

			var property = typeof(InputAutocomplete<TItem, TValue>).GetProperty("ServiceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.NotNull(property);
			property.SetValue(this, new EmptyServiceProvider());
			OnInitialized();
			OnParametersSet();
		}

		/// <summary>
		/// Formats a bound value using the component's suggestion label lookup.
		/// </summary>
		/// <param name="value">The bound selection to display.</param>
		public string Format(TValue? value) => FormatValueAsString(value);

		/// <summary>
		/// Parses autocomplete text through the configured matching strategy.
		/// </summary>
		/// <param name="text">The user-entered text.</param>
		/// <param name="value">The projected result when a suggestion matches.</param>
		/// <param name="error">The validation error when no suggestion matches.</param>
		public bool Parse(string? text, [MaybeNullWhen(false)] out TValue value, [NotNullWhen(false)] out string? error) => TryParseValueFromString(text, out value, out error);

		private sealed class FieldModel
		{
			/// <summary>
			/// Represents the field used for standalone component validation and change notifications.
			/// </summary>
			public TValue Value { get; set; } = default!;
		}
	}
}

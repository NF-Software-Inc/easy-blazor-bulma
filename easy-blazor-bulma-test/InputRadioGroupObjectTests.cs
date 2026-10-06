using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies dictionary and list configurations preserve radio labels and bound values.
/// </summary>
public class InputRadioGroupObjectTests
{
	/// <summary>
	/// Verifies the existing dictionary configuration supports nullable selections.
	/// </summary>
	[Fact]
	public void DictionaryOptionsRemainSupported()
	{
		var input = new TestInput<string>
		{
			Options = new() { ["First"] = "one", ["None"] = null },
			Value = "one"
		};
		input.SetParameters();

		Assert.Equal("First", GetDisplay(input));
		Assert.Equal(input.Options.ToArray(), GetItems(input));
		Select(input, null);
		Assert.Null(input.Value);
		Assert.Equal("None", GetDisplay(input));
	}

	/// <summary>
	/// Verifies list labels use the display function and selection notifies the bound value.
	/// </summary>
	[Fact]
	public void ListOptionsProvideLabelsAndSelection()
	{
		var input = new TestInput<int>
		{
			RadioOptions = [10, 20],
			DisplayValue = x => $"Option {x}",
			Value = 10
		};
		var notifications = 0;
		input.ValueChanged = EventCallback.Factory.Create<int>(this, _ => notifications++);
		input.SetParameters();

		Assert.Equal("Option 10", GetDisplay(input));
		Assert.Equal(new[] { "Option 10", "Option 20" }, GetItems(input).Select(x => x.Key));
		Assert.Equal(new[] { 10, 20 }, GetItems(input).Select(x => x.Value));
		Select(input, 20);
		Assert.Equal(20, input.Value);
		Assert.Equal("Option 20", GetDisplay(input));
		Assert.Equal(1, notifications);
	}

	/// <summary>
	/// Verifies custom equality resolves the selected label in list mode.
	/// </summary>
	[Fact]
	public void ListOptionsRespectCustomEquality()
	{
		var input = new TestInput<string>
		{
			RadioOptions = ["one", "two"],
			DisplayValue = x => x.ToUpperInvariant(),
			AreEqual = (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase),
			Value = "ONE"
		};
		input.SetParameters();

		Assert.Equal("ONE", GetDisplay(input));
		input.Value = "missing";
		Assert.Equal(string.Empty, GetDisplay(input));
	}

	/// <summary>
	/// Verifies absent, incomplete, and conflicting configurations are rejected.
	/// </summary>
	/// <param name="dictionary">Whether to supply dictionary options.</param>
	/// <param name="list">Whether to supply list options.</param>
	/// <param name="display">Whether to supply the display function.</param>
	[Theory]
	[InlineData(false, false, false)]
	[InlineData(false, true, false)]
	[InlineData(false, false, true)]
	[InlineData(true, true, false)]
	[InlineData(true, false, true)]
	[InlineData(true, true, true)]
	public void InvalidConfigurationsThrow(bool dictionary, bool list, bool display)
	{
		var input = new TestInput<int>();
		if (dictionary)
			input.Options = new() { ["One"] = 1 };
		if (list)
			input.RadioOptions = [1];
		if (display)
			input.DisplayValue = x => x.ToString();

		Assert.Throws<InvalidOperationException>(() => input.SetParameters());
	}

	/// <summary>
	/// Verifies either configuration can intentionally contain no options.
	/// </summary>
	[Fact]
	public void EmptyCollectionsAreValid()
	{
		var dictionary = new TestInput<int> { Options = [] };
		dictionary.SetParameters();
		Assert.Empty(GetItems(dictionary));

		var list = new TestInput<int> { RadioOptions = [], DisplayValue = x => x.ToString() };
		list.SetParameters();
		Assert.Empty(GetItems(list));
	}

	/// <summary>
	/// Verifies list entries and labels refresh when parameters are reapplied.
	/// </summary>
	[Fact]
	public void ParameterUpdatesRefreshListOptions()
	{
		var input = new TestInput<int> { RadioOptions = [1], DisplayValue = x => $"Old {x}", Value = 1 };
		input.SetParameters();

		input.RadioOptions = [1, 2];
		input.DisplayValue = x => $"New {x}";
		input.SetParameters();
		Assert.Equal("New 1", GetDisplay(input));
		Assert.Equal(2, GetItems(input).Length);
	}

	/// <summary>
	/// Verifies disabled inputs do not update their bound value.
	/// </summary>
	[Fact]
	public void DisabledInputsIgnoreSelection()
	{
		var input = new TestInput<int>
		{
			RadioOptions = [1, 2],
			DisplayValue = x => x.ToString(),
			Value = 1,
			AdditionalAttributes = new Dictionary<string, object> { ["disabled"] = true }
		};
		input.SetParameters();
		Select(input, 2);
		Assert.Equal(1, input.Value);
	}

	private static string GetDisplay<TValue>(InputRadioGroupObject<TValue> input)
	{
		return (string)typeof(InputRadioGroupObject<TValue>).GetProperty("CurrentValueDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(input)!;
	}

	private static KeyValuePair<string, TValue?>[] GetItems<TValue>(InputRadioGroupObject<TValue> input)
	{
		return ((IEnumerable<KeyValuePair<string, TValue?>>)typeof(InputRadioGroupObject<TValue>).GetField("RadioItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(input)!).ToArray();
	}

	private static void Select<TValue>(InputRadioGroupObject<TValue> input, TValue? value)
	{
		typeof(InputRadioGroupObject<TValue>).GetMethod("OnCurrentChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(input, [value]);
	}

	private sealed class TestInput<TValue> : InputRadioGroupObject<TValue>
	{
		public void SetParameters() => OnParametersSet();
	}
}

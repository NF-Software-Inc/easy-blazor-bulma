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

		Assert.Equal("0", GetIdentifier(input));
		Assert.Equal(input.Options.ToArray(), GetItems(input).Select(x => new KeyValuePair<string, string?>(x.Display, x.Value)).ToArray());
		Select(input, null);
		Assert.Null(input.Value);
		Assert.Equal("1", GetIdentifier(input));
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

		Assert.Equal("0", GetIdentifier(input));
		Assert.Equal(new[] { "Option 10", "Option 20" }, GetItems(input).Select(x => x.Display));
		Assert.Equal(new[] { 10, 20 }, GetItems(input).Select(x => x.Value));
		Select(input, 20);
		Assert.Equal(20, input.Value);
		Assert.Equal("1", GetIdentifier(input));
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

		Assert.Equal("0", GetIdentifier(input));
		Assert.Equal("ONE", GetItems(input)[0].Display);
		input.Value = "missing";
		Assert.Equal(string.Empty, GetIdentifier(input));
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
		Assert.Equal("0", GetIdentifier(input));
		Assert.Equal("New 1", GetItems(input)[0].Display);
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

	/// <summary>
	/// Verifies duplicate labels retain separate radio identities and HTML IDs.
	/// </summary>
	[Fact]
	public void DuplicateLabelsHaveDistinctIdentities()
	{
		var input = new TestInput<int> { RadioOptions = [10, 20], DisplayValue = _ => "Same label", Value = 10 };
		input.SetParameters();
		var items = GetItems(input);

		Assert.Equal(new[] { "Same label", "Same label" }, items.Select(x => x.Display));
		Assert.Equal(new[] { "0", "1" }, items.Select(x => x.Identifier));
		var method = typeof(InputRadioGroupObject<int>).GetMethod("GetRadioOptionId", BindingFlags.Instance | BindingFlags.NonPublic)!;
		Assert.NotEqual(method.Invoke(input, [items[0].Identifier]), method.Invoke(input, [items[1].Identifier]));
		Assert.Equal("0", GetIdentifier(input));
		Select(input, 20);
		Assert.Equal(20, input.Value);
		Assert.Equal("1", GetIdentifier(input));
	}

	private static string GetIdentifier<TValue>(InputRadioGroupObject<TValue> input)
	{
		return (string)typeof(InputRadioGroupObject<TValue>).GetProperty("CurrentValueIdentifier", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(input)!;
	}

	private static (string Identifier, string Display, TValue? Value)[] GetItems<TValue>(InputRadioGroupObject<TValue> input)
	{
		var items = (System.Collections.IEnumerable)typeof(InputRadioGroupObject<TValue>).GetField("RadioItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(input)!;
		return items.Cast<object>().Select(item =>
		{
			var type = item.GetType();
			return ((string)type.GetProperty("Identifier")!.GetValue(item)!,
				(string)type.GetProperty("Display")!.GetValue(item)!,
				(TValue?)type.GetProperty("Value")!.GetValue(item));
		}).ToArray();
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

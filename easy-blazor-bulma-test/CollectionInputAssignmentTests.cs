using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies collection inputs accept directly assignable values without a selector.
/// </summary>
public class CollectionInputAssignmentTests
{
	/// <summary>
	/// Verifies implementing records can bind to an interface without losing item identity.
	/// </summary>
	/// <param name="component">The collection input to exercise.</param>
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void InterfaceValuesDoNotRequireSelector(int component)
	{
		var item = new GenericAutocomplete(10, "Alpha");
		var input = Create<GenericAutocomplete, IAutocomplete>(component, [item], x => x.Name);

		Invoke(input, "OnParametersSet");

		Assert.Same(item, Invoke(input, "GetValue", item));
		Assert.Equal(component >= 2 ? "0" : "Alpha", Format(input, item));
	}

	/// <summary>
	/// Verifies a derived item can bind directly to its base class.
	/// </summary>
	/// <param name="component">The collection input to exercise.</param>
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void BaseClassValuesDoNotRequireSelector(int component)
	{
		var item = new DerivedSuggestion(10, "Alpha");
		var input = Create<DerivedSuggestion, BaseSuggestion>(component, [item], x => x.Name);

		Invoke(input, "OnParametersSet");

		Assert.Same(item, Invoke(input, "GetValue", item));
		Assert.Equal(component >= 2 ? "0" : "Alpha", Format(input, item));
	}

	/// <summary>
	/// Verifies every nullability permutation supports conversion and selected-value formatting.
	/// </summary>
	/// <param name="component">The collection input to exercise.</param>
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void MatchingUnderlyingTypesDoNotRequireSelector(int component)
	{
		VerifyDayOfWeek<DayOfWeek, DayOfWeek>(component, DayOfWeek.Monday);
		VerifyDayOfWeek<DayOfWeek, DayOfWeek?>(component, DayOfWeek.Monday);
		VerifyDayOfWeek<DayOfWeek?, DayOfWeek?>(component, DayOfWeek.Monday);
		VerifyDayOfWeek<DayOfWeek?, DayOfWeek>(component, DayOfWeek.Monday);
	}

	/// <summary>
	/// Verifies other interface implementations use equality lookup instead of a blind downcast.
	/// </summary>
	/// <param name="component">The collection input to exercise.</param>
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void OtherImplementationsUseSelectedValueLookup(int component)
	{
		var input = Create<GenericAutocomplete, IAutocomplete>(
			component, [new GenericAutocomplete(10, "Alpha")], x => x.Name,
			(x, y) => x.Id == y?.Id);
		Invoke(input, "OnParametersSet");

		Assert.Equal(component >= 2 ? "0" : "Alpha", Format(input, new OtherAutocomplete(10, "Other")));
		Assert.Equal(string.Empty, Format(input, new OtherAutocomplete(20, "Missing")));
	}

	/// <summary>
	/// Verifies both validation and conversion reject unrelated types with complete type diagnostics.
	/// </summary>
	/// <param name="component">The collection input to exercise.</param>
	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	[InlineData(3)]
	public void UnrelatedTypesRequireSelectorWithAssemblyQualifiedNames(int component)
	{
		var item = new GenericAutocomplete(10, "Alpha");
		var input = Create<GenericAutocomplete, int>(component, [item], x => x.Name);

		var validationError = Assert.Throws<InvalidOperationException>(() => Invoke(input, "OnParametersSet"));
		var conversionError = Assert.Throws<InvalidOperationException>(() => Invoke(input, "GetValue", item));

		Assert.Equal(validationError.Message, conversionError.Message);
		Assert.Contains("ValueSelector is required when TItem is not assignable to TValue.", validationError.Message);
		Assert.Contains(typeof(GenericAutocomplete).AssemblyQualifiedName!, validationError.Message);
		Assert.Contains(typeof(int).AssemblyQualifiedName!, validationError.Message);
	}

	private static void VerifyDayOfWeek<TItem, TValue>(int component, TItem item)
	{
		var input = Create<TItem, TValue>(component, [item], x => x!.ToString()!);
		Invoke(input, "OnParametersSet");

		var value = Invoke(input, "GetValue", item!);
		Assert.Equal(DayOfWeek.Monday, value);
		Assert.Equal(component >= 2 ? "0" : "Monday", Format(input, value!));
	}

	private static ComponentBase Create<
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem,
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(
		int component, IEnumerable<TItem> items, Func<TItem, string> display,
		Func<TValue, TValue?, bool>? areEqual = null) => component switch
		{
			0 => new TestAutocomplete<TItem, TValue>(items, display, areEqual),
			1 => new TestAutocompleteMultiple<TItem, TValue>(items, display, areEqual),
			2 => new TestSelect<TItem, TValue>(items, display, areEqual),
			3 => new TestSelectMultiple<TItem, TValue>(items, display, areEqual),
			_ => throw new ArgumentOutOfRangeException(nameof(component))
		};

	private static string Format(ComponentBase input, object value)
	{
		var componentType = input.GetType().BaseType!;
		if (componentType.GetGenericTypeDefinition() == typeof(InputSelectMultipleObject<,>))
		{
			componentType.GetProperty("Value")!.SetValue(input,
				Activator.CreateInstance(typeof(List<>).MakeGenericType(componentType.GenericTypeArguments[1])));
			var values = (System.Collections.IList)componentType.GetProperty("Value")!.GetValue(input)!;
			values.Add(value);
			var indices = (string[])componentType.GetProperty("SelectedIndices", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(input)!;
			return string.Join(',', indices);
		}

		var method = componentType.GetGenericTypeDefinition() == typeof(InputAutocompleteMultiple<,>)
			? "GetItemDisplayValue" : "FormatValueAsString";
		return Assert.IsType<string>(Invoke(input, method, value));
	}

	private static object? Invoke(ComponentBase input, string methodName, params object[] arguments)
	{
		var method = input.GetType().BaseType!.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		try
		{
			return method.Invoke(input, arguments);
		}
		catch (TargetInvocationException exception) when (exception.InnerException != null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}

	private interface IAutocomplete
	{
		long Id { get; }
	}

	private sealed record GenericAutocomplete(long Id, string Name) : IAutocomplete;
	private sealed record OtherAutocomplete(long Id, string Name) : IAutocomplete;
	private record BaseSuggestion(long Id, string Name);
	private sealed record DerivedSuggestion(long Id, string Name) : BaseSuggestion(Id, Name);

	private sealed class TestAutocomplete<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>
		: InputAutocomplete<TItem, TValue>
	{
		[SetsRequiredMembers]
		public TestAutocomplete(IEnumerable<TItem> items, Func<TItem, string> display, Func<TValue, TValue?, bool>? areEqual)
		{
			Items = items;
			DisplayValue = display;
			if (areEqual != null)
				AreEqual = areEqual;
		}
	}

	private sealed class TestAutocompleteMultiple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>
		: InputAutocompleteMultiple<TItem, TValue>
	{
		[SetsRequiredMembers]
		public TestAutocompleteMultiple(IEnumerable<TItem> items, Func<TItem, string> display, Func<TValue, TValue?, bool>? areEqual)
		{
			Items = items;
			DisplayValue = display;
			if (areEqual != null)
				AreEqual = areEqual;
		}
	}

	private sealed class TestSelect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>
		: InputSelectObject<TItem, TValue>
	{
		[SetsRequiredMembers]
		public TestSelect(IEnumerable<TItem> items, Func<TItem, string> display, Func<TValue, TValue?, bool>? areEqual)
		{
			Items = items;
			DisplayValue = display;
			if (areEqual != null)
				AreEqual = areEqual;
		}
	}

	private sealed class TestSelectMultiple<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TItem, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>
		: InputSelectMultipleObject<TItem, TValue>
	{
		[SetsRequiredMembers]
		public TestSelectMultiple(IEnumerable<TItem> items, Func<TItem, string> display, Func<TValue, TValue?, bool>? areEqual)
		{
			Items = items;
			DisplayValue = display;
			if (areEqual != null)
				Contains = (values, value) => values != null && values.Any(x => areEqual(x, value));
		}
	}
}

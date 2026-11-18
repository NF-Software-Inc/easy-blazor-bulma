using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace easy_blazor_bulma_test;

/// <summary>
/// Verifies keyboard navigation and key registration behavior for <see cref="InputCharacter{TValue}"/>.
/// </summary>
public class InputCharacterTests
{
	/// <summary>
	/// Verifies arrow keys move the selected character across rows and columns with wraparound when enabled.
	/// </summary>
	[Fact]
	public void ArrowKeysNavigateAndWrapAcrossRowsAndColumns()
	{
		var (input, _) = CreateInput('A', ['A', 'B', 'C', 'D', 'E', 'F'], columns: 3);

		InvokeKeyDown(input, "ArrowDown");
		Assert.Equal('D', input.Value);

		InvokeKeyDown(input, "ArrowDown");
		Assert.Equal('A', input.Value);

		InvokeKeyDown(input, "ArrowRight");
		Assert.Equal('B', input.Value);

		InvokeKeyDown(input, "ArrowLeft");
		Assert.Equal('A', input.Value);

		InvokeKeyDown(input, "ArrowLeft");
		Assert.Equal('C', input.Value);
	}

	/// <summary>
	/// Verifies disabling arrow-key navigation prevents selection changes and allows arrow keys through preventDefault registration.
	/// </summary>
	[Fact]
	public async Task DisablingArrowKeyNavigationSuppressesSelectionChangesAndUpdatesAllowedKeys()
	{
		var (input, jsRuntime) = CreateInput('B', ['A', 'B', 'C', 'D', 'E', 'F'], columns: 3);
		input.UseArrowKeyNavigation = false;
		SetButtonsElement(input, "character-buttons");

		await InvokeOnAfterRenderAsync(input, firstRender: true);
		Assert.Equal(["Escape", "Tab", "Enter", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"], jsRuntime.LastRegisteredKeys);

		InvokeKeyDown(input, "ArrowUp");
		InvokeKeyDown(input, "ArrowDown");
		InvokeKeyDown(input, "ArrowLeft");
		InvokeKeyDown(input, "ArrowRight");
		Assert.Equal('B', input.Value);

		input.UseArrowKeyNavigation = true;
		await InvokeOnAfterRenderAsync(input, firstRender: false);
		Assert.Equal(["Escape", "Tab", "Enter"], jsRuntime.LastRegisteredKeys);
	}

	private static (TestCharacterInput<TValue> Input, RecordingJsRuntime JsRuntime) CreateInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(
		TValue initialValue,
		char[] characters,
		int columns)
	{
		var jsRuntime = new RecordingJsRuntime();
		var input = new TestCharacterInput<TValue>
		{
			Value = initialValue,
			Characters = characters,
			Columns = columns
		};

		input.ValueChanged = EventCallback.Factory.Create<TValue>(new object(), value => input.Value = value);
		input.Initialize(jsRuntime);

		return (input, jsRuntime);
	}

	private static void InvokeKeyDown<TValue>(TestCharacterInput<TValue> input, string key)
	{
		var method = typeof(InputCharacter<TValue>).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		method.Invoke(input, [new KeyboardEventArgs { Key = key }]);
	}

	private static async Task InvokeOnAfterRenderAsync<TValue>(TestCharacterInput<TValue> input, bool firstRender)
	{
		var method = typeof(InputCharacter<TValue>).GetMethod("OnAfterRenderAsync", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		var task = Assert.IsAssignableFrom<Task>(method.Invoke(input, [firstRender]));
		await task;
	}

	private static void SetButtonsElement<TValue>(TestCharacterInput<TValue> input, string id)
	{
		var field = typeof(InputCharacter<TValue>).GetField("ButtonsElement", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		field.SetValue(input, (ElementReference?)new ElementReference(id));
	}

	private sealed class RecordingJsRuntime : IJSRuntime
	{
		/// <summary>
		/// Stores the keys passed to the most recent <c>RegisterKeyDownPreventDefault</c> invocation.
		/// </summary>
		public string[]? LastRegisteredKeys { get; private set; }

		/// <inheritdoc />
		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
		{
			return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
		}

		/// <inheritdoc />
		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
		{
			if (identifier == "easyBlazorBulma.Input.RegisterKeyDownPreventDefault" && args is { Length: > 1 } && args[1] is string[] keys)
				LastRegisteredKeys = keys;

			object? result = typeof(TValue) == typeof(bool) ? true : default(TValue);
			return new ValueTask<TValue>((TValue)result!);
		}
	}

	private sealed class TestCharacterInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputCharacter<TValue>
	{
		private readonly FieldModel Model = new();

		/// <summary>
		/// Configures edit context and injected services before running component initialization hooks.
		/// </summary>
		/// <param name="jsRuntime">The JS runtime double used to record key registration calls.</param>
		public void Initialize(IJSRuntime jsRuntime)
		{
			Model.Value = Value;
			EditContext = new EditContext(Model);
			FieldIdentifier = new FieldIdentifier(Model, nameof(FieldModel.Value));
			ValueExpression = () => Model.Value;

			var jsProperty = typeof(InputCharacter<TValue>).GetProperty("JsRuntime", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.NotNull(jsProperty);
			jsProperty.SetValue(this, jsRuntime);

			OnInitialized();
			OnParametersSet();
		}

		private sealed class FieldModel
		{
			/// <summary>
			/// Holds the bound value for standalone edit-context notifications.
			/// </summary>
			public TValue Value { get; set; } = default!;
		}
	}
}

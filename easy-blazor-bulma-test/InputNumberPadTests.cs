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
/// Verifies keyboard navigation and key entry behavior for <see cref="InputNumberPad{TValue}"/>.
/// </summary>
public class InputNumberPadTests
{
	/// <summary>
	/// Verifies horizontal arrow keys move focus across columns on the same row and wrap at both edges.
	/// </summary>
	[Fact]
	public async Task HorizontalArrowsNavigateAndWrapAcrossColumns()
	{
		var (input, jsRuntime, buttonId) = CreateInput<int>();

		await InvokeButtonKeyDownAsync(input, "ArrowRight", row: 1, column: 0);
		Assert.Equal($"{buttonId}-1-1", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowRight", row: 1, column: 2);
		Assert.Equal($"{buttonId}-1-0", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowLeft", row: 2, column: 0);
		Assert.Equal($"{buttonId}-2-2", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowLeft", row: 2, column: 2);
		Assert.Equal($"{buttonId}-2-1", jsRuntime.LastFocusedId);
	}

	/// <summary>
	/// Verifies vertical arrow keys wrap within the active column when optional top and bottom controls are hidden.
	/// </summary>
	[Fact]
	public async Task VerticalArrowsWrapWithinColumnWhenOptionalControlsAreHidden()
	{
		var (input, jsRuntime, buttonId) = CreateInput<int>();

		await InvokeButtonKeyDownAsync(input, "ArrowUp", row: 0, column: 1);
		Assert.Equal($"{buttonId}-3-1", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowDown", row: 3, column: 1);
		Assert.Equal($"{buttonId}-0-1", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowDown", row: 1, column: 2);
		Assert.Equal($"{buttonId}-2-2", jsRuntime.LastFocusedId);
	}

	/// <summary>
	/// Verifies vertical arrow keys step through DisplayInput and the custom button while remembering the active keypad column.
	/// </summary>
	[Fact]
	public async Task VerticalArrowsStepThroughDisplayInputAndCustomButtonPreservingColumn()
	{
		var (input, jsRuntime, buttonId) = CreateInput<decimal?>(
			displayInput: true,
			customButtonCallback: () => Task.CompletedTask,
			additionalAttributes: new Dictionary<string, object> { ["id"] = "custom-pad-input" });

		await InvokeButtonKeyDownAsync(input, "ArrowUp", row: 0, column: 2);
		Assert.Equal("custom-pad-input", jsRuntime.LastFocusedId);

		await InvokeInputKeyDownAsync(input, "ArrowDown");
		Assert.Equal($"{buttonId}-0-2", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowDown", row: 3, column: 2);
		Assert.Equal($"{buttonId}-4-0", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowUp", row: 4, column: 0);
		Assert.Equal($"{buttonId}-3-2", jsRuntime.LastFocusedId);

		await InvokeButtonKeyDownAsync(input, "ArrowDown", row: 4, column: 0);
		Assert.Equal("custom-pad-input", jsRuntime.LastFocusedId);

		await InvokeInputKeyDownAsync(input, "ArrowUp");
		Assert.Equal($"{buttonId}-4-0", jsRuntime.LastFocusedId);
	}

	/// <summary>
	/// Verifies digit, decimal, and Backspace keys update the bound value from both keypad buttons and DisplayInput.
	/// </summary>
	[Fact]
	public async Task DigitDecimalAndBackspaceKeysUpdateBoundValue()
	{
		var (input, _, _) = CreateInput<decimal?>(displayInput: true);

		await InvokeInputKeyDownAsync(input, "4");
		await InvokeButtonKeyDownAsync(input, ".", row: 0, column: 0);
		await InvokeButtonKeyDownAsync(input, "5", row: 0, column: 0);
		Assert.Equal(4.5m, input.Value);

		await InvokeInputKeyDownAsync(input, "Backspace");
		await InvokeInputKeyDownAsync(input, "Backspace");
		Assert.Equal(4m, input.Value);

		await InvokeButtonKeyDownAsync(input, "Backspace", row: 3, column: 2);
		Assert.Null(input.Value);
	}

	/// <summary>
	/// Verifies disabled state suppresses both arrow focus changes and value edits.
	/// </summary>
	[Fact]
	public async Task DisabledStateSuppressesNavigationAndEdits()
	{
		var (input, jsRuntime, _) = CreateInput<int>(
			displayInput: true,
			additionalAttributes: new Dictionary<string, object> { ["disabled"] = true });

		await InvokeButtonKeyDownAsync(input, "ArrowRight", row: 0, column: 0);
		await InvokeInputKeyDownAsync(input, "ArrowDown");
		await InvokeInputKeyDownAsync(input, "7");

		Assert.Null(jsRuntime.LastFocusedId);
		Assert.Equal(0, input.Value);
	}

	private static (TestNumberPad<TValue> Input, RecordingJsRuntime JsRuntime, string ButtonId) CreateInput<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue>(
		bool displayInput = false,
		Func<Task>? customButtonCallback = null,
		Dictionary<string, object>? additionalAttributes = null)
	{
		var jsRuntime = new RecordingJsRuntime();
		var input = new TestNumberPad<TValue>
		{
			DisplayInput = displayInput,
			OnCustomButtonClicked = customButtonCallback,
			AdditionalAttributes = additionalAttributes
		};

		input.ValueChanged = EventCallback.Factory.Create<TValue>(new object(), value => input.Value = value);
		input.Initialize(jsRuntime);

		var buttonIdField = typeof(InputNumberPad<TValue>).GetField("ButtonId", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(buttonIdField);

		var buttonId = Assert.IsType<string>(buttonIdField.GetValue(input));
		return (input, jsRuntime, buttonId);
	}

	private static async Task InvokeButtonKeyDownAsync<TValue>(TestNumberPad<TValue> input, string key, int row, int column)
	{
		var method = typeof(InputNumberPad<TValue>).GetMethod("OnButtonKeyDown", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		var task = Assert.IsAssignableFrom<Task>(method.Invoke(input, [new KeyboardEventArgs { Key = key }, row, column]));
		await task;
	}

	private static async Task InvokeInputKeyDownAsync<TValue>(TestNumberPad<TValue> input, string key)
	{
		var method = typeof(InputNumberPad<TValue>).GetMethod("OnInputKeyDown", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);

		var task = Assert.IsAssignableFrom<Task>(method.Invoke(input, [new KeyboardEventArgs { Key = key }]));
		await task;
	}

	private sealed class RecordingJsRuntime : IJSRuntime
	{
		/// <summary>
		/// Stores the element ID from the most recent <c>easyBlazorBulma.FocusElement</c> invocation.
		/// </summary>
		public string? LastFocusedId { get; private set; }

		/// <inheritdoc />
		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
		{
			return InvokeAsync<TValue>(identifier, CancellationToken.None, args);
		}

		/// <inheritdoc />
		public ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
		{
			if (identifier == "easyBlazorBulma.FocusElement" && args is { Length: > 0 } && args[0] is string focusedId)
				LastFocusedId = focusedId;

			object? result = typeof(TValue) == typeof(bool) ? true : default(TValue);
			return new ValueTask<TValue>((TValue)result!);
		}
	}

	private sealed class TestNumberPad<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TValue> : InputNumberPad<TValue>
	{
		private readonly FieldModel Model = new();

		/// <summary>
		/// Configures edit context and injected services before running component initialization hooks.
		/// </summary>
		/// <param name="jsRuntime">The JS runtime double used to record focus calls.</param>
		public void Initialize(IJSRuntime jsRuntime)
		{
			Model.Value = Value;
			EditContext = new EditContext(Model);
			FieldIdentifier = new FieldIdentifier(Model, nameof(FieldModel.Value));
			ValueExpression = () => Model.Value;

			var jsProperty = typeof(InputNumberPad<TValue>).GetProperty("JsRuntime", BindingFlags.Instance | BindingFlags.NonPublic);
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

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace easy_blazor_bulma;

/// <summary>
/// Class to provide base functionality to Blazor input components.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public abstract class EasyInputBase<TValue> : InputBase<TValue>
{
	/// <summary>
	/// Use to indicate when a render of the component is required.
	/// </summary>
	protected virtual bool AwaitingRender { get; set; }

	/// <summary>
	/// The duration to wait before checking <see cref="AwaitingRender"/> again.
	/// </summary>
	protected TimeSpan AwaitingRenderDelay { get; set; } = TimeSpan.FromMilliseconds(1);

	/// <summary>
	/// The maximum duration to wait for a render to complete before continuing execution.
	/// </summary>
	/// <remarks>
	/// Used when a cancellation token is not provided to <see cref="AwaitRender(CancellationToken?)"/> or <see cref="StateHasChangedAsync(CancellationToken?)"/>.
	/// </remarks>
	protected TimeSpan? MaxRenderDelay { get; set; }

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		AwaitingRender = false;
		await Task.CompletedTask;
	}

	/// <summary>
	/// Delays execution until <see cref="AwaitingRender"/> is equal to false.
	/// </summary>
	/// <param name="token">A token to cancel waiting.</param>
	/// <remarks>
	/// If you have overridden <see cref="ComponentBase.OnAfterRenderAsync(bool)"/> you must call <c>base.OnAfterRenderAsync(bool)</c> for this to work.
	/// </remarks>
	protected async Task AwaitRender(CancellationToken? token = null)
	{
		await Task.Yield();

		if (token == null && MaxRenderDelay != null)
			token = new CancellationTokenSource(MaxRenderDelay.Value).Token;
		else if (token == null)
			token = CancellationToken.None;

		while (AwaitingRender && token.Value.IsCancellationRequested == false)
		{
			try
			{
				await Task.Delay(AwaitingRenderDelay, token.Value);
			}
			catch (TaskCanceledException)
			{
				break;
			}
		}
	}

	/// <summary>
	/// Notifies the component its state has changed and waits for rendering to complete.
	/// </summary>
	/// <param name="token">A token to cancel waiting.</param>
	protected async Task StateHasChangedAsync(CancellationToken? token = null)
	{
		AwaitingRender = true;
		StateHasChanged();

		await AwaitRender(token);
	}
}

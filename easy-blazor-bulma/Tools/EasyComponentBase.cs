using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma;

/// <summary>
/// Class to provide base functionality to Blazor views.
/// </summary>
public abstract class EasyComponentBase : ComponentBase
{
	/// <summary>
	/// Use to indicate when a render of the component is required.
	/// </summary>
	protected virtual bool AwaitingRender { get; set; }

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		AwaitingRender = false;
		await Task.CompletedTask;
	}

	/// <summary>
	/// Delays execution until <see cref="AwaitingRender"/> is equal to false.
	/// </summary>
	/// <param name="interval">The duration in milliseconds to wait before checking <see cref="AwaitingRender"/> again.</param>
	/// <param name="token">A token to cancel waiting.</param>
	/// <remarks>
	/// If you have overridden <see cref="ComponentBase.OnAfterRenderAsync(bool)"/> you must call <c>base.OnAfterRenderAsync(bool)</c> for this to work.
	/// </remarks>
	protected async Task AwaitRender(int interval = 1, CancellationToken? token = null)
	{
		await Task.Yield();
		token ??= CancellationToken.None;

		while (AwaitingRender && token.Value.IsCancellationRequested == false)
		{
			try
			{
				await Task.Delay(interval, token.Value);
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

		await AwaitRender(token: token);
	}
}

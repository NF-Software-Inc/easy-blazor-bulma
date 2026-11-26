using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net;

namespace easy_blazor_bulma;

/// <summary>
/// Extends the default error boundary to provide access to the current error details.
/// </summary>
public class ExtendedErrorBoundary : ErrorBoundary
{
	/// <summary>
	/// Notifies subscribers that an error has occurred.
	/// </summary>
	[Parameter]
	public EventCallback<Exception> OnError { get; set; }

	private Exception? ExtendedException;

	/// <inheritdoc />
	protected async override Task OnErrorAsync(Exception exception)
	{
		ExtendedException = exception;

		await base.OnErrorAsync(exception);

		if (OnError.HasDelegate)
			await OnError.InvokeAsync(exception);
	}

	/// <inheritdoc cref="ErrorBoundaryBase.Recover" />
	public new void Recover()
	{
		ExtendedException = null;
		base.Recover();
	}

	/// <summary>
	/// Returns the current exception if present.
	/// </summary>
	public Exception? GetCurrentException()
	{
		return ExtendedException;
	}

	/// <summary>
	/// Returns the current error message.
	/// </summary>
	public string? GetErrorMessage()
	{
		return ExtendedException?.Message;
	}

	/// <summary>
	/// Returns the type of the current error.
	/// </summary>
	public Type? GetErrorType()
	{
		return ExtendedException?.GetType();
	}

	/// <summary>
	/// Returns the HTTP status code as an integer when present.
	/// </summary>
	public HttpStatusCode? GetHttpStatusCode()
	{
		if (ExtendedException != null && ExtendedException is HttpRequestException exception)
			return exception.StatusCode;

		return null;
	}
}

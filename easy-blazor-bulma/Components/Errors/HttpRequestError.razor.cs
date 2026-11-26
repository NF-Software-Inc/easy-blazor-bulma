using Microsoft.AspNetCore.Components;
using System.Net;

namespace easy_blazor_bulma;

/// <summary>
/// Displays an error page for the provided HTTP status code.
/// </summary>
public partial class HttpRequestError : ComponentBase
{
	/// <summary>
	/// The HTTP status code to display an error page for.
	/// </summary>
	[Parameter]
	public HttpStatusCode StatusCode { get; set; }
}

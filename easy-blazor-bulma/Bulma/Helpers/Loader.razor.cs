using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace easy_blazor_bulma;

/// <summary>
/// Displays a loading screen with a progress meter and message. This component will not cause child components to re-render when switching between loading and loaded modes.
/// </summary>
/// <remarks>
/// There are 5 additional attributes that can be used: container-class, progress-class, icon-class, loading-class, and content-class. Each of which apply CSS classes to the resulting elements as per their names.
/// </remarks>
public partial class Loader : ComponentBase
{
    /// <summary>
	/// Text to display while the loading function is active.
	/// </summary>
	[Parameter]
    public string Message { get; set; } = "Loading...";

    /// <summary>
    /// The percentage towards finishing that the loading function is at.
    /// </summary>
    [Parameter]
    [Range(0, 100)]
    public int? Completion { get; set; }

    /// <summary>
    /// The current status of the loading function.
    /// </summary>
    [Parameter]
    public LoadingStatus? Status { get; set; }

    /// <summary>
    /// Expression for manual binding to <see cref="Status"/>.
    /// </summary>
    [Parameter]
    public Expression<Func<LoadingStatus?>>? StatusExpression { get; set; }

    /// <summary>
    /// Event that occurs when the display status of the modal changes.
    /// </summary>
    [Parameter]
    public EventCallback<LoadingStatus?> StatusChanged { get; set; }

    /// <summary>
    /// Specifies whether the loading component should use the full screen height.
    /// </summary>
    [Parameter]
    public bool IsFullHeight { get; set; }

    /// <summary>
    /// Specifies whether to display the close button when loading is complete.
    /// </summary>
    [Parameter]
    public bool ShowClose { get; set; } = true;

    /// <summary>
    /// The text to display on the close button when loading is complete.
    /// </summary>
    [Parameter]
    public string CloseButtonText { get; set; } = "Close";

    /// <summary>
    /// A custom function to run when the close button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnCloseClicked { get; set; }

    /// <summary>
    /// The content to display when loading is occurring.
    /// </summary>
    [Parameter]
    public RenderFragment? Loading { get; set; }

    /// <summary>
    /// The content to display when loading is not occurring.
    /// </summary>
    [Parameter]
    public RenderFragment? Loaded { get; set; }

    /// <summary>
    /// Fallback for content to display when loading is not occurring.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Any additional attributes applied directly to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class", "container-class", "progress-class", "icon-class", "loading-class", "content-class" };

    private string MainCssClass
    {
        get
        {
            var css = "hero";

            if (IsFullHeight)
                css += " is-fullheight-with-navbar";

            if (Status == null || Status.Value == LoadingStatus.NotStarted)
                css += " is-hidden";

            return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
        }
    }

    private string ContainerCssClass
    {
        get
        {
            var css = "has-text-centered";

            if (IsFullHeight)
                css += " container";

            return string.Join(' ', css, AdditionalAttributes.GetValue("container-class"));
        }
    }

    private string ProgressCssClass => string.Join(' ', "progress", AdditionalAttributes.GetValue("progress-class"));

    private string IconCssClass
    {
        get
        {
            var css = "material-icons is-size-1";

            if (Status != null && Status.Value.HasFlag(LoadingStatus.Failed))
                css += " has-text-danger";
            else
                css += " has-text-success";

            return string.Join(' ', css, AdditionalAttributes.GetValue("icon-class"));
        }
    }

    private string LoadingCssClass
    {
        get
        {
            var css = "";

            if (Status == null || Status.Value == LoadingStatus.NotStarted)
                css += " is-hidden";

			return string.Join(' ', css, AdditionalAttributes.GetValue("loading-class"));
		}
    }

    private string ContentCssClass
    {
        get
        {
            var css = "";

            if (Status != null && Status.Value != LoadingStatus.NotStarted)
                css += " is-hidden";

			return string.Join(' ', css, AdditionalAttributes.GetValue("content-class"));
		}
    }

    private string Icon => Status != null && Status.Value.HasFlag(LoadingStatus.Failed) ? "error_outline" : "check_circle";

    private async Task OnClose()
    {
        Status = null;

        if (StatusChanged.HasDelegate)
            await StatusChanged.InvokeAsync(Status);

        if (OnCloseClicked.HasDelegate)
            await OnCloseClicked.InvokeAsync();
    }
}

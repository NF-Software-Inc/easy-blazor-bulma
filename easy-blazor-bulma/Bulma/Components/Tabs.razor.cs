using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace easy_blazor_bulma;

/// <summary>
/// Simple responsive horizontal navigation tabs, with different styles.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/components/tabs/">Bulma Documentation</see>
/// </remarks>
public partial class Tabs : ComponentBase
{
	/// <summary>
	/// The name of the tab that is currently displayed.
	/// </summary>
	[Parameter]
	public string? Active { get; set; }

	/// <summary>
	/// Expression for manual binding to <see cref="Active"/>.
	/// </summary>
	[Parameter]
	public Expression<Func<string?>>? ActiveExpression { get; set; }

	/// <summary>
	/// Event that occurs when <see cref="Active"/> is modified.
	/// </summary>
	[Parameter]
	public EventCallback<string?> ActiveChanged { get; set; }

    /// <summary>
    /// Positions the tab bar in the center of the area when true.
    /// </summary>
    [Parameter]
	public bool IsCentered { get; set; } = true;

    /// <summary>
    /// Uses the more classic style with borders for elements in the tab bar.
    /// </summary>
    [Parameter]
    public bool IsBoxed { get; set; }

    /// <summary>
    /// Uses mutually exclusive tabs (like radio buttons) for elements in the tab bar.
    /// </summary>
    [Parameter]
    public bool IsToggle { get; set; } = true;

    /// <summary>
    /// Rounds the first and last elements in the tab bar. Requires <see cref="IsToggle"/> to be true.
    /// </summary>
    [Parameter]
	public bool IsRounded { get; set; } = true;

    /// <summary>
    /// Event that occurs when an item in the tab bar is clicked.
    /// </summary>
    [Parameter]
	[Obsolete("Use OnTabClicked instead.")]
    public Func<string?, Task>? OnItemClicked { get; set; }

	/// <summary>
	/// Event that occurs when an item in the tab bar is clicked.
	/// </summary>
	[Parameter]
	public EventCallback<Tab> OnTabClicked { get; set; }

    /// <summary>
    /// The content to display within the tab bar. Can contain <see cref="Tab"/> elements, as well as other components and markup.
    /// </summary>
    [Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = ["class"];

    [Inject]
	private IServiceProvider ServiceProvider { get; init; } = default!;

    private readonly List<Tab> Children = [];
	private ILogger<Tabs>? Logger;

	/// <summary>
	/// The index of the currently active tab.
	/// </summary>
	/// <remarks>
	/// Serves as a zero-based identifier for the active tab within the component. A value of -1 indicates that no tab is currently active.
	/// </remarks>
	public int ActiveIndex { get; internal set; } = -1;

	private string MainCssClass
	{
		get
		{
            var css = "tabs is-size-6";

			if (IsCentered)
				css += " is-centered";

			if (IsBoxed)
				css += " is-boxed";
			else if (IsToggle)
				css += " is-toggle";

			if (IsToggle && IsRounded)
				css += " is-toggle-rounded";

            return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
        }
	}

	/// <inheritdoc/>
	protected async override Task OnInitializedAsync()
	{
		Logger = ServiceProvider.GetService<ILogger<Tabs>>();

		if (ActiveIndex == -1 && Children.Count > 0)
		{
			ActiveIndex = Children[0].Index;
			Active = Children[0].Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}
	}

	/// <inheritdoc/>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		var active = Children.FirstOrDefault(x => x.Index == ActiveIndex);

		if ((active != null && Active != active.Name) || (active == null && string.IsNullOrWhiteSpace(Active) == false))
		{
			if (string.IsNullOrWhiteSpace(Active) == false)
				ActiveIndex = Children.FirstOrDefault(x => x.Name == Active)?.Index ?? -1;
			else
				ActiveIndex = -1;
		}
	}

	internal async Task AddChild(Tab tab)
	{
		if (tab.Name == null)
		{
			Logger?.LogError("Tabs must have a name assigned.");
			return;
		}

		tab.Index = Children.Count != 0 ? Children.Max(x => x.Index) + 1 : 0;
		Children.Add(tab);

		if (ActiveIndex == -1)
		{
			ActiveIndex = tab.Index;
			Active = tab.Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}

		StateHasChanged();
	}

	internal async Task RemoveChild(Tab tab)
	{
		var child = Children.FirstOrDefault(x => x.Index == tab.Index);

		if (child == null)
		{
			Logger?.LogError("Could not find tab to remove with name {name}.", tab.Name);
			return;
		}

		Children.Remove(child);

		if (ActiveIndex == child.Index && Children.Count > 0)
		{
			ActiveIndex = Children[0].Index;
			Active = Children[0].Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}

		StateHasChanged();
	}

	private async Task OnSelectionChanged(Tab tab)
	{
		if (tab.AdditionalAttributes != null && tab.AdditionalAttributes.Any(x => x.Key == "disabled" && (x.Value.ToString() == "disabled" || x.Value.ToString() == "true")))
			return;

        if (OnItemClicked != null)
            await OnItemClicked.Invoke(tab.Name);

		if (OnTabClicked.HasDelegate)
			await OnTabClicked.InvokeAsync(tab);

		if (ActiveIndex != tab.Index)
		{
			ActiveIndex = tab.Index;
			Active = tab.Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}
	}

	private string GetChildCssClass(Tab tab)
	{
        string? css;

        if (tab.Index == 0)
			css = "mr-1";
		else if (tab.Index == Children.Count - 1)
			css = "ml-1";
		else
			css = "mx-1";

		if (ActiveIndex == tab.Index)
			css += " is-active";

        return string.Join(' ', css, tab.AdditionalAttributes.GetValue("class"));
    }
}

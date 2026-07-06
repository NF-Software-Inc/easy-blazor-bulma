using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace easy_blazor_bulma;

/// <summary>
/// In-depth steps for multi-step forms or wizards
/// </summary>
/// <remarks>
/// <see href="https://octoshrimpy.github.io/bulma-o-steps/">GitHub Documentation</see>
/// </remarks>
public partial class Steps : ComponentBase
{
	/// <summary>
	/// The name of the step that is currently active.
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
	/// Displays the steps in a vertical layout when true.
	/// </summary>
	[Parameter]
	public bool IsVertical { get; set; }

	/// <summary>
	/// Positions each step in the center of the area when true.
	/// </summary>
	[Parameter]
	public bool IsCentered { get; set; } = true;

	/// <summary>
	/// Specifies whether to display the text above or below the dotted line.
	/// </summary>
	[Parameter]
	public bool TextAbove { get; set; }

	/// <summary>
	/// Specifies whether to allow clicking each step.
	/// </summary>
	[Parameter]
	public bool IsClickable { get; set; } = true;

	/// <summary>
	/// Event that occurs when a step in the steps component is clicked.
	/// </summary>
	[Parameter]
	public EventCallback<Step> OnStepClicked { get; set; }

	/// <summary>
	/// The content to display within the steps component. Can contain <see cref="Step"/> elements, as well as other components and markup.
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

	private readonly List<Step> Children = [];
	private ILogger<Steps>? Logger;

	/// <summary>
	/// The index of the currently active step.
	/// </summary>
	/// <remarks>
	/// Serves as a zero-based identifier for the active step within the component. A value of -1 indicates that no step is currently active.
	/// </remarks>
	public int ActiveIndex { get; internal set; } = -1;

	private string MainCssClass
	{
		get
		{
			var css = "steps";

			if (IsVertical)
				css += " is-vertical";

			if (TextAbove)
				css += " has-content-above";

			if (IsCentered)
				css += " has-content-centered";

			return string.Join(' ', css, AdditionalAttributes.GetValue("class"));
		}
	}

	/// <inheritdoc/>
	protected async override Task OnInitializedAsync()
	{
		Logger = ServiceProvider.GetService<ILogger<Steps>>();

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

	internal async Task AddChild(Step step)
	{
		if (step.Name == null)
		{
			Logger?.LogError("Steps must have a name assigned.");
			return;
		}

		step.Index = Children.Count != 0 ? Children.Max(x => x.Index) + 1 : 0;
		Children.Add(step);

		if (ActiveIndex == -1)
		{
			ActiveIndex = step.Index;
			Active = step.Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}

		StateHasChanged();
	}

	internal async Task RemoveChild(Step step)
	{
		var child = Children.FirstOrDefault(x => x.Index == step.Index);

		if (child == null)
		{
			Logger?.LogError("Could not find step to remove with name {name}.", step.Name);
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

	private async Task OnSelectionChanged(Step step)
	{
		if (IsClickable == false)
			return;

		if (OnStepClicked.HasDelegate)
			await OnStepClicked.InvokeAsync(step);

		if (ActiveIndex != step.Index)
		{
			ActiveIndex = step.Index;
			Active = step.Name;

			if (ActiveChanged.HasDelegate)
				await ActiveChanged.InvokeAsync(Active);
		}
	}

	private string GetChildCssClass(Step step)
	{
		var css = "steps-segment";

		if (step.Index == ActiveIndex)
			css += " is-active";

		if (step.Index >= ActiveIndex)
			css += " is-dashed";

		return string.Join(' ', css, step.AdditionalAttributes.GetValue("class"));
	}

	private string GetMarkerCssClass(Step step)
	{
		var css = "steps-marker";

		if (step.Index == ActiveIndex)
			css += " is-active";

		if (IsClickable)
			css += " is-clickable";

		if (step.MarkerColor != BulmaColors.Default)
			css += ' ' + BulmaColorHelper.GetColorCss(step.MarkerColor);

		return string.Join(' ', css, step.AdditionalAttributes.GetValue("marker-class"));
	}

	private string GetContentCssClass(Step step) => string.Join(' ', "steps-content is-size-4", step.AdditionalAttributes.GetValue("content-class"));
}

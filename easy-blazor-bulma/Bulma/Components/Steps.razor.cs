﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;

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
	/// The name of the tab that is currently displayed.
	/// </summary>
	[Parameter]
    public string? Active { get; set; }

    /// <summary>
    /// Event that occurs when <see cref="Active"/> is modified.
    /// </summary>
    [Parameter]
    public EventCallback<string?>? ActiveChanged { get; set; }

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
    /// Event that occurs when an item in the tab bar is clicked.
    /// </summary>
    [Parameter]
    public Func<string?, Task>? OnItemClicked { get; set; }

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

	[Inject]
	private IServiceProvider ServiceProvider { get; init; }

	private readonly List<Step> Children = new();
	private ILogger<Steps>? Logger;

	private string FullCssClass
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

            return css;
        }
    }

    private string? CssClass
    {
        get
        {
            if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("class", out var css) && string.IsNullOrWhiteSpace(Convert.ToString(css, CultureInfo.InvariantCulture)) == false)
                return css.ToString();

            return null;
        }
    }

    /// <inheritdoc/>
	protected async override Task OnInitializedAsync()
    {
		Logger = ServiceProvider.GetService<ILogger<Steps>>();

		if (string.IsNullOrWhiteSpace(Active) && Children.Count > 0)
        {
			Active = Children.First().Name;

			if (ActiveChanged.HasValue && ActiveChanged.Value.HasDelegate)
				await ActiveChanged.Value.InvokeAsync(Active);
		}
    }

    internal async Task AddChild(Step step)
    {
        if (step.Name == null)
        {
            Logger?.LogError("Steps must have a name assigned.");
            return;
        }
            
        else if (Children.Any(x => x.Name == step.Name))
        {
            Logger?.LogError("Steps must have a unique name. Duplicate is {name}.", step.Name);
            return;
        }
            

        step.Index = Children.Count != 0 ? Children.Max(x => x.Index) + 1 : 0;

        Children.Add(step);

        if (string.IsNullOrWhiteSpace(Active))
        {
			Active = step.Name;

			if (ActiveChanged.HasValue && ActiveChanged.Value.HasDelegate)
				await ActiveChanged.Value.InvokeAsync(Active);
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

        var i = 0;

        foreach (var item in Children.OrderBy(x => x.Index))
            item.Index = i++;

        if (Active == child.Name && Children.Count != 0)
        {
			Active = Children.First().Name;

			if (ActiveChanged.HasValue && ActiveChanged.Value.HasDelegate)
				await ActiveChanged.Value.InvokeAsync(Active);
		}

		StateHasChanged();
	}

    private async Task OnSelectionChanged(Step step)
    {
        if (IsClickable == false)
            return;

        if (OnItemClicked != null)
            await OnItemClicked.Invoke(step.Name);

        if (Active != step.Name)
        {
            Active = step.Name;

            if (ActiveChanged.HasValue && ActiveChanged.Value.HasDelegate)
                await ActiveChanged.Value.InvokeAsync(Active);
        }
    }

    private string? GetChildCssClass(Step step)
    {
        var active = Children.Single(x => x.Name == Active);
        var css = "steps-segment";

		if (step.Index == active.Index)
			css += " is-active";

		if (step.Index >= active.Index)
			css += " is-dashed";

		if (step.AdditionalAttributes != null && step.AdditionalAttributes.TryGetValue("class", out var stepCss) && string.IsNullOrWhiteSpace(Convert.ToString(stepCss, CultureInfo.InvariantCulture)) == false)
			css += $" {stepCss}";

		return css;
    }

    private string GetMarkerCssClass(Step step)
    {
        var css = "steps-marker";

        if (step.Name == Active)
            css += " is-active";

        if (IsClickable)
            css += " is-clickable";

        if (step.MarkerColor != BulmaColors.Default)
            css += ' ' + BulmaColorHelper.GetColorCss(step.MarkerColor);

        return css;
    }
}

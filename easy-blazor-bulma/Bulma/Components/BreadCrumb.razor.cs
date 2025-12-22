using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace easy_blazor_bulma;

/// <summary>
/// A simple breadcrumb component to improve your navigation experience.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/components/breadcrumb/">Bulma Documentation</see>
/// </remarks>
public partial class BreadCrumb : ComponentBase
{
    /// <summary>
    /// The content to display within the breadcrumb. Can contain <see cref="BreadCrumbItem"/> elements.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly string[] Filter = new[] { "class" };

    [Inject]
    private IServiceProvider ServiceProvider { get; init; } = default!;

    private string MainCssClass => string.Join(' ', "breadcrumb", AdditionalAttributes.GetValue("class"));

	private readonly List<BreadCrumbItem> Children = new();
    private ILogger<BreadCrumb>? Logger;

    /// <inheritdoc/>
	protected override void OnInitialized()
    {
        Logger = ServiceProvider.GetService<ILogger<BreadCrumb>>();
    }

    internal void AddChild(BreadCrumbItem item)
    {
        item.Index = Children.Count != 0 ? Children.Max(x => x.Index) + 1 : 0;
		item.IsLast = true;

		foreach (var child in Children)
            child.IsLast = false;

		Children.Add(item);
		StateHasChanged();
    }

    internal void RemoveChild(BreadCrumbItem item)
    {
        var match = Children.FirstOrDefault(x => x.Index == item.Index);

        if (match == null)
        {
            Logger?.LogError("Could not find breadcrumb item to remove with name {name}.", item.DisplayText);
            return;
        }

        Children.Remove(match);

        var i = 0;

        foreach (var child in Children.OrderBy(x => x.Index))
        {
            child.Index = i++;
            child.IsLast = false;
        }

        var last = Children.LastOrDefault();

        if (last != null)
            last.IsLast = true;

        StateHasChanged();
    }
}

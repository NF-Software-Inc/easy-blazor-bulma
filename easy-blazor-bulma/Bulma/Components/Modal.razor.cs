using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace easy_blazor_bulma;

/// <summary>
/// A classic modal overlay, in which you can include any content you want.
/// </summary>
/// <remarks>
/// There are 5 additional attributes that can be used: card-class, card-style, header-class, body-class, and foot-class. Each of which apply CSS classes to the resulting elements as per their names.
/// <see href="https://bulma.io/documentation/components/modal/">Bulma Documentation</see>
/// </remarks>
public partial class Modal : ComponentBase
{
	/// <summary>
	/// Displays text in a bar at the top of the modal.
	/// </summary>
	[Parameter]
	public string? Title { get; set; }

	/// <summary>
	/// Specifies whether to display the modal.
	/// </summary>
	[Parameter]
	public bool IsDisplayed { get; set; }

	/// <summary>
	/// Expression for manual binding to <see cref="IsDisplayed"/>.
	/// </summary>
	[Parameter]
	public Expression<Func<bool>>? IsDisplayedExpression { get; set; }

	/// <summary>
	/// Event that occurs when the display status of the modal changes.
	/// </summary>
	[Parameter]
	public EventCallback<bool> IsDisplayedChanged { get; set; }

	/// <summary>
	/// Specifies whether to display the transparent overlay outside of the modal. Clicking this closes the modal.
	/// </summary>
	[Parameter]
	public bool UseBackground { get; set; } = true;

	/// <summary>
	/// The content to display within the body section of the modal.
	/// </summary>
	[Parameter]
	public RenderFragment? ModalBody { get; set; }

	/// <summary>
	/// The content to display within the footer section of the modal.
	/// </summary>
	[Parameter]
	public RenderFragment? ModalFooter { get; set; }

	/// <summary>
	/// Fallback for content to display within the body section of the modal. Applies when neither <see cref="ModalBody"/> nor <see cref="ModalFooter"/> are provided.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private readonly string[] Filter = new[] { "class", "card-class", "header-class", "body-class", "foot-class" };

	private string MainCssClass
	{
		get
		{
			var css = "modal";

			if (IsDisplayed)
				css += " is-active";

			return string.Join(' ', css, AdditionalAttributes.GetClass("class"));
		}
	}

	private string CardCssClass => string.Join(' ', "modal-card", AdditionalAttributes.GetClass("card-class"));
	private string? CardCssStyle => AdditionalAttributes.GetClass("card-style");

	private string HeaderCssClass => string.Join(' ', "modal-card-head", AdditionalAttributes.GetClass("header-class"));
	private string BodyCssClass => string.Join(' ', "modal-card-body", AdditionalAttributes.GetClass("body-class"));
	private string FootCssClass => string.Join(' ', "modal-card-foot", AdditionalAttributes.GetClass("foot-class"));

	private async Task CloseModal()
	{
		IsDisplayed = false;

		if (IsDisplayedChanged.HasDelegate)
			await IsDisplayedChanged.InvokeAsync(IsDisplayed);
	}
}

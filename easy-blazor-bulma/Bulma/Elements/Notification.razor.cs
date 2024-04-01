﻿using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace easy_blazor_bulma;

/// <summary>
/// Creates a bold notification block, to alert your users of something.
/// </summary>
/// <remarks>
/// <see href="https://bulma.io/documentation/elements/notification/">Bulma Documentation</see>
/// </remarks>
public partial class Notification : ComponentBase
{
	/// <summary>
	/// Displays a button to hide the notification from the display.
	/// </summary>
	[Parameter]
	public bool ShowDeleteButton { get; set; } = true;

	/// <summary>
	/// Specifies whether the notification should be hidden from display.
	/// </summary>
	[Parameter]
	public bool IsHidden { get; set; }
	/// <summary>
	/// Sets the color to use for the notification text and background.
	/// </summary>
	[Parameter]
	public BulmaColors Color { get; set; }

	/// <summary>
	/// Content to display within the notification.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Any additional attributes applied directly to the component.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? AdditionalAttributes { get; set; }

	private string FullCssClass
	{
		get
		{
			var css = "notification";

			if (IsHidden)
				css += " is-hidden";

			if (Color != BulmaColors.Default)
				css += ' ' + BulmaColorHelper.GetColorCss(Color);

			return string.Join(' ', css, CssClass);
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

	private void Delete()
	{
		IsHidden = true;
		StateHasChanged();
	}
}

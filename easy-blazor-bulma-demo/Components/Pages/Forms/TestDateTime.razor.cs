using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestDateTime : ComponentBase
{
	private readonly PageModel InputModel = new();

	private readonly InputDateTimeOptions DateOnlyOptions = InputDateTimeOptions.HoverPopout |
		InputDateTimeOptions.ShowResetButton |
		InputDateTimeOptions.ShowDate |
		InputDateTimeOptions.CloseOnDateClicked;

	private readonly InputDateTimeOptions DateBelowOptions = InputDateTimeOptions.ClickPopout |
		InputDateTimeOptions.PopoutBottom |
		InputDateTimeOptions.PopoutLeft |
		InputDateTimeOptions.ShowNowButton |
		InputDateTimeOptions.ShowResetButton |
		InputDateTimeOptions.UpdateOnPopoutChange |
		InputDateTimeOptions.UseAutomaticStatusColors |
		InputDateTimeOptions.ShowDate |
		InputDateTimeOptions.ShowHours |
		InputDateTimeOptions.ShowMinutes |
		InputDateTimeOptions.CloseOnDateClicked |
		InputDateTimeOptions.ValidateTextInput |
		InputDateTimeOptions.ShowFullValueBelow;

	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;

	private class PageModel
	{
		[Display(Name = "Date Time Test", Description = "Input to test a non-nullable DateTime value.")]
		public DateTime DateTimeTest { get; set; } = DateTime.Now;

		[Display(Name = "Date Only Test", Description = "Input to test a nullable DateOnly value.")]
		public DateOnly? DateOnlyTest { get; set; }

		[Display(Name = "Full Date Test", Description = "Input to display the full DateTime below the input.")]
		public DateTime? FullDateTest { get; set; }
	}
}

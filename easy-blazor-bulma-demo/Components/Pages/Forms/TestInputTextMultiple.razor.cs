using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestInputTextMultiple : ComponentBase
{
	private readonly PageModel InputModel = new()
	{
		RequiredSkills = ["C#", "Blazor"],
		InterviewKeywords = ["Team Player", "Team Player"]
	};

	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;
	private string? SubmitMessage;

	private void OnSubmit()
	{
		SubmitMessage = $"Submit at {DateTime.Now:G}";
	}

	private class PageModel
	{
		[Display(Name = "Required Skills", Description = "Add one or more required skills. Duplicate entries are ignored.")]
		public List<string> RequiredSkills { get; set; } = [];

		[Display(Name = "Interview Keywords", Description = "Add one or more keywords. Duplicate entries are allowed in this example.")]
		public List<string> InterviewKeywords { get; set; } = [];
	}
}

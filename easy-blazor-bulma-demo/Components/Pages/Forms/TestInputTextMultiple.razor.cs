using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestInputTextMultiple : ComponentBase
{
	private readonly PageModel InputModel = new()
	{
		RequiredSkills1 = ["C#", "Blazor", "blazor"],
		RequiredSkills2 = ["C#", "Blazor"],
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
		[Display(Name = "Required Skills 1", Description = "Add one or more required skills. Duplicate entries are ignored. Case sensitive.")]
		public List<string> RequiredSkills1 { get; set; } = [];

		[Display(Name = "Required Skills 2", Description = "Add one or more required skills. Duplicate entries are ignored. Case insensitive.")]
		public List<string> RequiredSkills2 { get; set; } = [];

		[Display(Name = "Interview Keywords 1", Description = "Add one or more keywords. Duplicate entries are allowed in this example.")]
		public List<string> InterviewKeywords { get; set; } = [];
	}
}

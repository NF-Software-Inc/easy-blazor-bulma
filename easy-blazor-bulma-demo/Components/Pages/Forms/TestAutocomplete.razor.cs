using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestAutocomplete : ComponentBase
{
	private readonly PageModel InputModel = new()
	{
		SelectedItem1 = AllItems[2]
	};

	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;
	private string? Filter;
	private string? SubmitMessage;

	private readonly InputAutocompleteOptions Options2 =
		InputAutocompleteOptions.TypePopout |
		InputAutocompleteOptions.PopoutTop |
		InputAutocompleteOptions.UseAutomaticStatusColors;

	private readonly InputAutocompleteOptions Options3 =
		InputAutocompleteOptions.TypePopout |
		InputAutocompleteOptions.ClickPopout |
		InputAutocompleteOptions.PopoutTop |
		InputAutocompleteOptions.UseAutomaticStatusColors |
		InputAutocompleteOptions.AutoSelectOnExit;


	private static readonly List<DemoAutocomplete> AllItems =
	[
		new DemoAutocomplete { Id = 1, Name = "Jimbo Moneybags", Age = 30, Position = "Accountant" },
		new DemoAutocomplete { Id = 2, Name = "Suzy Goldenfold", Age = 18, Position = "Accountant" },
		new DemoAutocomplete { Id = 3, Name = "Sal Mopnbucket", Age = 44, Position = "Janitor" },
		new DemoAutocomplete { Id = 4, Name = "Jill Broomhandel", Age = 61, Position = "Janitor" },
		new DemoAutocomplete { Id = 5, Name = "Reggie Cashonlee", Age = 23, Position = "Accountant" },
		new DemoAutocomplete { Id = 6, Name = "Oldie Butagoodie", Age = 72, Position = "Janitor" },
		new DemoAutocomplete { Id = 7, Name = "Rickie Notapenny", Age = 56, Position = "Accountant" },
		new DemoAutocomplete { Id = 8, Name = "Cheryl McMillions", Age = 33, Position = "Accountant" },
		new DemoAutocomplete { Id = 9, Name = "Lindsay Spicnspan", Age = 46, Position = "Janitor" },
		new DemoAutocomplete { Id = 10, Name = "Bob Skidmarks-McNastypants", Age = 55, Position = "Janitor" },
		new DemoAutocomplete { Id = 11, Name = "Pat Waxon", Age = 24, Position = "Janitor" },
		new DemoAutocomplete { Id = 12, Name = "Melissa Taxevader", Age = 36, Position = "Accountant" },
		new DemoAutocomplete { Id = 13, Name = "Larry Robberson", Age = 47, Position = "Accountant" },
		new DemoAutocomplete { Id = 14, Name = "Pat Waxoff", Age = 19, Position = "Janitor" },
		new DemoAutocomplete { Id = 15, Name = "Betty Terletson", Age = 20, Position = "Janitor" }
	];

	private IEnumerable<DemoAutocomplete> GetAutocompleteItems()
	{
		if (string.IsNullOrWhiteSpace(Filter) == false)
			return AllItems.Where(x => x.Position == "Accountant" && x.Name.StartsWith(Filter, StringComparison.OrdinalIgnoreCase));
		else
			return AllItems.Where(x => x.Position == "Accountant");
	}

	private async Task OnItemsRequested(string? value)
	{
		Filter = value;

		if (value != null && value.Equals("More", StringComparison.OrdinalIgnoreCase))
		{
			AllItems.Add(new DemoAutocomplete { Id = AllItems.Max(x => x.Id) + 1, Name = "Dupie Duplicado", Age = 99, Position = "Accountant" });
		}
		else if (value != null && value.Equals("Less", StringComparison.OrdinalIgnoreCase))
		{
			var remove = AllItems.FirstOrDefault(x => x.Position == "Accountant");

			if (remove != null)
				AllItems.Remove(remove);
		}

		await Task.CompletedTask;
		StateHasChanged();
	}

	private void OnSubmit()
	{
		SubmitMessage = $"Submit at {DateTime.Now:G}";
	}

	private class PageModel
	{
		[Display(Name = "Janitor Applicant", Description = "Please type the name of the applicant to hire for the Janitor position.")]
		public DemoAutocomplete SelectedItem1 { get; set; } = AllItems[0];

		[Display(Name = "Accountant Applicant", Description = "Please type the name of the applicant to hire for the Accountant position.")]
		public DemoAutocomplete? SelectedItem2 { get; set; }

		[Display(Name = "Accountant Applicant with AutoSelect on Exit", Description = "Select an applicant in the drop down with arrow keys and press tab")]
		public DemoAutocomplete? SelectedItem3 { get; set; }
	}

	private record class DemoAutocomplete
	{
		public int Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public int Age { get; init; }

		public string Position { get; init; } = string.Empty;
	}
}

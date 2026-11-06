using easy_blazor_bulma;
using easy_blazor_bulma_demo.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestAutocomplete : ComponentBase
{
	/// <summary>
	/// The open generic type definition for the autocomplete input component.
	/// </summary>
    private static readonly Type AutocompleteType = typeof(InputAutocomplete<,>);

    /// <summary>
    /// The open generic type definition for multiple-value autocomplete inputs.
    /// </summary>
    private static readonly Type AutocompleteMultipleType = typeof(InputAutocompleteMultiple<,>);

    /// <summary>
    /// References the parameters section element in the rendered component. Used for scrolling to the section when "View all parameters" button is clicked.
    /// </summary>
    private ElementReference ParametersSection;

	/// <summary>
	/// Moves focus to the parameters section.
	/// </summary>
	private async Task JumpToParameters() => await ParametersSection.FocusAsync();

    private readonly PageModel InputModel = new()
	{
		SelectedItem1 = AllItems[2]
	};

	private DemoAutocomplete? PrivateSelection { get; set; } = AllItems[0];

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

	private readonly InputAutocompleteOptions Options4 =
		InputAutocompleteOptions.TypePopout |
		InputAutocompleteOptions.ClickPopout |
		InputAutocompleteOptions.PopoutTop |
		InputAutocompleteOptions.UseAutomaticStatusColors |
		InputAutocompleteOptions.AutoSelectOnExit;


    private static readonly List<DemoAutocomplete> AllItems = AutocompleteSampleData.Applicants;

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
		public IAutocomplete SelectedItem1 { get; set; } = AllItems[0];

		[Display(Name = "Accountant Applicant", Description = "Please type the name of the applicant to hire for the Accountant position.")]
		public DemoAutocomplete? SelectedItem2 { get; set; }

		[Display(Name = "Accountant Applicant with AutoSelect on Exit", Description = "Select an applicant in the drop down with arrow keys and press tab")]
		public IAutocomplete? SelectedItem3 { get; set; }

		[Display(Name = "Interview Selections", Description = "Select one or more applicants from the drop down.")]
		public List<DemoAutocomplete> SelectedItems4 { get; set; } = [];

		[Display(Name = "Applicant ID", Description = "Select an applicant while binding only their ID.")]
		public int? SelectedItemId5 { get; set; } = AllItems[7].Id;

		[Display(Name = "Applicant IDs", Description = "Select multiple applicants while binding only their IDs.")]
		public List<int> SelectedItemIds6 { get; set; } = [AllItems[3].Id, AllItems[7].Id];

		[Display(Name = "Field Applicant", Description = "Select an applicant using a field instead of property.")]
		public DemoAutocomplete? FieldSelection = AllItems[1];
	}

}

using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;
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

	private readonly PageModel InputModel = new();

	private DemoAutocomplete? PrivateSelection { get; set; } = AllItems[0];

	private readonly TooltipOptions TooltipDisplayMode = TooltipOptions.Right | TooltipOptions.HasArrow | TooltipOptions.Multiline;
	private string? SubmitMessage;

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

	private void OnSubmit()
	{
		SubmitMessage = $"Submit at {DateTime.Now:G}";
	}

    private class PageModel
	{
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

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


    private static readonly List<DemoAutocomplete> AllItems = AutocompleteSampleData.Applicants;

    private class PageModel
	{
		[Display(Name = "Field Applicant", Description = "Select an applicant using a field instead of property.")]
		public DemoAutocomplete? FieldSelection = AllItems[1];
	}

}

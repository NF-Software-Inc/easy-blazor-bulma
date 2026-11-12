using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestAutocomplete : TestComponentBase
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
    protected override ElementReference ParametersSection { get; set; }

}

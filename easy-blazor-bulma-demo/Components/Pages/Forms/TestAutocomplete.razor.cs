using easy_blazor_bulma;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestAutocomplete
{
    /// <summary>
    /// Autocomplete component types documented on this test page.
    /// </summary>
    private static readonly Type[] ComponentTypes =
    [
        typeof(InputAutocomplete<,>),
        typeof(InputAutocompleteMultiple<,>)
    ];

}

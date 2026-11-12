using easy_blazor_bulma;

namespace easy_blazor_bulma_demo.Components.Pages.Forms;

public partial class TestAutocomplete : TestComponentBase
{
    /// <summary>
    /// Cached array of autocomplete component types documented on this test page.
    /// </summary>
    private static readonly Type[] _componentTypes =
    {
        typeof(InputAutocomplete<,>),
        typeof(InputAutocompleteMultiple<,>)
    };

    /// <inheritdoc />
    protected override Type[] ComponentTypes => _componentTypes;
}

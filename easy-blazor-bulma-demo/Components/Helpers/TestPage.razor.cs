using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Helpers
{
    public partial class TestPage : ComponentBase
    {
        /// <summary>
        /// The title displayed for the test page.
        /// </summary>
        [EditorRequired]
        [Parameter]
        public required string Title { get; set; }

        /// <summary>
        /// The component types documented on the test page. Used to generate the information and parameters sections if custom content is not provided.
        /// </summary>
        [EditorRequired]
        [Parameter]
        public required Type[] ComponentTypes { get; set; }

        /// <summary>
        /// The demo examples displayed on the test page.
        /// </summary>
        [EditorRequired]
        [Parameter]
        public required RenderFragment Examples { get; set; }

        /// <summary>
        /// Custom content for the information panel. If omitted, component XML documentation is displayed.
        /// </summary>
        [Parameter]
        public RenderFragment? Information { get; set; }

        /// <summary>
        /// Custom content for the parameters section. If omitted, parameter tables are generated from component XML documentation.
        /// </summary>
        [Parameter]
        public RenderFragment? Parameters { get; set; }

        /// <summary>
        /// The referenced parameters section element. Used for scrolling to the section.
        /// </summary>
        private ElementReference ParametersSection;

        /// <summary>
        /// Moves focus to the parameters section.
        /// </summary>
        private async Task JumpToParameters() =>
            await ParametersSection.FocusAsync();
    }
}

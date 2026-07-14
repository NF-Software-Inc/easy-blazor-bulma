using easy_blazor_bulma;
using Microsoft.AspNetCore.Components;

namespace easy_blazor_bulma_demo.Components.Pages.Layout;

public partial class TestMasonry : ComponentBase
{
    private Masonry? _masonry;
    private readonly List<DemoTile> Tiles = [];
    private bool UseInfiniteScroll = true;
    private bool IsBusy;

    /// <summary>
    /// CSS selector used by Masonry.js to identify layout items.
    /// </summary>
    private string MasonryItemSelector = MasonryItem.DefaultSelector;

    /// <summary>
    /// Fixed column width in pixels.
    /// </summary>
    private int MasonryColumnWidth = 240;

    /// <summary>
    /// Space between items in pixels.
    /// </summary>
    private int MasonryGutter = 12;

    /// <summary>
    /// Whether to use percentage-based positioning.
    /// </summary>
    private bool MasonryPercentPosition = true;

    /// <summary>
    /// Whether to order items horizontally instead of vertically.
    /// </summary>
    private bool MasonryHorizontalOrder;

    /// <summary>
    /// The duration of the transition animation in milliseconds.
    /// </summary>
    private int MasonryTransitionMs = 400;

    /// <summary>
    /// Indicates whether a pending append operation is waiting to be processed after the next render.
    /// </summary>
    private bool _pendingAppend;

    /// <summary>
    /// Keeps track of the next unique ID to assign to a new tile.
    /// </summary>
    private int _nextId;

    /// <summary>
    /// A catalog of demo tile templates used to populate the masonry layout.
    /// </summary>
    private static readonly List<DemoTileTemplate> Catalog =
    [
        new("Beach Walk", "https://picsum.photos/id/10/420/300", "A simple image card inside a masonry item."),
        new("Forest Path", "https://picsum.photos/id/20/420/520", "Different heights show the masonry flow naturally."),
        new("City Lights", "https://picsum.photos/id/30/420/340", "This tile demonstrates variable image dimensions."),
        new("Mountain View", "https://picsum.photos/id/40/420/560", "Cards are rendered as content blocks with media."),
        new("Bridge", "https://picsum.photos/id/50/420/360", "Masonry keeps columns balanced."),
        new("Desert", "https://picsum.photos/id/60/420/480", "Image and text can be mixed in each card."),
        new("Morning Lake", "https://picsum.photos/id/70/420/320", "Useful for gallery-like layouts."),
        new("Coastline", "https://picsum.photos/id/80/420/540", "Each item can include metadata."),
        new("Night Sky", "https://picsum.photos/id/90/420/370", "Layout updates can be triggered manually."),
        new("Sunset", "https://picsum.photos/id/100/420/500", "Mimics records loaded from a data source with just enough information to keep the card readable while still showing that item text can vary slightly between entries."),
        new("Green Hills", "https://picsum.photos/id/110/420/350", "Tile content remains compact and readable."),
        new("River Bend", "https://picsum.photos/id/120/420/530", "Works with optional infinite scrolling."),
        new("Harbor", "https://picsum.photos/id/130/420/390", "A medium-length description to add more variety across cards."),
        new("Cliff Edge", "https://picsum.photos/id/140/420/560", "This entry intentionally has a longer description so you can observe how additional text impacts card height, which in turn affects Masonry positioning and the overall waterfall appearance of the grid."),
        new("Snow Trail", "https://picsum.photos/id/150/420/330", "Short text."),
        new("Golden Field", "https://picsum.photos/id/160/420/510", "This card contains an extra-long description intended to stress-test the layout behavior when some records have significantly more textual content than others. In practical terms, this can represent product descriptions, user-generated captions, or CMS-managed content where text length is inconsistent, and the component still needs to produce a clean, stable, and visually balanced masonry arrangement."),
        new("Old Town", "https://picsum.photos/id/170/420/355", "Demonstrates another compact card with modest text length."),
        new("Rainy Street", "https://picsum.photos/id/180/420/470", "Another long description example that simulates rich metadata loaded from a backend service, including summary text, optional notes, and occasionally verbose commentary entered by users or content editors."),
        new("Open Road", "https://picsum.photos/id/190/420/360", "Simple and concise."),
        new("Waterfall", "https://picsum.photos/id/200/420/580", "Tall image plus medium description creates a naturally larger tile."),
        new("Pine Valley", "https://picsum.photos/id/210/420/420", "Balanced card size for comparison against both short and very long entries."),
        new("Canyon", "https://picsum.photos/id/220/420/600", "This is an intentionally verbose, extra-long sample description used to verify that even with substantial text volume, lazy-loaded images, and asynchronous append operations, the Masonry layout remains stable after relayout calls and preserves a coherent visual rhythm across columns.")
    ];

    protected override void OnInitialized()
    {
        AddTiles(20);
        _pendingAppend = true;
    }

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">Indicates whether this is the first time the component is being rendered.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingAppend && _masonry != null)
        {
            _pendingAppend = false;
            await _masonry.Append();

            if (IsBusy)
            {
                IsBusy = false;
                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Loads more tiles into the masonry layout.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadMore()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        // Simulates a short API/database fetch so the demo feels realistic.
        await Task.Delay(250);
        AddTiles(10);
        _pendingAppend = true;

        StateHasChanged();
    }

    /// <summary>
    /// Manually triggers a layout update for the masonry component.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task RefreshLayout()
    {
        if (_masonry != null)
            await _masonry.Layout();
    }

    /// <summary>
    /// Applies the current Masonry option values without reinitializing the component.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task ApplyMasonryOptions()
    {
        if (_masonry != null)
            await _masonry.UpdateOptions();
    }

    /// <summary>
    /// Adds a specified number of tiles to the masonry layout.
    /// </summary>
    /// <param name="count">The number of tiles to add.</param>
    private void AddTiles(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var template = Catalog[_nextId % Catalog.Count];
            var id = _nextId + 1;
            _nextId++;

            Tiles.Add(new DemoTile(id, template.Title, template.ImageUrl, template.Description));
        }
    }

    /// <summary>
    /// Handles the event when an image has finished loading.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task OnImageLoaded()
    {
        if (_masonry != null)
            await _masonry.Layout();
    }

    /// <summary>
    /// Represents a demo tile with an ID, title, image URL, and description.
    /// </summary>
    /// <param name="Id">The unique identifier of the tile.</param>
    /// <param name="Title">The title of the tile.</param>
    /// <param name="ImageUrl">The URL of the image for the tile.</param>
    /// <param name="Description">The description of the tile.</param>
    private sealed record DemoTile(int Id, string Title, string ImageUrl, string Description);

    /// <summary>
    /// Represents a template for creating demo tiles.
    /// </summary>
    /// <param name="Title">The title of the tile.</param>
    /// <param name="ImageUrl">The URL of the image for the tile.</param>
    /// <param name="Description">The description of the tile.</param>
    private sealed record DemoTileTemplate(string Title, string ImageUrl, string Description);
}

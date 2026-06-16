# 🎯 Implement Masonry Layout Component with Optional Infinite Scroll

## Context

Adding a lightweight masonry (cascading grid) layout component to the easy-blazor-bulma library. The component will use Masonry.js bundled locally, follow existing patterns in the codebase, and provide optional infinite scroll functionality that users can opt out of.

## Technical Approach

### 1. JavaScript Integration
- Download and bundle Masonry.js v4.2.2 (latest stable) into `wwwroot/js/`
- Create wrapper functions in `JavaScript/easy-blazor-bulma.js` under `window.easyBlazorBulma.masonry` namespace
- Add C# extension methods in `Tools/JavaScriptExtensions.cs` following existing interop patterns
- Minify the bundled JS in release builds

### 2. Component Structure
Following the code-behind pattern used throughout the library:
- `Bulma/Layout/Masonry.razor` — Main container component
- `Bulma/Layout/Masonry.razor.cs` — Logic, parameters, JS interop
- `Bulma/Layout/MasonryItem.razor` — Individual item wrapper
- `Bulma/Layout/MasonryItem.razor.cs` — Item-specific logic

### 3. Infinite Scroll Support
- Create `Bulma/Layout/MasonryInfiniteScroll.razor` — Optional wrapper component
- Uses IntersectionObserver via JS interop
- Provides `OnLoadMore` event callback
- Can wrap `<Masonry>` or be disabled entirely

### 4. Key Design Decisions
- **Bundled JS**: Aligns with self-contained library pattern (no CDN dependencies)
- **Manual image handling**: User controls when to call `Layout()` after images load
- **Individual parameters**: More Blazor-idiomatic than flags enum
- **Opt-in infinite scroll**: Core `<Masonry>` is standalone; `<MasonryInfiniteScroll>` is optional wrapper

### 5. Configuration Parameters

**Masonry Component:**
- `ItemSelector` (string) — CSS selector for items (default: ".masonry-item")
- `ColumnWidth` (int?) — Fixed column width in pixels (optional)
- `ColumnWidthSelector` (string?) — CSS selector for sizing element
- `Gutter` (int) — Space between items in pixels (default: 0)
- `PercentPosition` (bool) — Use percent-based positioning (default: true)
- `HorizontalOrder` (bool) — Layout items left-to-right (default: false)
- `TransitionDuration` (string) — CSS transition duration (default: "0.4s")
- `InitializeOnRender` (bool) — Auto-init after first render (default: true)
- `AdditionalAttributes` — Standard capture for class, id, etc.

**MasonryItem Component:**
- Just wraps content and applies the "masonry-item" class
- `AdditionalAttributes` for custom styling

**MasonryInfiniteScroll Component:**
- `Threshold` (double) — Intersection ratio to trigger load (default: 0.1)
- `RootMargin` (string) — Margin around root element (default: "0px")
- `OnLoadMore` (EventCallback) — Fires when sentinel enters viewport
- `IsEnabled` (bool) — Enable/disable infinite scroll (default: true)
- `IsBusy` (bool) — Prevent multiple simultaneous loads
- `ChildContent` (RenderFragment) — Wraps the `<Masonry>` component

### 6. File References

**Library Files:**
- `easy-blazor-bulma/JavaScript/easy-blazor-bulma.js` — Add masonry wrapper functions
- `easy-blazor-bulma/Tools/JavaScriptExtensions.cs` — Add C# interop extensions
- `easy-blazor-bulma/Bulma/Layout/Masonry.razor` (new)
- `easy-blazor-bulma/Bulma/Layout/Masonry.razor.cs` (new)
- `easy-blazor-bulma/Bulma/Layout/MasonryItem.razor` (new)
- `easy-blazor-bulma/Bulma/Layout/MasonryItem.razor.cs` (new)
- `easy-blazor-bulma/Bulma/Layout/MasonryInfiniteScroll.razor` (new)
- `easy-blazor-bulma/Bulma/Layout/MasonryInfiniteScroll.razor.cs` (new)
- `easy-blazor-bulma/wwwroot/js/masonry.min.js` (new, downloaded)
- `easy-blazor-bulma/wwwroot/js/masonry.pkgd.min.js` (packaged version with dependencies)

**Demo Files:**
- `easy-blazor-bulma-demo/Components/Pages/Layout/Masonry.razor` (new test page)
- `easy-blazor-bulma-demo/Components/Layout/NavMenu.razor` — Add link to masonry test

### 7. Implementation Risks

**Risk 1: Masonry.js licensing**
Masonry is MIT licensed, fully compatible with this project. No concerns.

**Risk 2: Bundle size increase**
Masonry.js minified is ~7KB. Negligible impact.

**Risk 3: IntersectionObserver browser support**
Well-supported (IE11+ with polyfill). Document as requirement or fallback gracefully.

**Risk 4: Blazor render timing**
Masonry must init after DOM is fully rendered. Use `OnAfterRenderAsync` with `firstRender` check.

**Risk 5: Component disposal**
Ensure Masonry instances are properly destroyed on component disposal to prevent memory leaks.

**Last Updated**: 2026-06-16 14:05:12

## 📝 Plan Steps
-  **Download Masonry.js — Fetch `masonry.pkgd.min.js` v4.2.2 from unpkg.com or GitHub releases and place in `easy-blazor-bulma/wwwroot/js/`**
-  **Update JavaScript wrapper — Add five functions to `JavaScript/easy-blazor-bulma.js`:**
-  **Add C# extension methods — In `Tools/JavaScriptExtensions.cs`, add:**
-  **Create Masonry.razor — Component template with:**
-  **Create Masonry.razor.cs — Component code-behind with:**
-  **Create MasonryItem.razor — Simple wrapper:**
-  **Create MasonryItem.razor.cs — Minimal code-behind:**
-  **Create MasonryInfiniteScroll.razor — Wrapper component with:**
-  **Create MasonryInfiniteScroll.razor.cs — Logic with:**
-  **Add demo page — Create `easy-blazor-bulma-demo/Components/Pages/Layout/Masonry.razor` with:**
-  **Update NavMenu — Add `<NavbarItem href="masonry">Masonry</NavbarItem>` under Layout dropdown**
-  **Update bundler config — Ensure `bundleconfig.json` includes new JS files for minification (if not already auto-detected)**
-  **Add XML documentation — Comprehensive `<summary>` and `<remarks>` for all components, parameters, and public methods; include usage examples and link to Masonry.js docs**
-  **Test all scenarios — Verify:**


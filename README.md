# Easy Blazor Bulma

[![MIT](https://img.shields.io/github/license/NF-Software-Inc/Easy-Blazor-Bulma)](https://github.com/NF-Software-Inc/Easy-Blazor-Bulma/blob/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Easy.Blazor.Bulma.svg)](https://www.nuget.org/packages/Easy.Blazor.Bulma/)
[![Build](https://img.shields.io/github/actions/workflow/status/NF-Software-Inc/easy-blazor-bulma/build.yml)](https://github.com/NF-Software-Inc/easy-blazor-bulma/actions/workflows/build.yml)
[![Publish](https://img.shields.io/github/actions/workflow/status/NF-Software-Inc/easy-blazor-bulma/publish.yml?label=publish)](https://github.com/NF-Software-Inc/easy-blazor-bulma/actions/workflows/publish.yml)

## Getting Started

This library provides an easy to use, out of the box implementation of the Bulma CSS framework for use with Blazor. It uses the version 0.9.4 of Bulma. There are also several extensions to Bulma that are included and integrated here. Additionally, the Google Material Symbols font is packaged here also. All of this is takes just a single line in your index.html or single using statement in your CS files to start using.

After install, you can use all of the Bulma styles, along with those of the extension packages on your own components. You may want to store a copy of the pre-compiled CSS files somewhere in your solution so that Visual Studio can use Intellisense to give you CSS class recommendations. You will also be able to use the `.material-icons` CSS class to use any of the Material Symbols. The icon font will also be updated regularly, let us know if there are any missing and we will update it.

### Installation

To use this library: 

* Clone a copy of the repository
* Reference the [NuGet package](https://www.nuget.org/packages/Easy.Blazor.Bulma/)
* Try it on Android with the APK file (see releases)
* Try it on Windows with the [Microsoft Store App](https://apps.microsoft.com/detail/9p09hv890mh2) or MSIX package (see releases)

### Features

* Pre-compiled CSS bundle containing Bulma, several extensions, and a dark theme
* Interactive Blazor components for Bulma framework elements
* Additional components to help with common Blazor tasks
* Bundled with Google Material Symbols icon font
* No additional dependencies (Microsoft.AspNetCore.Components.Web is already required by Blazor)
* JavaScript extension methods for direct use with IJSRuntime

### Component List

The following components are currently available for use. All of them are documented and you can hover over them in Visual Studio to get details for the components and their parameters.

**Elements**

* `<BooleanIcon />`, creates an icon with either a check or 'X'
* `<ButtonBase />`, allows easily creating standard buttons
* `<Calendar />`, creates a grid style calendar to display dates and schedule items
* `<Card />`, creates an all-around flexible and composable component
* `<DeleteButton />`, creates a button to delete a record with a confirmation modal
* `<Message />`, creates a styled block containing a note to display
* `<Modal />`, creates an overlay that can display additional content
* `<Notification />`, creates a bold notification block, to alert your users of something
* `<Panel />`, creates a styled block containing content to display
* `<ProgressBar />`, creates a meter to display completion or loading status
* `<QrCode />`, creates a QR code image from text
* `<ResetButton />`, creates a button to reset contents of a form
* `<Steps />` and `<Step />`, creates a progress tracker with visual indicators
* `<SubmitButton />`, creates a button to submit contents of a form
* `<Table />`, creates an HTML table
* `<TableCellBase />`, allows easily creating table cells
* `<TableData />`, creates a td element within a table
* `<TableHeader />`, creates a th element within a table
* `<TableRow />`, creates a tr element within a table
* `<Tabs />` and `<Tab />`, creates a tab bar and content panels
* `<WeekScheduler />`, creates a weekly scheduler to display events

**Errors**

* `<_400 />`, displays a message with 400: Bad Request
* `<_401 />`, displays a message with 401: Unauthorized
* `<_403 />`, displays a message with 403: Forbidden
* `<_404 />`, displays a message with 404: Not Found
* `<_405 />`, displays a message with 405: Method Not Allowed
* `<_408 />`, displays a message with 408: Request Timeout
* `<_409 />`, displays a message with 409: Conflict
* `<_422 />`, displays a message with 422: Unprocessable Entity
* `<_423 />`, displays a message with 423: Locked
* `<_426 />`, displays a message with 426: Upgrade Required
* `<_500 />`, displays a message with 500: Internal Server Error
* `<_501 />`, displays a message with 501: Not Implemented
* `<_502 />`, displays a message with 502: Bad Gateway
* `<_503 />`, displays a message with 503: Service Unavailable
* `<_504 />`, displays a message with 504: Gateway Timeout
* `<ExtendedErrorBoundary />`, provides additional details on exceptions
* `<HttpRequestError />`, displays the error component corresponding to the HTTP status code of a failed request
* `<Unknown />`, displays a message with generic error details

**Form**

* `<InputAutocomplete />`, creates a drop-down list of options to select from
* `<InputCharacter />`, creates a series of buttons to select a single character
* `<InputDateTime />`, creates an input with a popout for binding date and time values
* `<InputDuration />`, creates an input with a popout for binding duration values
* `<InputFlaggedEnum />`, creates a series of checkboxes for bitmasked enum types
* `<InputNumberFormatted />`, creates an input for numeric values with custom formatting
* `<InputPassword />`, creates an input for entering passwords
* `<InputPhone />`, creates an input for phone numbers with custom formatting and validation
* `<InputNumberPad />`, creates a keyboard style number pad for numeric values
* `<InputRadioGroupObject />`, creates a radio group with the provided option list
* `<InputReadonly />`, creates an input to display a value that cannot be edited
* `<InputSelectObject />`, creates a select list for object types
* `<InputSelectMultipleObject />`, creates a select list for selection of multiple object types
* `<InputSwitch \>`, creates a sliding on off switch with an underlying checkbox
* `<Label />`, creates a label to display with form inputs

**Layout**

* `<Hero />`, creates an imposing hero banner to showcase something
* `<Level />` and `<LevelItem />`, displays a multi-purpose horizontal level
* `<Loader />`, displays a loading screen with a progress meter and message
* `<MarkupViewer />`, creates a viewer for displaying HTML markup safely
* `<Masonry />`, `<MasonryItem />`, and `<MasonryInfiniteScroll />`, creates a masonry layout for displaying items in a grid
* `<MediaObject />`, creates a media object prevalent in social media interfaces
* `<ThreeColumns />`, creates a simple three column layout
* `<TruncatedText />`, creates a block of text that is truncated after a set number of characters
* `<TwoColumns />`, creates a simple two column layout

**Navigation**

* `<BreadCrumb />` and `<BreadCrumbItem />`, creates a simple breadcrumb component to improve your navigation experience
* `<Clock />`, creates a live updating clock display
* `<Navbar />`, creates a navigation menu to use at the top of the screen
* `<NavbarDropdown />`, creates a dropdown to use in a Navbar menu
* `<NavbarItem />`, creates a link to use in a Navbar menu
* `<NavigationButtons />`, creates Previous and Next buttons for navigating between pages
* `<ScrollToTopButton />`, creates a button that will scroll the page to the top when clicked
* `<ThemeSelector />`, toggles between dark and light CSS themes (requires JavaScript file and both CSS stylesheets in index.html)
* `<TitleBlock />`, displays a banner accross the top of the screen with some text
* `<ZoomSelector />`, creates a drop-down list of zoom levels to select from and apply to a target element

### Usage

**Basic Example**

The following example provides a complete use case. This example makes use of the most basic configuration.

In your index.html file add the following line. You must do this even if you just want to use the components, they need this too.

```html
<link rel="stylesheet" href="_content/Easy.Blazor.Bulma/css/easy-blazor-bulma.css" />
```

This will link the CSS style pack and Google Material Symbols icon font.

Next, add one of the components or use some of the styles in your own components.

```razor
@using easy_blazor_bulma

<p class='block'>Here's a giant checkbox!</p>
<BooleanIcon class='is-size-1 is-jumbo' @bind-Value=Test />

@code {
	private bool Test { get; set; } = true;
}
```

**Dark Theme Example**

To use the dark theme, just add a secondary stylesheet reference in your index.html. The theme can automatically be selected based on the preference of the user. Changing between the styles can be done easily with an `IJSRuntime`.

```html
<head>
    <!-- Your head content -->
    <link rel="stylesheet" href="_content/Easy.Blazor.Bulma/css/easy-blazor-bulma.css" id="easy-blazor-bulma" media="(prefers-color-scheme: light)" />
    <link rel="stylesheet" href="_content/Easy.Blazor.Bulma/css/easy-blazor-bulma-dark.css" id="easy-blazor-bulma-dark" media="(prefers-color-scheme: dark)" />
</head>

<body>
    <!-- Your body content -->
    <script src="_content/Easy.Blazor.Bulma/js/easy-blazor-bulma.js" type="text/javascript"></script>
</body>
```

There are several helper methods in the bundled JavaScript file. The recommendation is to use the extension methods provided in the `easy_blazor_bulma` namespace to call these functions.

Additionally, you can use the `<ThemeSelector />` component to display a button that will toggle between themes. By default it applies the `.navbar-item` CSS class, if you provide another class this will override it. Simply including this component anywhere in your display will automatically load the correct theme when your app starts.

**Icons**

To use an icon just add a `<span>` with the desired icon text and the `.material-icons` CSS class. The sample below will produce the magnifier glass icon. An icon reference is available at [Google Fonts](https://fonts.google.com/icons).

```html
<span class="material-icons">search</span>
```

## Build Details

### Frameworks

- .NET 8.0
- .NET 9.0
- .NET 10.0

### External Libraries

| Name | Usage | License | Library |
| ------ | ------ | ------ | ------ |
| Easy Core | NuGet | MIT | [GitHub](https://github.com/NF-Software-Inc/easy-core) |
| Bulma | Submodule | MIT | [GitHub](https://github.com/jgthms/bulma) |
| Bulma Calendar | Submodule | MIT | [GitHub](https://github.com/Wikiki/bulma-calendar) |
| Bulma Checkradio | Submodule | MIT | [GitHub](https://github.com/Wikiki/bulma-checkradio) |
| Bulma-O-Steps | Submodule | GPL-3.0 | [GitHub](https://github.com/octoshrimpy/bulma-o-steps) |
| Bulma Switch  | Submodule | MIT | [GitHub](https://github.com/Wikiki/bulma-switch) |
| Bulma Tooltip | Submodule | MIT | [GitHub](https://github.com/CreativeBulma/bulma-tooltip) |
| Bulma Jumbo Tiny | Submodule | MIT | [GitHub](https://github.com/NF-Software-Inc/Bulma-Jumbo-Tiny) |
| Material Symbols | Embedded | Apache 2.0 | [GitHub](https://github.com/google/material-design-icons) |
| Masonry.js | Embedded | MIT | [GitHub](https://github.com/desandro/masonry) |
| qrcode.js | Embedded | MIT | [GitHub](https://github.com/davidshimjs/qrcodejs) |

## Authors

* **NF Software Inc.**

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

Thank you to:
* [Kmg Design](https://www.iconfinder.com/kmgdesignid) for the project icon

# Enable InputDateTime popout time entry
## Understanding
Apply the InputDuration direct-entry pattern to InputDateTime's existing hours, minutes, and seconds fields. Retain the calendar, arrows, display formatting, and existing value-commit behavior.
## Assumptions
- InputDateTime currently exposes three numeric time columns; day selection remains in the calendar and there is no milliseconds column or option. Adding new columns/options is outside this minimal change.
- Use the existing numeric input pattern and `is-spinnerless` utility without introducing new components or dependencies.
## Approach
Add private per-field focus state, CSS properties, and shared component limits in [InputDateTime.razor.cs](easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs). Route the three input change handlers through one small helper that parses a nonnegative integer, clamps to the component limit, and delegates the difference to the existing TimeSpan-based UpdatePopoutValue overload. Wire the existing markup in [InputDateTime.razor](easy-blazor-bulma/Bulma/Form/InputDateTime.razor) while retaining padded overlays and arrows.
## Key Files
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs - focus state, shared limits, and input events.
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor - existing time fields.
## Risks & Open Questions
- New direct-entry interactions require manual UI review; existing tests may not cover rendered input events.
## Steps
1. Implement time-field state and shared input updates in code-behind.
2. Wire direct entry in the three existing Razor time fields.
3. Build and run existing tests, then pause for review.
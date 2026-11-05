# Enable direct minute entry in InputDuration
## Understanding
Add direct numeric entry for the minutes column in the `InputDuration` popout, retaining its existing up/down arrows and leaving all other unit columns unchanged. Preserve the component's display modes, negative-duration convention, and popout update behavior.
## Assumptions
- The minutes column's edit/display overlay should follow the existing timepicker CSS behavior: toggle `is-input` on focus, with the number input visible during editing.
- When `DisplayHoursAsMinutes` is active, the minutes field represents total minutes; otherwise, with separate hours, it represents the minute component (0–59). If no hours are shown and this mode is inactive, retain the existing minute component semantics.
- Changing minutes should update only the minute quantity and preserve the other duration units, while `UpdatePopoutValue` continues applying existing limits and update-on-change behavior.
## Approach
Inspect the current minutes markup, duration value formatting and limits, and test conventions. Add a minutes input with focus-state styling and a focused numeric projection; on change, calculate an adjustment from the current displayed minutes and apply it through the shared duration update method. Add narrowly scoped tests for the calculation/update path if it can be tested without rendering, then build and run the test executable. Stop after minutes for user review.
## Key Files
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor` - minutes input and focus styling.
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs` - minute projection, focus state, and value update.
- `easy-blazor-bulma-test/InputDurationTests.cs` - focused tests for minute-value calculations if practical.
## Risks & Open Questions
- `DisplayHoursAsMinutes` changes the minute field from a component value to total minutes, so its permissible bounds differ from the standard 0–59 field.
- Negative durations display a separate sign; editing the minute quantity must preserve that sign rather than allow a second negative sign in the field.
## Steps
1. Inspect minute display modes, update behavior, and testable boundaries.
2. Implement the minute numeric input and focus-state styling in the Razor markup.
3. Implement minute projection and editing updates in the code-behind.
4. Add targeted tests for minute-entry behavior.
5. Build, run tests, and pause for user review.
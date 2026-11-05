# Enable direct day entry in InputDuration
## Understanding
Implement direct editing for the days unit in the `InputDuration` popout first, preserving the existing arrow controls and shared `PopoutValue` behavior. Use `InputNumberFormatted` as the model for formatting and numeric editing, but do not embed it until its binding and the duration-unit display conventions are understood.
## Assumptions
- This increment covers only the days unit; the other duration units remain unchanged for review.
- The day editor must preserve the remaining duration components and respect the component's existing negative-duration and 24-hour limits.
- The existing overlay styling may rely on the current nested input/value markup, so inspect the relevant styles before choosing markup.
## Approach
Inspect the `InputDuration` markup, `InputNumberFormatted` formatting and binding API, and applicable timepicker styles. Then implement a days-only editor and its code-behind binding, matching existing duration state conventions. Validate the touched files/build and stop for the user's review before extending the pattern to other units.
## Key Files
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor` - days popout editor markup.
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs` - duration state, formatting, and bounds.
- `easy-blazor-bulma/Bulma/Form/InputNumberFormatted.razor.cs` - reference for formatting and numeric field semantics.
## Risks & Open Questions
- Negative durations use a separate sign display while `TimeSpan` day components are signed; the editable representation must not accidentally invert the sign or discard sub-day components.
- `InputNumberFormatted` applies a standard input class; confirm it can fit the timepicker's existing layered styling before reusing it.
## Steps
1. Inspect days-unit formatting, state updates, and applicable timepicker styles.
2. Implement a bound day editor in `InputDuration` markup.
3. Implement day-value projection and update logic in the code-behind.
4. Validate the changed component and build.
5. Present the days-only result and pause for review.
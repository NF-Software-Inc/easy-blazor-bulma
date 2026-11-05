# Centralize InputDuration unit limits
## Understanding
Move the maximums for day, hour, minute, second, and millisecond popout inputs into reusable code-behind properties, then reference those properties from both Razor `max` attributes and change-handler validation. Preserve option-dependent total-unit limits and current behavior.
## Assumptions
- Existing component maximums are days 1,000,000; hours/minutes/seconds use component limits 23/59/59 unless the corresponding total-unit display option is enabled (then 1,000,000); milliseconds 999.
- Limits are inclusive and numeric inputs should use the same value enforced by code-behind.
- Preserve existing formatting APIs and input behavior.
## Approach
Inspect all five input attributes and handlers. Add constants for shared fixed maxima and computed properties for option-dependent maxima in `InputDuration.razor.cs`. Replace literals in markup and handlers with those properties, then build and run the existing InputDuration tests.
## Key Files
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor` - native `max` attributes.
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs` - centralized maximums and input validation.
## Risks & Open Questions
- Browser `max` is a client-side hint; the change handlers must still enforce the exact same maximum.
## Steps
1. Inspect existing maximum literals and option-dependent cases.
2. Add shared maximum definitions in the code-behind.
3. Reuse the definitions in Razor and handlers.
4. Build and run the focused duration tests.
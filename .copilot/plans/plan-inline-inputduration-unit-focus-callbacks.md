# Inline InputDuration unit focus callbacks
## Understanding
Move only the five popout units' focus-in/focus-out callbacks into Razor lambdas, as clarified. Keep focus fields, projections, CSS composition, and main-input logic in code-behind.
## Assumptions
- No behavior change or InputDateTime edits are requested.
## Approach
Replace named unit focus callbacks in [InputDuration.razor](easy-blazor-bulma/Bulma/Form/InputDuration.razor) with simple boolean-assignment lambdas, then remove those ten methods from [InputDuration.razor.cs](easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs).
## Key Files
- easy-blazor-bulma/Bulma/Form/InputDuration.razor - inline callbacks.
- easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs - remove unused callbacks.
## Risks & Open Questions
- Keep main textbox focus handling intact because it also opens the popout.
## Steps
1. Inline unit focus callbacks in Razor.
2. Remove unused unit focus methods from code-behind.
3. Build and run focused duration tests.
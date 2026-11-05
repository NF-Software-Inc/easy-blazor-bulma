# Inline InputDateTime unit callbacks
## Understanding
Apply the same callback cleanup as InputDuration: inline unit focus assignments and unit value-change forwarding calls in InputDateTime markup.
## Assumptions
- Keep shared update logic, state, limits, and main textbox handlers unchanged.
## Approach
Replace callbacks in [InputDateTime.razor](easy-blazor-bulma/Bulma/Form/InputDateTime.razor) and remove the nine unused forwarding methods in [InputDateTime.razor.cs](easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs).
## Key Files
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor - three unit callbacks.
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs - remove unused methods.
## Risks & Open Questions
- No behavior changes are intended.
## Steps
1. Inline focus and change callbacks in Razor.
2. Remove unused callbacks from code-behind.
3. Build and run focused InputDateTime tests.
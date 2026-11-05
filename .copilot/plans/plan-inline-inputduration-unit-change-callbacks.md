# Inline InputDuration unit change callbacks
## Understanding
Move the five thin unit-value-change handlers into InputDuration Razor lambdas. Keep shared parsing and update logic in code-behind.
## Assumptions
- This applies to InputDuration only; no behavior changes are requested.
## Approach
Replace the five named onchange callbacks in [InputDuration.razor](easy-blazor-bulma/Bulma/Form/InputDuration.razor) with lambdas calling UpdatePopoutUnit using the same arguments, then remove the unused forwarding methods from [InputDuration.razor.cs](easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs).
## Key Files
- easy-blazor-bulma/Bulma/Form/InputDuration.razor - onchange callbacks.
- easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs - remove forwarding handlers only.
## Risks & Open Questions
- Preserve all existing limits and shared validation logic.
## Steps
1. Inline the five unit change callbacks in Razor.
2. Remove unused forwarding methods from code-behind.
3. Build and run focused duration tests.
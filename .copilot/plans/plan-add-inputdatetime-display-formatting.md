# Add InputDateTime display formatting
## Understanding
Add main-textbox formatting equivalent to InputDuration, without changing calendar selection or popout time entry.
## Assumptions
- Formatter overrides DisplayFormat while unfocused; focus restores the existing editable representation.
- Culture applies to custom display formatting; existing parsing and default formatting remain unchanged in this focused increment.
## Approach
Add documented private-focus state and public formatting parameters in [InputDateTime.razor.cs](easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs), keeping the existing type-specific formatting as the fallback. Wire main textbox focus handlers in [InputDateTime.razor](easy-blazor-bulma/Bulma/Form/InputDateTime.razor), preserving OpenPopout on focus. Add focused formatting tests for supported types and formatter precedence.
## Key Files
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor.cs - formatting parameters and precedence.
- easy-blazor-bulma/Bulma/Form/InputDateTime.razor - main input focus events.
- easy-blazor-bulma-test/InputDateTimeTests.cs - new formatting tests.
## Risks & Open Questions
- Custom display strings need not be parseable, so they must not replace the focused editable representation.
## Steps
1. Implement main-textbox formatting and focus handling.
2. Add tests for custom formats and formatter precedence.
3. Build and run tests, then pause for review.
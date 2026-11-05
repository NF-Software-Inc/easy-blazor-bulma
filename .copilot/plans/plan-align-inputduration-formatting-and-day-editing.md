# Align InputDuration formatting and day editing
## Understanding
Revise the days-only change so `InputDuration` does not render `InputNumberFormatted`. Use the built-in `InputNumber<int>` for direct day entry, and add `DisplayFormat`/`Formatter` behavior by reference to `InputNumberFormatted`, applying those options only to the main duration textbox as requested.
## Assumptions
- `Formatter` takes precedence over `DisplayFormat` and both affect the main textbox only.
- The main textbox uses the existing `FormatTimeSpan`/`FormatTimeOnly` representation while focused, so text remains compatible with the existing parser; custom formatting applies while unfocused.
- The days editor remains a magnitude field with separate negative sign display and keeps the duration's smaller units and current constraints.
## Approach
Update `InputDuration.razor` to use `InputNumber<int>` for its days editor and track focus transitions on the main textbox. Add matching `DisplayFormat` and `Formatter` parameters and formatting precedence in the code-behind. Extend the existing format tests for the new parameters and verify the library/tests build, then stop for review before adding other unit editors.
## Key Files
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor` - days editor and main textbox focus event wiring.
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs` - formatting parameters and day update logic.
- `easy-blazor-bulma-test/InputDurationTests.cs` - formatting precedence and focused/unfocused behavior coverage.
## Risks & Open Questions
- Custom format strings must only affect the unfocused main textbox; displaying a non-parser-compatible custom string while focused would break ordinary editing.
- The main input opens the popout on focus; focus tracking must not interfere with that interaction.
## Steps
1. Replace the nested formatted-number component with the built-in numeric input and update main-input focus event wiring.
2. Implement `DisplayFormat` and `Formatter` on `InputDuration` with focused/unfocused formatting behavior.
3. Add tests for custom formatter and format-string precedence.
4. Build the library and run the relevant duration tests.
5. Summarize the days-only revision and pause for review.
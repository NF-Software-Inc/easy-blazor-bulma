# Remove days-only edits while preserving InputDuration formatting
## Understanding
Revert only the direct days-entry changes from the current working tree. Preserve the main-textbox `DisplayFormat`, `Formatter`, `Culture`, focus handling, explanatory popout comment, and formatting tests so the formatting work is isolated for review and commit.
## Assumptions
- The original days markup used a plain number placeholder plus the `PopoutValue.ToString("%d")` overlay.
- Day-specific state and event handlers (`PopoutDays`, days focus CSS state, and day change handler) are not part of the formatting change and should be removed.
- Existing `Culture` work is part of the formatting change and must remain.
## Approach
Restore the days markup only, remove only the days-specific code-behind properties and handlers, then verify the remaining diff and build/test the formatting-only changes.
## Key Files
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor` - restore the original days field markup while retaining main textbox focus wiring and the introductory comment.
- `easy-blazor-bulma/Bulma/Form/InputDuration.razor.cs` - remove day-editor state/handlers while preserving formatting parameters and focus formatting.
- `easy-blazor-bulma-test/InputDurationTests.cs` - retain DisplayFormat/Formatter tests unchanged.
## Risks & Open Questions
- The current working tree includes a `Culture` parameter; preserve it and its `FormatProvider` use as part of formatting rather than reverting it.
## Steps
1. Restore the original days markup.
2. Remove days-only editor state and handlers from the code-behind.
3. Verify the formatting-only diff and run build/tests.
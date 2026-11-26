# Test-driven development (mandatory)

All production code in `backend/src/` and `web/src/` is written test-first. Follow the
`tdd` skill (`.claude/skills/tdd/SKILL.md`) for the full loop and commands.

## The loop
1. **Test list first.** Before code, list the behaviors to prove. Derive them from the rule IDs
   (`CMD-…`): happy path, every boundary (exactly at cutoff, zero amount, last unit of
   capacity, effective-date edges), every rejection, tenant/scope isolation, idempotency.
2. **Red.** Write one test. Run it. It must fail **for the expected reason** (assertion or
   the expected missing member) — not a typo, wrong setup or unrelated failure.
3. **Green.** Write the minimum production code to pass. Do not add untested behavior.
4. **Refactor.** With all tests green, remove duplication and improve names to match the
   ubiquitous language. Run the whole affected suite again.
5. Repeat until the test list is empty. Add new list items as you discover them.

## Non-negotiables
- **Bug fix = failing test first** that reproduces the bug, then the fix.
- No production behavior without a test that would fail if that behavior were removed.
- Tests assert **business outcomes** (state, returned error code, emitted record), not
  implementation details (private calls, call counts on internal collaborators).
- Name tests as behavior sentences in the domain language:
  `Change_exactly_at_cutoff_is_rejected_for_employee`, not `Test1` or `SelectAsync_Works`.
- Tag backend tests with the rule they prove: `[Trait("Rule", "CMD-038")]`.
- Never weaken, skip or delete a failing test to get green. If a test is wrong, say why in the
  commit message.
- Never claim a test passed without running it; report the command and the result.

## Exempt (state why in the PR)
Pure configuration, scaffolding with no behavior, docs, and throwaway spikes (spikes are
never merged — re-implement test-first).

## Evidence
The PR body's **TDD evidence** section lists the test list, and for each rule the test name
and the red failure reason observed before the fix.

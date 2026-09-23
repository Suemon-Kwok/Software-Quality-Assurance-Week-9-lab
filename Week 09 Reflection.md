# Week 9 Reflection — TDD and QA in CI/CD

## What I built
- `Week09QualityLab.Core`: `DiscountCalculator` (Regular / Premium 10% / VIP 20%, negative prices rejected) and `ParkingFeeCalculator` ($4 first hour, $2 per additional hour, $20 daily cap, invalid hours rejected).
- `Week09QualityLab.Tests`: MSTest tests for both classes, written in the Arrange–Act–Assert pattern.
- `PipelineQualityNotes.md` and an example GitHub Actions workflow describing how the tests act as a quality gate.

## What TDD changed about how I worked
Writing the failing test first forced me to decide exactly what "correct" meant before writing any code. The discount example made this obvious: the requirement says the final price must never be negative, but a percentage discount can never make a non-negative price negative. The only way a negative result can happen is a negative *input*, so the real rule became "reject negative original prices". I would probably have missed that if I had started with the implementation.

The Red–Green–Refactor cycle also made the refactor step feel safe. Replacing the chain of `if` statements with a `switch` expression was a small change, but having four passing tests meant I could check it in seconds.

## Design decisions and things I would still question
- **Unknown customer types** are treated like Regular (no discount). That is what the lab implementation does, so I added a test to record it, but in a real system I would probably throw an exception instead so a typo like `"Vip"` does not silently give the wrong price.
- **Rounding:** the requirements do not say how to round money, so I did not test it. In a real shop I would ask for a rule (for example, round to 2 decimal places) and add tests for it.
- **Parking hours:** I assumed the input is whole, already-rounded hours from 1 to 24 for a single day. Zero, negative, and more than 24 hours are invalid. Charging part-hours and multi-day stays would be new requirements and need new tests.
- **Boundary tests** mattered most for parking. Testing 8, 9 and 10 hours shows exactly where the $20 cap starts (9 hours is 4 + 8 × 2 = 20), which a single "large number of hours" test would not.

## Using GitHub Copilot responsibly
Whatever a tool suggests, I am still responsible for it. The checks I applied to any suggested test or code were:
1. Does the test match the written requirement, not just what the code happens to do?
2. Is the expected value correct? I work it out by hand (for example 4 + 2 × 2 = 8) instead of copying the code's answer.
3. Are boundary cases covered (0, 1, cap boundary, just above / just below)?
4. Does the assertion check behaviour, not implementation details?
5. Is the code more complicated than it needs to be?

**TODO before submitting — add your own real example here:** one Copilot suggestion you accepted or rejected, the exact prompt you used, and why (for example, a generated test with a wrong expected value, or an edge case Copilot suggested that you had not thought of).

## What I learned about QA in CI/CD
Passing tests on my own computer are only a starting point. The value comes from running them automatically on every push, on a clean machine, and making a red result block the merge. The tests then act as regression tests, catching accidental changes to existing rules, while quality gates (build, tests, coverage, vulnerability scan, secret detection) cover risks that unit tests alone do not. I also learned that a flaky test is a pipeline problem, because if people stop trusting a red build the whole safety net stops working.

## What I would do next
- Add a rule for unknown customer types and decide on money rounding.
- Turn on branch protection so the CI check is required before merging.
- Add coverage reporting to pull requests and raise the threshold over time.

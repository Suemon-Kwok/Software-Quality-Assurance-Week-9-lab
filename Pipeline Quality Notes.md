# Pipeline Quality Notes — Week09QualityLab

Project: `Week09QualityLab` (C# / .NET, MSTest) — `DiscountCalculator` and `ParkingFeeCalculator`.
An example GitHub Actions workflow that implements these notes is in `.github/workflows/ci.yml`.

---

## Task 1: The CI pipeline for this project

When a developer pushes code (or opens a pull request) on GitHub, the pipeline should run automatically, without anyone remembering to do it:

1. **Checkout** – the runner gets a clean copy of the exact commit that was pushed.
2. **Restore dependencies** – `dotnet restore` downloads the NuGet packages (MSTest, Test SDK, coverlet). A clean machine proves the project does not depend on anything only present on one laptop.
3. **Build the solution** – `dotnet build --no-restore -c Release`. If it does not compile, nothing else is worth running.
4. **Run the MSTest tests** – `dotnet test --no-build` runs every test in `Week09QualityLab.Tests` (discount calculator and parking fee calculator).
5. **Collect test results** – the tests write a `.trx` result file and a coverage file, which are uploaded as pipeline artefacts so the team can inspect them later.
6. **Report pass or fail** – the pipeline shows a green tick or red cross on the commit / pull request, with a link to the failing test names and messages.

**Why failed tests must stop a merge or release:** a failing test means the code no longer does what the team agreed it should do (for example, a VIP customer no longer gets 20% off). If the change merges anyway, the bug reaches `main`, other developers build on top of it, and it may reach customers, where it is far more expensive to find and fix. Blocking the merge keeps `main` always in a releasable state and makes the tests a real safety net, not a suggestion.

---

## Task 2: Pipeline stages

| Stage | What it does | Quality risk it reduces |
|---|---|---|
| Source code checkout | Fetches the exact commit being tested. | Prevents testing the wrong or a stale version of the code, so results are traceable to one commit. |
| Dependency restore | Downloads NuGet packages from the feed. | Catches missing or broken package references and "works on my machine" setups. |
| Build verification | Compiles the whole solution in Release mode. | Catches compile errors, missing files and broken project references before any testing time is wasted. |
| Unit test execution | Runs all MSTest tests. | Catches broken business rules (discounts, parking fees) as soon as they are introduced. |
| Test result reporting | Publishes `.trx` results and a summary. | Makes failures easy to diagnose and gives an auditable history, so nobody has to guess why a build is red. |
| Code coverage measurement | Measures how much code the tests execute (coverlet). | Reveals untested code, such as a rule with no test, where regressions could slip in unnoticed. |
| Static code analysis | Runs .NET analyzers / a tool such as SonarCloud. | Finds bugs, code smells and security problems (null handling, unused code, injection risks) that tests do not exercise. |
| Dependency vulnerability scanning | `dotnet list package --vulnerable --include-transitive` or Dependabot. | Prevents shipping known-vulnerable NuGet packages, including ones pulled in indirectly. |
| Artefact creation | `dotnet publish` output / NuGet package stored as a pipeline artefact. | Ensures what gets deployed is the exact, tested build, not something rebuilt separately by hand. |
| Deployment to test environment (optional) | Deploys the artefact to a test/staging environment. | Catches configuration and environment problems, and lets people try the change before production. |

---

## Task 3: Quality gates

A quality gate is a rule that must be met before a change can move forward. Proposed gates for this project:

1. **The solution must build successfully (zero build errors).** Code that does not compile cannot be tested or released.
2. **All MSTest unit tests must pass (100%).** Any failure means an agreed behaviour is broken.
3. **Test failures block merge.** Branch protection on `main` requires the CI check to be green, so a red build cannot be merged by accident or under time pressure.
4. **Code coverage must be at least 80% line coverage on `Week09QualityLab.Core`, and new code must not lower it.** The project is small and every rule has tests, so 80% is realistic. The gate stops untested code slipping in.
5. **No critical or high severity vulnerabilities in NuGet dependencies.** Prevents known, exploitable weaknesses from being shipped.
6. **No secrets (API keys, passwords, connection strings) may be detected in the repository.** A leaked secret must be treated as compromised, so it is much cheaper to block it before it is committed to history.
7. **Static analysis must report no blocker/critical issues.** Keeps serious defects and security problems out of the codebase.
8. **At least one reviewer must approve the pull request.** A second person can spot requirement mistakes that automated tests cannot, such as a wrong expected value in a test.

---

## Task 4: Regression testing in the pipeline

A regression is when a change breaks behaviour that used to work. Every test written in this lab is a **regression test**: once it passes, it keeps guarding that behaviour every time the pipeline runs.

**Discount calculator example.** Suppose a developer later adds a "Student" customer type and edits the `switch` expression. By mistake they change `"Premium" => 0.10m` to `"Premium" => 0.01m`, or they insert the new case so that VIP falls through to the default. Nothing crashes and the code still compiles, but customers are now charged the wrong price. `CalculateFinalPrice_ShouldApplyTenPercentDiscount_ForPremiumCustomer` (expects 90) would fail immediately.

**Parking fee calculator example.** Suppose someone changes the daily cap from 20 to 25, or changes `Math.Min` to a different formula while "simplifying". The boundary tests for 8, 9 and 10 hours would fail: the 9-hour test proves the exact point where the cap starts, and the 10/12/24-hour tests prove it holds.

**How the pipeline detects this early.** The developer pushes the change, the pipeline builds it and runs the whole test suite automatically, and the pull request is marked red with the name of the failing test and the expected/actual values. The problem is found within minutes, on the developer's own branch, while the change is still fresh in their mind, instead of in production days later.

---

## Task 5: Possible pipeline failures and the team response

| Failure | Likely cause | Action the team should take |
|---|---|---|
| **Build failure** | Syntax error, missing reference, package version conflict, or a file not committed. | Read the build log, fix the compile error on the branch, push again. Do not merge until the build is green. |
| **Failing unit test** | A real regression, or a test with a wrong expected value after a legitimate requirement change. | Decide which is wrong: the code or the test. Fix the code if it is a regression. If the requirement changed, update the test *and* record why in the pull request. Never delete or disable the test just to get a green build. |
| **Low code coverage** | New code was added without tests, or tests were removed. | Write tests for the uncovered branches (guided by the coverage report), then re-run. Do not lower the threshold to make the gate pass. |
| **Detected secret** | A key or password was committed. | Treat the secret as compromised: revoke and rotate it immediately, remove it from the code (and history if needed), move it to a secret store (GitHub Secrets / environment variables), and re-run the scan. |
| **Vulnerable NuGet package** | A dependency has a published CVE. | Upgrade to a patched version, or replace the package. If no fix exists, document the risk, assess whether the vulnerable code is used, and agree a deadline. Re-run the scan. |
| **Static analysis warning / blocker** | Possible null reference, unused code, or a security hotspot. | Fix blockers before merge. For warnings, fix them or explicitly justify and suppress them with a comment so the decision is visible. |
| **Unstable (flaky) test result** | Test depends on time, randomness, ordering or shared state. | Quarantine and fix the root cause quickly (see Task 6). Do not solve it by simply re-running until it passes. |

---

## Task 6: Test data and test reliability

Good test data should be:

- **Repeatable:** the same input always gives the same result, no matter when or how often the test runs. `CalculateFee(9)` must always be 20.
- **Isolated:** each test creates its own data and objects (Arrange) and does not depend on another test running first or leave anything behind. In this lab every test creates its own calculator, so test order does not matter.
- **Safe:** tests should use made-up data, never real customer details, real payment information or production databases, and should not have side effects such as sending emails or charging cards.

**Example of poor test data from this project.** Imagine a parking test that reads the *current time* to work out how long the car has been parked, for example `DateTime.Now - entryTime`. The test would pass at 9:00 in the morning, but if the test runs slowly or across a boundary (say 8 hours 59 minutes 59 seconds becoming 9 hours) the fee could flip between 18 and 20. Another poor example is a shared static list of prices that one test modifies and another test reads. In both cases the result depends on the clock or on test order, not on the code being tested. The tests in this lab avoid this by passing whole hours in as plain parameters.

**What a flaky test is.** A flaky test sometimes passes and sometimes fails when run against exactly the same code. Common causes are timing, random data, shared state, test order, or external services.

**Why flaky tests hurt trust in CI/CD.** When a red build might be a "false alarm", developers start re-running until it turns green and ignoring failures. Then a real failure is dismissed as "just that flaky test" and a genuine regression reaches production. The pipeline stops being a reliable signal, so flaky tests should be fixed or quarantined as soon as they are found.

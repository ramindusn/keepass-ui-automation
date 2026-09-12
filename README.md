# KeePass UI automation

A desktop UI test framework built with FlaUI and NUnit, driving [KeePass 2.x](https://keepass.info/)
as the application under test. The point of the repository is the framework: how screens, tests,
settings and evidence are organised so the suite stays readable as it grows.

KeePass is not mine and is not modified here. It is fetched at a pinned version by a script that
checks the download against the SHA-256 KeePass publishes, and it is never committed.

## How it fits together

![Architecture](docs/architecture.png)

- **Scenarios** are the tests, one class per window. A test creates the screens it uses in
  `BeforeEach` and then lists every click and typed field. Nothing is hidden between a test line
  and the click it names.
- **Screens** are one class per window or dialog. Each finds its own window by title
  when an action is called, declares its controls, and has one method per action. A screen never
  drives another screen. This is the only place an automation id may appear.
- **Setup** is three files: `BaseTest` starts and stops KeePass, `TestConfig` holds every setting
  in one place, `Evidence` records the video and captures a failure.
- **TestData** builds a database of the test's own through the UI, in a temporary folder.
- **Framework** knows nothing about KeePass: scoped screen objects, controls found on use, one
  place that waits, and the session that launches an app.

The rule that keeps it honest: a test may name visible text, such as a button caption or a dialog
title, but never an automation id.

```bash
grep -r "ByAutomationId" tests/     # returns nothing
```

## Run it

Requires **Windows**: FlaUI drives the Windows UI Automation API. The projects also compile on
macOS and Linux, so the build and style check can run anywhere, but the tests cannot.

You need the .NET SDK pinned in `global.json` and PowerShell 7 (`pwsh`).

```bash
pwsh tools/Get-KeePass.ps1     # fetch the pinned KeePass into .keepass/ (verified)
pwsh tools/Get-FFmpeg.ps1      # fetch ffmpeg for the videos (required on CI, optional locally)
dotnet test                    # the whole suite: a fresh KeePass per test, about a minute
```

Run a subset:

```bash
dotnet test --filter Category=Smoke                   # the app starts and its core path works
dotnet test --filter FullyQualifiedName~EntryTests    # one window's tests
```

Set `KEEPASS_EXE` to the path of any `KeePass.exe` to test an installed copy instead of the
fetched one.

### What a run leaves behind

Every test writes an Allure result, every test is recorded on video, and a failed test also
attaches a screenshot and the UIA tree of the window as it was at the moment of failure. To open
the report locally:

```bash
npm install --global allure-commandline
allure serve tests/KeePassAutomation.Tests/bin/Debug/net8.0-windows/allure-results
```

On CI the report for `main` is published after every run:
[ramindusn.github.io/keepass-ui-automation](https://ramindusn.github.io/keepass-ui-automation/).

Each test carries the requirement it verifies, so the report shows it in two places: the
**Behaviors** view groups the tests under their requirements, which is the traceability matrix,
and a test's own entry in the report states the requirement id and wording above the steps, the video and, on
failure, the screenshot and UIA tree.

### Settings

Everything a run can be tuned with lives in `tests/KeePassAutomation.Tests/Setup/TestConfig.cs`:
the output folder, the timeouts, whether to record video, what to capture on failure. Change a
value there and nothing else needs to change.

## Add a test

1. **Find or add the screen.** If the window already has a class under `Screens/`, add the action
   you need as one method. If not, add a class: it takes the session, finds its window by title in
   `Locate()`, declares its controls with `ById(...)`, and has one method per action. The id comes
   from the `DumpUiaTree` diagnostic test (`dotnet test --filter Name=DumpUiaTree`) or from
   Accessibility Insights.

2. **Write the test.** Derive from `BaseTest`, create your screens in `BeforeEach`, and write the
   steps. If the test needs a database, create `DatabaseTestData` in `BeforeEach` and delete it in
   `AfterEach`.

   ```csharp
   public class SearchTests : BaseTest
   {
       private MainWindow _mainWindow;
       private FindDialog _findDialog;
       private DatabaseTestData _database;

       [SetUp]
       public void BeforeEach()
       {
           _mainWindow = new MainWindow(Session);
           _findDialog = new FindDialog(Session);

           _database = new DatabaseTestData(Session);
           _database.CreateSavedDatabase();
       }

       [TearDown]
       public void AfterEach()
       {
           _database.Delete();
       }

       [Test]
       [Requirement("REQ-005", "Searching by title finds the matching entry")]
       public void SearchFindsTheMatchingEntry()
       {
           _mainWindow.ClickMenuItem("Find", "Find...");
           _findDialog.SetSearchText("#2");
           _findDialog.ClickOk();
           _mainWindow.WaitForEntryRow("Sample Entry #2");

           Assert.That(_mainWindow.EntryTitles, Is.EquivalentTo(new[] { "Sample Entry #2" }));
       }
   }
   ```

3. **Name the requirement.** Add it to `requirements.json` and put `[Requirement("REQ-00x", "...")]`
   on the test. CI fails if a requirement has no test. Add `[Category("Smoke")]` if the test
   belongs in the quick subset.

4. **Check style and open a pull request.** `dotnet format --verify-no-changes` must pass; the
   same rules are build errors. Create an issue for the task first; the branch is `kp-<n>-<slug>`
   and every commit and the PR title read `type(KP-<n>): description`, where `<n>` is the issue
   number. CI runs the style check on Linux, then the suite on Windows.

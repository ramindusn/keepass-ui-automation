# Automation IDs

The reference the screen objects are built from. No locator goes into
`src/KeePassAutomation.Library/Screens/` unless it is listed here.

## Where the IDs come from

KeePass is WinForms, where a control's UI Automation `AutomationId` is its `Name`, set in the
form's designer file. The names below come from the **KeePass 2.61.1 source package** (checked
against the SHA-256 published on keepass.info), with the designer file given for each form.

A name in the source only proves the control exists. Whether Windows UI Automation actually
exposes it is confirmed on a running copy — the **Confirmed** column.

## The rule found by inspection

Inspecting a running KeePass 2.61.1 with Accessibility Insights for Windows (2026-09-11):

- **Real controls expose their name as the AutomationId** — the group tree, entry list, toolbar
  strip and menu bar all did.
- **Items drawn inside a toolbar or menu don't.** Toolbar buttons and menu items have *no*
  AutomationId. They do have a `Name` (their visible text), and support `Invoke` (buttons) or
  `ExpandCollapse` (menus), so FlaUI can operate them without the mouse.
- So a toolbar button or menu item is found **by control type and name, searched only inside
  its container**, and the container is found by its AutomationId.
- A `Name` is visible text and changes with KeePass's UI language. Tests run KeePass in its
  default English.
- **Double-clicking an entry row copies a column.** A click lands in the middle of the row, on
  whichever column sits there, so a row is selected with a click and opened with Enter instead.
- **Items in the Entry menu report stale names.** KeePass rebuilds that menu as it opens, and its
  items keep the names they had before, so `Add Entry...` is not findable by name — the run's tree
  dump showed `Perform Auto-Type` twice in its place. The toolbar button `Add Entry` is used instead.

## Main window — `Forms/MainForm.Designer.cs`

| Control | Locator | Confirmed |
|---|---|---|
| Group tree | AutomationId `m_tvGroups` | ✅ |
| Entry list | AutomationId `m_lvEntries` | ✅ |
| Toolbar | AutomationId `m_toolMain` | ✅ |
| Menu bar | AutomationId `m_menuMain` | ✅ |
| Toolbar button "Open Database" | no AutomationId → Button named `Open Database` inside `m_toolMain` | ✅ |
| Menu "File" | no AutomationId → MenuItem named `File` inside `m_menuMain` (access key Alt+F) | ✅ |
| Entry preview (bottom) | AutomationId `m_richEntryView` | ☐ |
| A group in the tree | no AutomationId → TreeItem named like the group, inside `m_tvGroups` | ☐ |
| An entry row | no AutomationId → ListItem named by its Title column, inside `m_lvEntries` | ☐ |

Other names seen in the inspector, found the same way as their confirmed neighbours:

- **Toolbar buttons** (inside `m_toolMain`): `New Database`, `Open Database`, `Save Database`,
  `Add Entry`, `Copy User Name`, `Copy Password`, `Open URL(s)`, `Copy URL(s)`,
  `Perform Auto-Type`, `Find`, `Find Entries`, `Lock Workspace`, and a quick-search combo box.
- **Menus** (inside `m_menuMain`): `File`, `Group`, `Entry`, `Find`, `View`, `Tools`, `Help`.
  Their sub-items only appear in the tree once a menu is expanded.

## Dialogs — from the source, confirmed by their own tests

A dialog only exists in the UI Automation tree while it is open, and most need a database,
which arrives with KP-9. Each dialog's names are confirmed when the task that uses it first
runs its tests. A wrong name fails with the dialog's full UIA tree in the error message.

| Dialog | Designer file | Names the tests will use | Task |
|---|---|---|---|
| New database: save | Windows | title `Create New Database`; file name box ID `1001` | KP-9 |
| New database: master key | `KeyCreationForm` | title `Create Master Key`; `m_tbPassword`, `m_tbRepeatPassword`, `m_btnOK` | KP-9 |
| New database: settings | `DatabaseSettingsForm` | title `Configure New Database`; `m_btnOK` | KP-9 |
| Open database: file | Windows | title `Open Database File`; file name box ID `1148` | KP-9 |
| Open database: master key | `KeyPromptForm` | title `Open Database - <file>`; `m_tbPassword`, `m_btnOK`, `m_btnCancel` | KP-9 |
| Entry | `PwEntryForm` | tab control `m_tabMain` with **five** tabs: `m_tabEntry`, `m_tabAdvanced`, `m_tabProperties`, `m_tabAutoType`, `m_tabHistory`; fields `m_tbTitle`, `m_tbUserName`, `m_tbPassword`, `m_tbRepeatPassword`, `m_tbUrl`, `m_rtNotes`; history list `m_lvHistory` (fixed "Dialog (Unsaved)" and "Current" rows, then one row per earlier version); `m_btnHistoryView` opens `View Entry (Read-Only)`; `m_btnOK`. Titled `Add Entry` / `Edit Entry`; tabs are captioned `General` … `History` | KP-11 |
| Find | `SearchForm` | opened through menu `Find` → `Find...`; titled `Find`; search text `m_tbSearch`, `m_cbTitle`, `m_btnOK`; results replace the entry list, and the group selection is unchanged | KP-12 |
| Options | `OptionsForm` | tab control `m_tabMain` with `m_tabSecurity`, `m_tabPolicy`, `m_tabGui1`, `m_tabGui2`, `m_tabIntegration`, `m_tabAdvanced` | none yet |

Two things to expect when these are confirmed:

- **Password fields** (`m_tbPassword`, `m_tbRepeatPassword`) are KeePass's own secure text box.
  UI Automation lets a test type into a password field but deliberately won't read its value back.
- **Tabs** may appear in the UIA tree as tab items named by their caption rather than by the
  tab page's name — the same situation as toolbar buttons.

## Names are only unique within one window

`m_btnOK`, `m_btnCancel` and `m_tabMain` appear in almost every dialog. That is why every screen
object searches only inside its own window or panel rather than across the whole desktop.

## Checking again

- **Inspector:** Accessibility Insights for Windows → Live Inspect. Add `AutomationId` via the
  gear icon next to *Properties*, hover a control, and pause (⏸). Items inside toolbars and
  menus are easiest to reach by selecting the container, pausing, and clicking the item in the
  tree on the left.
- **Test:** `dotnet test --filter Name=DumpUiaTree` writes the main window's full tree to
  `artifacts/main-window.tree.txt`.
- **Source:** in the source package, every name is a `.Name = "m_…"` line in
  `KeePass/Forms/<Form>.Designer.cs`.

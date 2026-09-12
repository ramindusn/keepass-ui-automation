using System;
using System.IO;
using KeePassAutomation.Framework.AppUnderTest;
using KeePassAutomation.Screens;
using KeePassAutomation.Screens.Dialogs;

namespace KeePassAutomation.Tests.TestData
{
    // A database of the test's own, created through the UI in a temporary folder; no test data is
    // committed. A test creates it in BeforeEach and deletes it in AfterEach.
    public sealed class DatabaseTestData
    {
        // KeePass warns, and waits for Yes/No, on master passwords it rates at 79 bits or less.
        public const string MasterPassword = "V9q!Lm2#Xr7tZ-pK4w@Nj8sH3&dF6yB";

        private readonly AppSession _session;
        private readonly string _folder;

        public DatabaseTestData(AppSession session)
        {
            _session = session;
            _folder = Directory.CreateTempSubdirectory("keepass-test-").FullName;
        }

        public string Path
        {
            get { return System.IO.Path.Combine(_folder, "test.kdbx"); }
        }

        // KeePass names a new database's top group after the file.
        public string TopGroupName
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
        }

        // Every step is written out: toolbar, Windows save dialog, master key, settings, then save.
        // Saved, so closing KeePass afterwards doesn't stop at a "save changes?" prompt.
        public void CreateSavedDatabase()
        {
            var mainWindow = new MainWindow(_session);
            var fileDialog = new FileDialog(_session);
            var masterKeyDialog = new CreateMasterKeyDialog(_session);
            var settingsDialog = new DatabaseSettingsDialog(_session);

            mainWindow.ClickToolbarButton("New Database");
            fileDialog.TypeFileName(Path);
            fileDialog.PressEnter();
            masterKeyDialog.TypePassword(MasterPassword);
            masterKeyDialog.TypeRepeatPassword(MasterPassword);
            masterKeyDialog.ClickOk();
            settingsDialog.ClickOk();
            mainWindow.WaitForDatabaseToOpen();

            mainWindow.ClickToolbarButton("Save Database");
            mainWindow.WaitForDatabaseToBeSaved();
        }

        // Best effort: a leftover temporary folder must not fail the test.
        public void Delete()
        {
            try
            {
                Directory.Delete(_folder, true);
            }
            catch (IOException)
            {
                // Left behind; the operating system cleans its temp folder.
            }
            catch (UnauthorizedAccessException)
            {
                // KeePass may still hold the file for a moment after closing.
            }
        }
    }
}

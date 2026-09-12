using System;
using System.IO;
using NUnit.Framework;

namespace KeePassAutomation.Tests.Setup
{
    // For tests that need a database: each creates its own in a temporary folder; no test data is committed.
    public abstract class DatabaseTestBase : KeePassTestBase
    {
        // KeePass warns, and waits for Yes/No, on master passwords it rates at 79 bits or less.
        protected const string MasterPassword = "V9q!Lm2#Xr7tZ-pK4w@Nj8sH3&dF6yB";

        private string _folder;

        protected string DatabasePath
        {
            get { return Path.Combine(_folder, "test.kdbx"); }
        }

        // KeePass names a new database's top group after the file.
        protected string TopGroupName
        {
            get { return Path.GetFileNameWithoutExtension(DatabasePath); }
        }

        [SetUp]
        public void CreateFolder()
        {
            _folder = Directory.CreateTempSubdirectory("keepass-test-").FullName;
        }

        [TearDown]
        public void DeleteFolder()
        {
            try
            {
                Directory.Delete(_folder, true);
            }
            catch (IOException)
            {
                // Best effort: a leftover temporary folder must not fail the test.
            }
            catch (UnauthorizedAccessException)
            {
                // Same: KeePass may still hold the file for a moment after closing.
            }
        }

        // Saved, so closing KeePass afterwards doesn't stop at a "save changes?" prompt.
        protected void CreateSavedDatabase()
        {
            MainWindow.CreateDatabase(DatabasePath, MasterPassword);
            MainWindow.SaveDatabase();
        }
    }
}

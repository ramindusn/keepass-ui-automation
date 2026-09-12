using NUnit.Framework;

// One desktop, one KeePass: the tests cannot run side by side. NUnit's default is serial too, but
// the constraint is declared here rather than assumed.
[assembly: NonParallelizable]

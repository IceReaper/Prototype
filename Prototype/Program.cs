using Prototype;

// TODO this is a hack around a stride bug!
// TODO If the runtime db files are not deleted, the whole app will only render on the first start!

// TODO under windows-x64 the data dir is used, so we clear all generated stuff.
File.WriteAllText("data/db/index", "");

foreach (var directory in Directory.GetDirectories("data/db")
	         .Select(static e => e.Replace("\\", "/"))
	         .Where(static directory => directory is not ("data/db/tmp" or "data/db/bundles")))
	Directory.Delete(directory, true);

// TODO under osx-arm64 however the runtime generated db files are in another location
if (Directory.Exists("local"))
	Directory.Delete("local", true);

using var prototype = new PrototypeGame();
prototype.Run();

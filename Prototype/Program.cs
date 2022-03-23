using Prototype;

// TODO this is a hack around a stride bug!
File.WriteAllText("data/db/index", "");

foreach (var directory in Directory.GetDirectories("data/db").Select(e => e.Replace("\\", "/")))
{
	if (directory is not ("data/db/tmp" or "data/db/bundles"))
		Directory.Delete(directory, true);
}

using var prototype = new PrototypeGame();
prototype.Run();

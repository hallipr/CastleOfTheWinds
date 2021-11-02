<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

int index = 0;
string outPath = @"d:\castle\test\something";

async Task Main()
{
	// watch this file. when it changes, copy the new version to a unique file name in outPath
	var watcher = new FileSystemWatcher(@"d:\castle", "temp.cwg");
	watcher.Changed += FileChanged;
	watcher.EnableRaisingEvents = true;

	try
	{
		Directory.Delete(outPath, true);
	}
	catch {}
	
	Directory.CreateDirectory(outPath);
	
	await Task.Delay(TimeSpan.FromMilliseconds(int.MaxValue).Dump());	
}

void FileChanged(object sender, FileSystemEventArgs e)
{
	var fileNumber = Interlocked.Increment(ref index);
	var fileName = $@"{outPath}\{fileNumber}.cwg";
	File.Copy(e.FullPath, fileName, true);
	fileName.Dump();
}

// You can define other methods, fields, classes and namespaces here

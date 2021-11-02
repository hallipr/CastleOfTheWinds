<Query Kind="Program">
  <Namespace>GregsStack.InputSimulatorStandard</Namespace>
  <Namespace>GregsStack.InputSimulatorStandard.Native</Namespace>
  <Namespace>PInvoke</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Input</Namespace>
</Query>

#load ".\Common"

// continually:
// - loads a save game
// - enters and exists each shop to generate unique inventory
// - saves the game.
// press num lock to stop

const string sourceFilePath = @"D:\castle\saves\new-2.cwg";
const string tempFilePath = @"D:\castle\saves\temp.cwg";

static readonly string testName = @"stores";
static readonly string outFilePath = @$"D:\castle\test\{testName}";

private readonly int start = 0;
private readonly int count = 1000;

const int longDelay = 60;
const int shortDelay = 20;

const VirtualKeyCode Up = VirtualKeyCode.UP;
const VirtualKeyCode Left = VirtualKeyCode.LEFT;
const VirtualKeyCode Down = VirtualKeyCode.DOWN;
const VirtualKeyCode Right = VirtualKeyCode.RIGHT;

private readonly InputSimulator input = new();

private CastleProcess _process;

// currently set up to enter stores in castle2 village, with the
// character starting in front of the top, left store
public List<Store> Stores = new()
{
	new Store("a", 0, 0, Up),
	new Store("b", 3, 0, Up),
	new Store("c", 6, 0, Up),
	new Store("d", 9, 0, Up),
	new Store("e", 12, 0, Up),
	new Store("f", 12, 3, Down),
	new Store("g", 9, 3, Down),
	new Store("h", 6, 3, Down),
	new Store("i", 3, 3, Down),
	new Store("j", 0, 3, Down),
};

[STAThread]
void Main()
{
	var progress = new Util.ProgressBar().Dump();

	Directory.CreateDirectory(outFilePath);

	var missingFiles = Enumerable.Range(start, count)
	.Select(x => @$"{outFilePath}\{x}.cwg")
	.Where(x => !File.Exists(x))
	.ToArray();

	var stopWatch = Stopwatch.StartNew();
	for (var i = 0; i < missingFiles.Length; i++)
	{
		input.ThrowIfNumLock();

		if (_process == null)
		{
			_process = CastleProcess.StartCastleProcess(2, 0, 0, 800, 600);
		}	
		
		_process.ClickCastleWindow();
		
		Thread.Sleep(longDelay);

		try
		{
			File.Copy(sourceFilePath, tempFilePath, true);
					
			RunGameActions(tempFilePath);
			
			double complete = i + 1;			
			var rate = stopWatch.Elapsed / complete;
			var remaining = rate * (count - complete);
			progress.Fraction = complete / count;
			progress.Caption = $"{complete} / {count} - {(int)remaining.TotalMinutes}:{remaining:ss}";

			File.Move(tempFilePath, missingFiles[i], true);
		}
		catch(Exception ex)
		{
			Thread.Sleep(1000);
			_process?.Kill();
			_process = null;
			ex.Dump();
			Thread.Sleep(1000);
			i--;
		}
	}
}

void RunGameActions(string tempFilePath)
{
	_process.LoadSaveFile(tempFilePath);

	var coordinates = (X:0,Y:0);
	foreach (var store in Stores)
	{
		while(store.X < coordinates.X)
		{
			_process.SendKeys(Left);
			coordinates = (coordinates.X - 1, coordinates.Y);
		}
		
		while (store.X > coordinates.X)
		{
			_process.SendKeys(Right);
			coordinates = (coordinates.X + 1, coordinates.Y);
		}
		
		while (store.Y < coordinates.Y)
		{
			_process.SendKeys(Up);
			coordinates = (coordinates.X, coordinates.Y - 1);
		}

		while (store.Y > coordinates.Y)
		{
			_process.SendKeys(Down);
			coordinates = (coordinates.X, coordinates.Y + 1);
		}

		EnterExitStore(store.EnterKey);
	}

	input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_F);

	_process.SendKeys(VirtualKeyCode.VK_S);
	
	Thread.Sleep(longDelay);
}

void EnterExitStore(params VirtualKeyCode[] directions)
{
	_process.SendKeys(directions);

	var stopwatch = Stopwatch.StartNew();
	
	while (!_process.WindowTitle.Contains("Weight"))
	{
		if (stopwatch.ElapsedMilliseconds > 2000)
		{
			throw new TimeoutException();
		}
		Thread.Yield();
	}

	_process.SendKeys(VirtualKeyCode.ESCAPE);

	stopwatch.Restart();
	while (_process.WindowTitle.Contains("Weight"))
	{
		if (stopwatch.ElapsedMilliseconds > 2000)
		{
			throw new TimeoutException();
		}
		Thread.Yield();
	}
}

public class Store
{
	public Store(string name, int x, int y, VirtualKeyCode key)
	{
		Name = name;
		X = x;
		Y = y;
		EnterKey = key;
	}

	public string Name { get; }
	public int X { get; }
	public int Y { get; }
	public VirtualKeyCode EnterKey { get; }
}

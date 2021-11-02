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
// - clicks a spell button,
// - clicks a point,
// - saves the game.

const string saveFilePath = @"D:\castle\saves\cret-2.cwg";
const string tempFilePath = @"D:\castle\saves\temp.cwg";

static readonly string testName = @"cretures";
static readonly string outFilePath = @$"D:\castle\test\{testName}";

private readonly int startNumber = 0;
private readonly int count = 1000;

const int longDelay = 60;
const int shortDelay = 20;

const VirtualKeyCode Up = VirtualKeyCode.UP;
const VirtualKeyCode Left = VirtualKeyCode.LEFT;
const VirtualKeyCode Down = VirtualKeyCode.DOWN;
const VirtualKeyCode Right = VirtualKeyCode.RIGHT;

private readonly InputSimulator input = new();

private CastleProcess _process;

[STAThread]
void Main()
{
	var progress = new Util.ProgressBar().Dump();

	Directory.CreateDirectory(outFilePath);
	
	var missingFiles = Enumerable.Range(startNumber, count)
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
			File.Copy(saveFilePath, tempFilePath, true);

			RunGameActions(tempFilePath);

			var total = missingFiles.Length;
			
			double complete = i + 1;			
			var rate = stopWatch.Elapsed / complete;
			var remaining = rate * (total - complete);
			progress.Fraction = complete / total;
			progress.Caption = $"{complete} / {total} - {(int)remaining.TotalMinutes}:{remaining:ss}";

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
	
	_process.SetCursorPosition(386, 62);
	_process.LeftButtonClick();

	_process.SetCursorPosition(372, 292);	
	_process.LeftButtonClick();

	input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_F);

	_process.SendKeys(VirtualKeyCode.VK_S);
	
	Thread.Sleep(longDelay);
}


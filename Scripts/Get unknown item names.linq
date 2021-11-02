<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Input</Namespace>
  <Namespace>System.Dynamic</Namespace>
</Query>

#load ".\Common"

const string dataPath = @"D:\castle\inv";
const int maxAttempts = 4;
private ItemNamesFile _itemNames;
private CastleProcess _process;
private InputSimulator _input;

// Use the Rename Item window to copy the item names for unknown items.
// This loads all of the saves under dataPath and finds names for items we haven't indexed.
// This uses a hash of a screen shot of the the item name to find unique, unknown names.

[STAThread]
void Main()
{
	_input = new();
	_itemNames = new ItemNamesFile($@"{dataPath}\itemNames.jsonl", $@"{dataPath}\textNames.jsonl");
	
	var saveFiles = Directory.EnumerateFiles(dataPath, "*.cwg", SearchOption.AllDirectories)
		.ToArray()
		.Select(SaveData.Read)
		.Where(x => x != null)
		.ToArray();

	var items = saveFiles.SelectMany(f => f.Shops.SelectMany(s => s.Items.Select(i => new { Shop = s, Save = f, Item = i }))).ToArray();

	var shopGroups = items
		.Where(x => !_itemNames.ItemShaNames.ContainsKey(x.Item.Sha))
		.GroupBy(x => x.Item.Sha, (key, items) => items.First())
		.OrderByDescending(x => x.Item.Index)
		.GroupBy(x => (x.Save.FilePath, x.Shop.ShopName), (key, items) => items.ToArray())
		.Select(x => new {
			x[0].Save,
			x[0].Shop,
			Items = x.Select(y => y.Item)
		});
		
	foreach (var group in shopGroups)
	{
		_input.ThrowIfNumLock();
		try
		{
			if (_process == null)
			{
				_process = CastleProcess.StartCastleProcess(2, 700, 300, 1024, 768);
				_process.shortDelay = 50;
				_process.longDelay = 120;
			}
			
			var shopLocation = GetShopLocation(group.Shop.ShopName);
			using(var stream = File.OpenWrite(group.Save.FilePath))
			{
				stream.Position = 0x00B4;
				stream.WriteByte((byte)shopLocation.Y);
				stream.WriteByte((byte)shopLocation.X);
			}
					
			_process.LoadSaveFile(group.Save.FilePath);
			EnterShop(group.Shop);

			var inventoryWindow = _process.GetInventoryWindow();
			var shopWindow = _process.GetShopWindow();
			shopWindow.CaptureItems();
			
			foreach (var item in group.Items.OrderByDescending(x => x.Index))
			{
				_input.ThrowIfNumLock();

				if (_itemNames.ItemShaNames.ContainsKey(item.Sha))
				{
					continue;
				}

				var name = GetItemType(group.Shop, item.Index, shopWindow, inventoryWindow);

				_itemNames.AddItemShaName(item.Sha, name);
			}
		}
		catch (Exception ex)
		{
			ex.Dump();
			_process?.Kill();
			_process = null;
		}
	}
}

string GetItemType(ShopData shop, int index, ItemWindow shopWindow, ItemWindow inventoryWindow)
{
	// buy item
	var item = shopWindow.Items.Single(x => x.Index == index);
	if(_itemNames.TextShaNames.TryGetValue(item.Sha, out var name))
	{
		return name;
	}

	var storeX = shopWindow.Rectangle.Left + item.Column * 80 + 40;
	var storeY = shopWindow.Rectangle.Top + item.Row * 80 + 16;

	var destX = inventoryWindow.Rectangle.Right - 100;
	var destY = inventoryWindow.Rectangle.Bottom - 100;

	_process.SetCursorPosition(storeX, storeY);
	_process.LeftButtonClick();
	_process.SendKeys(VirtualKeyCode.RETURN);
	Thread.Sleep(_process.shortDelay);

	_process.SetCursorPosition(destX, destY);
	_process.LeftButtonClick();
	Thread.Sleep(_process.shortDelay);

	_process.SendKeys(VirtualKeyCode.RETURN);

	var newIndex = inventoryWindow.Items.Count;
	inventoryWindow.Items.Add(item);
	
	var itemsPerRow = inventoryWindow.Rectangle.Width / 80;

	var packCol = newIndex % itemsPerRow;
	var packRow = newIndex / itemsPerRow;

	var packX = inventoryWindow.Rectangle.Left + packCol * 80 + 40;
	var packY = inventoryWindow.Rectangle.Top + packRow * 80 + 16;

	// select the item in the pack
	_process.SetCursorPosition(packX, packY);
	_process.LeftButtonClick();
	Thread.Sleep(_process.longDelay);

	// open the rename window
	_process.SendModifiedKey(VirtualKeyCode.MENU, VirtualKeyCode.VK_N);
	Thread.Sleep(_process.longDelay);

	// copy the pre-selected text
	_process.SendModifiedKey(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);

	//close the rename window to get item name
	_process.SendKeys(VirtualKeyCode.ESCAPE);
	Thread.Sleep(_process.longDelay);
	
	name = System.Windows.Clipboard.GetText();
	
	if(name.Contains("Can't rename"))
	{
		name = "CAN'T RENAME";
	}

	_itemNames.AddTextShaName(item.Sha, name);

	return name;
}

//string GetItemName(ItemWindow storeWindow, ItemWindow inventoryWindow, ItemInfo item, ref int purchased)
//{

//}

Point GetShopLocation(string shopName)
{
	switch (shopName)
	{
		case "Thangbrand's Sword and Scabbard":
			return new Point(9, 10);
		case "Sverting's Armor Shop":
			return new Point(5, 10);
		case "Rognvald's Outfitters":
			return new Point(4, 11);
		case "Myrkjartan's Miscellaneous Magic Shop":
			return new Point(9, 13);
		default:
			throw new Exception("bad");
	}
}

VirtualKeyCode GetShopDirection(string shopName)
{
	switch (shopName)
	{
		case "Thangbrand's Sword and Scabbard":
			return VirtualKeyCode.UP;
		case "Sverting's Armor Shop":
			return VirtualKeyCode.UP;
		case "Rognvald's Outfitters":
			return VirtualKeyCode.DOWN;
		case "Myrkjartan's Miscellaneous Magic Shop":
			return VirtualKeyCode.LEFT;
		default:
			throw new Exception("bad");
	}
}

void EnterShop(ShopData shop)
{
	var shopDirection = GetShopDirection(shop.ShopName);
	_process.SendKeys(shopDirection, shopDirection);

	var stopwatch = Stopwatch.StartNew();
	while (!_process.WindowTitle.Contains("Weight"))
	{
		if (stopwatch.ElapsedMilliseconds > 2000)
		{
			throw new TimeoutException();
		}
		
		Thread.Yield();
	}

	Thread.Sleep(_process.longDelay);
}

private class ItemNamesFile
{
	private string _itemNamesPath;
	private string _textNamesPath;
	
	public ItemNamesFile(string itemNamesPath, string textNamesPath)
	{
		_itemNamesPath = itemNamesPath;
		_textNamesPath = textNamesPath;
		
		ItemShaNames = Load(itemNamesPath);
		TextShaNames = Load(textNamesPath);
	}

	public Dictionary<string, string> ItemShaNames { get; }
	public Dictionary<string, string> TextShaNames { get; }

	internal void AddItemShaName(string sha, string name)
	{
		if (ItemShaNames.TryAdd(sha, name))
		{
			File.AppendAllLines(_itemNamesPath, new[] { $"{sha}: {name}" });
		}
	}

	internal void AddTextShaName(string sha, string name)
	{
		if(TextShaNames.TryAdd(sha, name))
		{
			File.AppendAllLines(_textNamesPath, new[] { $"{sha}: {name}" });			
		}
	}

	private Dictionary<string, string> Load(string path)
	{
		if (File.Exists(path))
		{
			return File.ReadAllLines(path)
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Split(':', 2))
				.ToDictionary(x => x[0].Trim(), x => x[1].Trim());
		}

		return new();
	}
}
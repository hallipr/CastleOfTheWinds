<Query Kind="Statements">
  <NuGetReference>GregsStack.InputSimulatorStandard</NuGetReference>
  <NuGetReference>PInvoke.User32</NuGetReference>
  <Namespace>PInvoke</Namespace>
  <Namespace>GregsStack.InputSimulatorStandard.Native</Namespace>
  <Namespace>GregsStack.InputSimulatorStandard</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

#load "Common\Helpers"

public class SaveData
{
	public string FilePath { get; set; }
	
	public byte Strength { get; set; }
	public byte Intelligence { get; set; }
	public byte Constitution { get; set; }
	public byte Dexterity { get; set; }
	public byte StrengthBase { get; set; }
	public byte IntelligenceBase { get; set; }
	public byte ConstitutionBase { get; set; }
	public byte DexterityBase { get; set; }
	public short HitPoints { get; set; }
	public short HitPointsMax { get; set; }
	public short Mana { get; set; }
	public short ManaMax { get; set; }
	public short Level { get; set; }
	public int Experience { get; set; }
	public int ExperienceMax { get; set; }
	public short ArmorClass { get; set; }
	public short ToHit { get; set; }
	public short ToDamage { get; set; }
	public short Speed { get; set; }
	public short SpeedBase { get; set; }
	public short SpeedBurden { get; set; }
	public short GameVersion { get; set; }
	public byte CurrentX { get; set; }
	public byte CurrentY { get; set; }
	public byte PreviousX { get; set; }
	public byte PreviousY { get; set; }
	public StoryProgression Story { get; set; } = new();
	public string PlayerName { get; set; }
	public short ResistFire { get; set; }
	public short ResistCold { get; set; }
	public short ResistLightening { get; set; }
	public short ResistAcid { get; set; }
	public short ResistFear { get; set; }
	public short ResistDrainLife { get; set; }
	public SpellBook LearnedSpells { get; set; }
	public int Bulk { get; set; }
	public int Weight { get; set; }
	public int BulkMax { get; set; }
	public int WeightMax { get; set; }
	
	public HashSet<string> StoreNames { get; set; } = new();
	public List<ShopData> Shops { get; set; } = new();

	public static SaveData Read(string filePath)
	{
		using var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(filePath)));

		var saveData = new SaveData { FilePath = filePath };

		reader.BaseStream.Position = 0x0080;
		saveData.Strength = reader.ReadByte();           //80
		saveData.Intelligence = reader.ReadByte();	     //81
		saveData.Constitution = reader.ReadByte();	     //82
		saveData.Dexterity = reader.ReadByte();		     //83
		saveData.StrengthBase = reader.ReadByte();	     //84
		saveData.IntelligenceBase = reader.ReadByte();   //85
		saveData.ConstitutionBase = reader.ReadByte();   //86
		saveData.DexterityBase = reader.ReadByte();	     //87
														 
		reader.Skip(12);                                 //88-93                             
														 
		saveData.HitPoints = reader.ReadInt16();         //94
		saveData.HitPointsMax = reader.ReadInt16();      //96
		saveData.Mana = reader.ReadInt16();				 //98
		saveData.ManaMax = reader.ReadInt16();			 //9A
		saveData.Level = reader.ReadInt16();			 //9C
		saveData.Experience = reader.ReadInt32();		 //9E
		saveData.ExperienceMax = reader.ReadInt32();	 //A2
		saveData.ArmorClass = reader.ReadInt16();        //A6
		saveData.ToHit = reader.ReadInt16();             //A8
		saveData.ToDamage = reader.ReadInt16();          //AA
		saveData.Speed = reader.ReadInt16();             //AC
		saveData.SpeedBase = reader.ReadInt16();         //AE
		saveData.SpeedBurden = reader.ReadInt16();       //B0
		saveData.GameVersion = reader.ReadInt16();       //B2
		saveData.CurrentY = reader.ReadByte();           //B4
		saveData.CurrentX = reader.ReadByte();           //B5
		saveData.PreviousY = reader.ReadByte();          //B6
		saveData.PreviousX = reader.ReadByte();          //B7

		saveData.Story = StoryProgression.Read(reader);  //C7-D5
		
		reader.BaseStream.Position = 0x100;  
		var playerNameBlock = reader.ReadBytes(80);        //100-149
		for(var i = 0; i < 80; i++)
		{
			if(playerNameBlock[i] == 0)
			{
				saveData.PlayerName = Encoding.ASCII.GetString(playerNameBlock, 0, i);
				break;
			}
		}

		saveData.ResistFire = reader.ReadInt16();         //150
		saveData.ResistCold = reader.ReadInt16();         //152
		saveData.ResistLightening = reader.ReadInt16();   //154
		saveData.ResistAcid = reader.ReadInt16();         //156
		saveData.ResistFear = reader.ReadInt16();         //158
		saveData.ResistDrainLife = reader.ReadInt16();    //15A

		saveData.LearnedSpells = SpellBook.Read(reader);  //1B2-356

		reader.BaseStream.Position = 0x03FE;
		saveData.Weight = reader.ReadInt32(); //3FE
		saveData.Bulk = reader.ReadInt32(); //402
		saveData.WeightMax = reader.ReadInt32(); //406
		saveData.BulkMax = reader.ReadInt32(); //40A
		
		return saveData;
		
		// Sverting's
		reader.BaseStream.Position = 0x06E4;
		if (reader.ReadByte() != 0)
		{
			saveData.StoreNames.Add("Svertings");
		}

		// Thangbrand's
		reader.BaseStream.Position = 0x06EA;
		if (reader.ReadByte() != 0)
		{
			saveData.StoreNames.Add("Svertings");
		}

		// Myrkjartan's
		reader.BaseStream.Position = 0x06F7;
		if (reader.ReadByte() != 0)
		{
			saveData.StoreNames.Add("Svertings");
		}

		// Rognvald's
		reader.BaseStream.Position = 0x0705;
		if (reader.ReadByte() != 0)
		{
			saveData.StoreNames.Add("Svertings");
		}

		var shopCount = saveData.StoreNames.Count;

		// skip to first shop
		reader.BaseStream.Position = 0x072D;

		for (var i = 0; i < shopCount; i++)
		{
			var nameLength = reader.ReadByte();

			var shopName = Encoding.ASCII.GetString(reader.ReadBytes(nameLength));

			// skip unknown
			reader.BaseStream.Seek(36, SeekOrigin.Current);

			var itemCount = reader.ReadInt16();

			// skip 2 unknown bytes + 5 bytes per item (item instance ids?)
			reader.BaseStream.Seek(2 + itemCount * 5, SeekOrigin.Current);

			var shop = new ShopData { ShopName = shopName };

			for (var j = 0; j < itemCount; j++)
			{
				var item = ItemDefinition.Read(reader);
				item.Index = j;
				shop.Items.Add(item);
			}

			saveData.Shops.Add(shop);
		}

		return saveData;
	}
}

public class ShopData
{
	public string ShopName { get; set; }
	public List<ItemDefinition> Items { get; set; } = new();
}

public class ItemDefinition
{
	public byte Type { get; set; }
	public byte SubType { get; set; }
	public int Value { get; set; }
	public ushort NameOffset { get; set; }
	public bool Wield { get; set; }
	public bool Unwield { get; set; }
	public bool Activate { get; set; }
	public bool Use { get; set; }
	public bool DeleteOnActivate { get; set; }
	public bool Charged { get; set; }
	public byte Identify { get; set; }
	public bool Identified { get; set; }
	public byte Enchantment { get; set; }
	public bool Multiple { get; set; }
	public bool HasFixedWeight { get; set; }
	public bool HasFixedBulk { get; set; }
	public bool NoExpand { get; set; }
	public bool ObjectList { get; set; }

	// for non-objectlist
	public byte AttributeCount { get; set; }
	public byte AllocatedAttributeCount { get; set; }
	public byte DescriptionType { get; set; }
	public ItemAttribute[] Attributes { get; set; }
	public long Offset { get; set; }
	public int Index { get; set; }

	// for objectlist
	public short ParentObjectListHandle { get; set; }
	public int Weight { get; set; }
	public int Bulk { get; set; }
	public int WeightMax { get; set; }
	public int BulkMax { get; set; }
	public int WeightFixed { get; set; }
	public int BulkFixed { get; set; }
	public short SlotCount { get; set; }
	public short SlotAllocatedCount { get; set; }
	public Slot[] Slots { get; set; }
	public ItemDefinition[] Objects { get; set; }

	public static ItemDefinition Read(BinaryReader reader)
	{
		var item = new ItemDefinition();

		item.Offset = reader.BaseStream.Position;
		item.Type = reader.ReadByte();
		item.SubType = reader.ReadByte();
		item.Value = reader.ReadInt32();
		item.NameOffset = reader.ReadUInt16();

		var bits = new BitArray(reader.ReadBytes(2));

		item.Wield = bits[0];
		item.Unwield = bits[1];
		item.Activate = bits[2];
		item.Use = bits[3];
		item.DeleteOnActivate = bits[4];
		item.Charged = bits[5];

		var identify = (bits[6] ? 2 : 0) + (bits[7] ? 1 : 0);
		item.Identify = (byte)identify;

		item.Identified = bits[8];

		var enchantment = (bits[9] ? 2 : 0) + (bits[10] ? 1 : 0);
		item.Enchantment = (byte)enchantment;

		item.Multiple = bits[11];
		item.HasFixedWeight = bits[12];
		item.HasFixedBulk = bits[13];
		item.NoExpand = bits[14];
		item.ObjectList = bits[15];

		if (item.ObjectList)
		{
			item.ParentObjectListHandle = reader.ReadInt16();
			item.Weight = reader.ReadInt32();
			item.Bulk = reader.ReadInt32();
			item.WeightMax = reader.ReadInt32();
			item.BulkMax = reader.ReadInt32();
			item.WeightFixed = reader.ReadInt32();
			item.BulkFixed = reader.ReadInt32();
			item.SlotCount = reader.ReadInt16();
			item.SlotAllocatedCount = reader.ReadInt16();

			item.Slots = new Slot[Math.Max(item.SlotAllocatedCount, (byte)1)];

			var itemCount = 0;
			for (var i = 0; i < item.Slots.Length; i++)
			{
				var slot = Slot.Read(reader);
				item.Slots[i] = slot;
				if (slot.ObjectHandle != 0)
				{
					throw new Exception("Cannot process slots with objects");
					itemCount++;
				}
			}

			item.Objects = new ItemDefinition[itemCount];

			for (var i = 0; i < itemCount; i++)
			{
				$"    object list item {i} @ {reader.BaseStream.Position}".Dump();
				item.Objects[i] = ItemDefinition.Read(reader);
			}
		}
		else
		{
			var readByte = reader.ReadByte();

			item.AttributeCount = (byte)((readByte & 0b11110000) >> 4);
			item.AllocatedAttributeCount = (byte)(readByte & 0b00001111);

			readByte = reader.ReadByte();

			item.DescriptionType = (byte)((readByte & 0b11100000) >> 5);

			item.Attributes = new ItemAttribute[Math.Max(item.AllocatedAttributeCount, (byte)1)];

			for (var i = 0; i < item.Attributes.Length; i++)
			{
				item.Attributes[i] = ItemAttribute.Read(reader);
			}
		}

		return item;
	}

	public string Sha => GetHashBytes().ToShaString();

	public byte[] GetHashBytes()
	{
		var data = new List<byte>();

		data.Add(Type);
		data.Add(SubType);
		data.AddRange(BitConverter.GetBytes(SlotCount));
		data.AddRange(BitConverter.GetBytes(SlotAllocatedCount));

		if (Attributes?.Length > 0)
		{
			data.AddRange(Attributes.SelectMany(x => x.GetHashBytes()));
		}

		if (Slots?.Length > 0)
		{
			data.AddRange(Slots.SelectMany(x => x.GetHashBytes()));
		}

		if (Objects?.Length > 0)
		{
			data.AddRange(Objects.SelectMany(x => x.GetHashBytes()));
		}

		return data.ToArray();
	}
}

public class Slot
{
	public byte ObjectType { get; set; }
	public bool MustBeExpandable { get; set; }
	public byte SubType { get; set; }
	public byte Unknown { get; set; }
	public short ObjectHandle { get; set; }

	public static Slot Read(BinaryReader reader)
	{
		var slot = new Slot();

		var bits = reader.ReadByte();

		slot.ObjectType = (byte)(bits >> 1);
		slot.MustBeExpandable = (bits & 0b00000001) != 0;
		slot.SubType = reader.ReadByte();
		slot.Unknown = reader.ReadByte();
		slot.ObjectHandle = reader.ReadInt16();

		return slot;
	}

	internal byte[] GetHashBytes()
	{
		var bytes = new List<byte>();
		bytes.Add(ObjectType);
		bytes.Add(SubType);
		bytes.AddRange(BitConverter.GetBytes(MustBeExpandable));
		return bytes.ToArray();
	}
}

public class ItemAttribute
{
	public ushort Attributes { get; set; }
	public bool Wield { get; set; }
	public bool Unwield { get; set; }
	public bool Activate { get; set; }
	public bool Use { get; set; }
	public bool TimeActivate { get; set; }
	public bool Fuse { get; set; }
	public byte Identified { get; set; }
	public short Count { get; set; }
	public ushort wParam { get; set; }
	public uint lParam { get; set; }

	public static ItemAttribute Read(BinaryReader reader)
	{
		var item = new ItemAttribute();
		var bytes = reader.ReadBytes(2);
		item.Attributes = (ushort)(BitConverter.ToUInt16(bytes) << 6);

		var bits = new BitArray(bytes);
		item.Wield = bits[10];
		item.Unwield = bits[11];
		item.Activate = bits[12];
		item.Use = bits[13];
		item.TimeActivate = bits[14];
		item.Fuse = bits[15];

		item.Count = (short)(reader.ReadInt16() >> 2);

		item.wParam = reader.ReadUInt16();
		item.lParam = reader.ReadUInt32();

		return item;
	}

	internal byte[] GetHashBytes()
	{
		var bytes = new List<byte>();
		bytes.AddRange(BitConverter.GetBytes(Attributes));
		bytes.AddRange(BitConverter.GetBytes(wParam));
		bytes.AddRange(BitConverter.GetBytes(lParam));
		return bytes.ToArray();
	}
}

public class StoryProgression
{
	public byte BurningFarm { get; set; }
	public byte Parchment { get; set; }
	public byte BurningHamlet { get; set; }
	public byte PatrolOrders { get; set; }
	public byte BanditChest { get; set; }
	public byte HrugnirFirst { get; set; }
	public byte HrugnirSpell { get; set; }
	public byte HrugnirFinal { get; set; }
	public byte FatherSpeach { get; set; }
	public byte EndGameMessage { get; set; }

	public static StoryProgression Read(BinaryReader reader)
	{
		var stories = new StoryProgression();
		
		reader.BaseStream.Position = 0xC7;

		stories.BurningFarm = reader.ReadByte();    //C7
		stories.Parchment = reader.ReadByte();      //C8
		stories.BurningHamlet = reader.ReadByte();  //C9
		reader.Skip(1);                             //CA
		stories.PatrolOrders = reader.ReadByte();   //CB
		stories.BanditChest = reader.ReadByte();    //CC
		stories.HrugnirFirst = reader.ReadByte();   //CD
		reader.Skip(1);                             //CE
		stories.HrugnirSpell = reader.ReadByte();   //CF
		stories.HrugnirFinal = reader.ReadByte();   //D0
		reader.Skip(1);                             //D1
		stories.FatherSpeach = reader.ReadByte();   //D2
		reader.Skip(2);                             //D3-D4
		stories.EndGameMessage = reader.ReadByte(); //D5
		
		return stories;
	}
}

public class SpellBook
{
	public sbyte HealMinorWounds { get; set; }
	public sbyte DetectObjects { get; set; }
	public sbyte Light { get; set; }
	public sbyte MagicArrow { get; set; }
	public sbyte PhaseDoor { get; set; }
	public sbyte Shield { get; set; }
	public sbyte Clairvoyance { get; set; }
	public sbyte ColdBolt { get; set; }
	public sbyte DetectMonsters { get; set; }
	public sbyte DetectTraps { get; set; }
	public sbyte Identify { get; set; }
	public sbyte Levitation { get; set; }
	public sbyte NeutralizePoison { get; set; }
	public sbyte ColdBall { get; set; }
	public sbyte HealMedWounds { get; set; }
	public sbyte FireBolt { get; set; }
	public sbyte LightningBolt { get; set; }
	public sbyte RemoveCurse { get; set; }
	public sbyte ResistFire { get; set; }
	public sbyte ResistCold { get; set; }
	public sbyte ResistLightning { get; set; }
	public sbyte ResistAcid { get; set; }
	public sbyte ResistFear { get; set; }
	public sbyte SleepMonster { get; set; }
	public sbyte SlowMonster { get; set; }
	public sbyte Teleport { get; set; }
	public sbyte RuneOfReturn { get; set; }
	public sbyte HealMajorWounds { get; set; }
	public sbyte Fireball { get; set; }
	public sbyte BallLightning { get; set; }
	public sbyte Healing { get; set; }
	public sbyte TransMonster { get; set; }
	public sbyte CreateTraps { get; set; }
	public sbyte HasteMonster { get; set; }
	public sbyte TeleportAway { get; set; }
	public sbyte CloneMonster { get; set; }
	
	public static SpellBook Read(BinaryReader reader)
	{
		var spells = new SpellBook();
		
		reader.BaseStream.Position = 0x1B2;

		spells.HealMinorWounds = reader.ReadSByte();
		reader.Skip(11);
		spells.DetectObjects = reader.ReadSByte();
		reader.Skip(11);
		spells.Light = reader.ReadSByte();
		reader.Skip(11);
		spells.MagicArrow = reader.ReadSByte();
		reader.Skip(11);
		spells.PhaseDoor = reader.ReadSByte();
		reader.Skip(11);
		spells.Shield = reader.ReadSByte();
		reader.Skip(11);
		spells.Clairvoyance = reader.ReadSByte();
		reader.Skip(11);
		spells.ColdBolt = reader.ReadSByte();
		reader.Skip(11);
		spells.DetectMonsters = reader.ReadSByte();
		reader.Skip(11);
		spells.DetectTraps = reader.ReadSByte();
		reader.Skip(11);
		spells.Identify = reader.ReadSByte();
		reader.Skip(11);
		spells.Levitation = reader.ReadSByte();
		reader.Skip(11);
		spells.NeutralizePoison = reader.ReadSByte();
		reader.Skip(11);
		spells.ColdBall = reader.ReadSByte();
		reader.Skip(11);
		spells.HealMedWounds = reader.ReadSByte();
		reader.Skip(11);
		spells.FireBolt = reader.ReadSByte();
		reader.Skip(11);
		spells.LightningBolt = reader.ReadSByte();
		reader.Skip(11);
		spells.RemoveCurse = reader.ReadSByte();
		reader.Skip(11);
		spells.ResistFire = reader.ReadSByte();
		reader.Skip(11);
		spells.ResistCold = reader.ReadSByte();
		reader.Skip(11);
		spells.ResistLightning = reader.ReadSByte();
		reader.Skip(11);
		spells.ResistAcid = reader.ReadSByte();
		reader.Skip(11);
		spells.ResistFear = reader.ReadSByte();
		reader.Skip(11);
		spells.SleepMonster = reader.ReadSByte();
		reader.Skip(11);
		spells.SlowMonster = reader.ReadSByte();
		reader.Skip(11);
		spells.Teleport = reader.ReadSByte();
		reader.Skip(11);
		spells.RuneOfReturn = reader.ReadSByte();
		reader.Skip(11);
		spells.HealMajorWounds = reader.ReadSByte();
		reader.Skip(11);
		spells.Fireball = reader.ReadSByte();
		reader.Skip(11);
		spells.BallLightning = reader.ReadSByte();
		reader.Skip(11);
		spells.Healing = reader.ReadSByte();
		reader.Skip(11);
		spells.TransMonster = reader.ReadSByte();
		reader.Skip(11);
		spells.CreateTraps = reader.ReadSByte();
		reader.Skip(11);
		spells.HasteMonster = reader.ReadSByte();
		reader.Skip(11);
		spells.TeleportAway = reader.ReadSByte();
		reader.Skip(11);
		spells.CloneMonster = reader.ReadSByte();
		
		return spells;
	}
}

public enum Spell
{
	HealMinorWounds,
	DetectObjects,
	Light,
	MagicArrow,
	PhaseDoor,
	Shield,
	Clairvoyance,
	ColdBolt,
	DetectMonsters,
	DetectTraps,
	Identify,
	Levitation,
	NeutralizePoison,
	ColdBall,
	HealMedWounds,
	FireBolt,
	LightningBolt,
	RemoveCurse,
	ResistFire,
	ResistCold,
	ResistLightning,
	ResistAcid,
	ResistFear,
	SleepMonster,
	SlowMonster,
	Teleport,
	RuneOfReturn,
	HealMajorWounds,
	Fireball,
	BallLightning,
	Healing,
	TransMonster,
	CreateTraps,
	HasteMonster,
	TeleportAway,
	CloneMonster,
}

public class CastleProcess
{
	const string DialogClass = "#32770";
	const string LoadSavedGameDialog = "Load Saved Game";
	const string CastleWindowClass = "WIN16119FCastle";

	private readonly Process _process;
	private readonly InputSimulator _input;

	public CastleProcess(Process process)
	{
		_process = process;
		_input = new InputSimulator();
	}

	public int Id => _process.Id;
	public int longDelay { get; set; } = 60;
	public int shortDelay { get; set; } = 20;

	public bool LoadDialogVisible
	{
		get
		{
			var dialogWindows = FindWindowsByClass(DialogClass)
				.Where(x => x.ProcessId == _process.Id)
				.Select(x => x.hWnd)
				.ToList();

			return dialogWindows.Any(x => GetWindowTitle(x) == LoadSavedGameDialog);
		}
	}

	public string WindowTitle => GetWindowTitle(GetCastleWindow());

	public static CastleProcess StartCastleProcess(int game, int x, int y, int width, int height)
	{
		var processStartInfo = new ProcessStartInfo
		{
			FileName = @"D:\castle\wine\otvdm.exe",
			Arguments = @$"D:\castle\CASTLE{game}.EXE",
			WorkingDirectory = @"D:\castle\",
		};

		var process = new CastleProcess(Process.Start(processStartInfo));

		Thread.Sleep(1000);

		var dialogs = FindWindowsByClass(DialogClass);

		var spashScreen = dialogs.Single(x => x.ProcessId == process.Id).hWnd;

		var windowInfo = GetWindowInfo(spashScreen);

		User32.SetForegroundWindow(spashScreen);

		process.SendKeys(VirtualKeyCode.ESCAPE);

		process.SendKeys(VirtualKeyCode.ESCAPE);

		process.SizeCastleWindow(x, y, width, height);

		return process;
	}

	public static CastleProcess GetCastleProcess()
	{
		var process = Process.GetProcessesByName("otvdm").FirstOrDefault()
		 ?? Process.GetProcessesByName("otvdmw").FirstOrDefault();
		 
		return process == null ? null : new CastleProcess(process);
	}

	public void SizeCastleWindow(int x, int y, int width, int height)
	{
		var hWnd = GetCastleWindow();

		User32.SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, PInvoke.User32.SetWindowPosFlags.SWP_ASYNCWINDOWPOS | PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW);
	}

	public void ClickCastleWindow()
	{
		var windowInfo = GetWindowInfo(GetCastleWindow());

		SetCursorPosition(windowInfo.rcWindow.left + 32, windowInfo.rcWindow.bottom - 32);

		LeftButtonClick();
	}
	
	public void SendKeys(params VirtualKeyCode[] keys)
	{
		EnsureFocused();
		foreach (var key in keys)
		{
			Thread.Sleep(shortDelay);
			_input.Keyboard.KeyPress(key);
		}
	}
	
	public void SendModifiedKey(VirtualKeyCode modifierKey, VirtualKeyCode key)
	{
		EnsureFocused();
		Thread.Sleep(shortDelay);
		_input.Keyboard.ModifiedKeyStroke(modifierKey, key);
	}

	public ItemWindow GetShopWindow()
	{
		var rect = GetWindowInfo(GetCastleWindow()).rcClient;
		var testPoint = new POINT { x = rect.right - 80, y = rect.top + 120 };

		var hwnd = User32.WindowFromPoint(testPoint);
		rect = GetWindowInfo(hwnd).rcClient;

		return new ItemWindow
		{
			Rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top),
		};
	}

	public ItemWindow GetInventoryWindow()
	{
		var rect = GetWindowInfo(GetCastleWindow()).rcClient;
		var testPoint = new POINT { x = rect.right - 80, y = rect.bottom - 120 };

		var hwnd = User32.WindowFromPoint(testPoint);
		rect = GetWindowInfo(hwnd).rcClient;

		return new ItemWindow
		{
			Rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top),
		};
	}

	public void SetCursorPosition(int x, int y)
	{
		User32.SetCursorPos(x, y);
	}
	
	public void LeftButtonClick()
	{
		_input.Mouse.LeftButtonClick();
	}

	public bool EnsureFocused()
	{
		var hWnd = User32.GetForegroundWindow();
		User32.GetWindowThreadProcessId(hWnd, out var processId);
		
		if(processId == _process.Id)
		{
			return true;
		}

		var processWindow = FindWindowsByProcess()
			.Select(x => (Class: GetWindowClass(x), hWnd: x))
			.OrderBy(x => x.Class == DialogClass ? 0 
				: x.Class == CastleWindowClass ? 1 
				: 2)
			.First();
			
		return User32.SetForegroundWindow(processWindow.hWnd);
	}

	public bool HasFocus()
	{
		var hWnd = User32.GetForegroundWindow();
		User32.GetWindowThreadProcessId(hWnd, out var processId);
		return processId == _process.Id;
	}

	public void ThrowIfLostFocus()
	{
		if (!HasFocus())
		{
			throw new Exception("!HasFocus()");
		}
	}
	
	public void LoadSaveFile(string filePath)
	{
		SendKeys(VirtualKeyCode.ESCAPE, VirtualKeyCode.ESCAPE);

		_input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.VK_F);

		SendKeys(VirtualKeyCode.VK_L);

		var stopwatch = Stopwatch.StartNew();

		while (!LoadDialogVisible)
		{
			if (stopwatch.ElapsedMilliseconds > 2000)
			{
				throw new TimeoutException();
			}
			Thread.Yield();
		}

		Thread.Sleep(shortDelay);

		_input.Keyboard.TextEntry(filePath);

		SendKeys(VirtualKeyCode.RETURN);

		Thread.Sleep(longDelay);
	}

	public void Kill() => _process.Kill();

	private IntPtr GetCastleWindow()
	{
		return FindWindowsByClass(CastleWindowClass)
			.Where(x => x.ProcessId == _process.Id)
			.Select(x => x.hWnd)
			.Single();
	}

	private List<IntPtr> FindWindowsByProcess()
	{
		List<IntPtr> handles = new();
		
		User32.EnumWindows((hWnd, lParam) =>
		{
			User32.GetWindowThreadProcessId(hWnd, out var processId);
			if(processId == _process.Id)
			{
				handles.Add(hWnd);
			}

			return true;
		}, IntPtr.Zero);
		
		return handles;
	}

	private static User32.WINDOWINFO GetWindowInfo(IntPtr hWnd)
	{
		var windowInfo = new User32.WINDOWINFO();
		User32.GetWindowInfo(hWnd, ref windowInfo);
		return windowInfo;
	}

	private static string GetWindowTitle(IntPtr handle)
	{
		const int nChars = 256;
		var buff = new char[nChars];
		var read = User32.GetWindowText(handle, buff, nChars);

		if (read > 0)
		{
			return new String(buff.AsSpan(0, read));
		}

		return null;
	}

	private static string GetWindowClass(IntPtr handle)
	{
		const int nChars = 256;
		var buff = new char[nChars];
		var read = User32.GetClassName(handle, buff, nChars);

		if (read > 0)
		{
			return new String(buff.AsSpan(0, read));
		}

		return null;
	}

	private static List<(IntPtr hWnd, int ProcessId)> FindWindowsByClass(string className)
	{
		var windows = new List<(IntPtr hWnd, int ProcessId)>();

		User32.EnumWindows((hWnd, lParam) =>
		{
			var info = GetWindowInfo(hWnd);

			var windowClass = GetWindowClass(hWnd);

			if (windowClass == className)
			{
				User32.GetWindowThreadProcessId(hWnd, out var processId);

				windows.Add((hWnd, processId));
			}

			return true;
		}, IntPtr.Zero);

		return windows;
	}
}

public class ItemWindow
{
	public Rectangle Rectangle { get; set; }
	
	public List<ItemInfo> Items { get; set; } = new();
			
	public void CaptureItems()
	{
		const string EmptySha = "9E5E8C12EA312B7627C84C2C1EDE898C51852744";
		using var image = CaptureScreen();
		var white = Color.FromArgb(255, 255, 255, 255);
		var black = Color.FromArgb(255, 0, 0, 0);

		var cols = image.Width / 80;
		var rows = image.Height / 80;

		var itemInfos = new List<ItemInfo>();
		var index = 0;
		for (var r = 0; r < rows; r++)
		{
			for (var c = 0; c < cols; c++)
			{
				var left = c * 80;
				var top = r * 80 + 35;

				var bytes = new byte[450];
				byte current = 0;

				for (var y = 0; y < 45; y++)
				{
					for (var x = 0; x < 80; x++)
					{
						var color = image.GetPixel(left + x, top + y);

						var shift = x % 8;

						if (color != white)
						{
							current |= (byte)(1 << shift);
						}

						if (shift == 7)
						{
							var i = y * 10 + x / 8;
							bytes[i] = current;
							current = 0;
						}
					}
				}

				var sha = BitConverter.ToString(SHA1.HashData(bytes)).Replace("-", "");
				if (sha != EmptySha)
				{
					itemInfos.Add(new ItemInfo
					{
						Column = c,
						Row = r,
						Sha = sha,
						Index = index,
					});
				}
				index++;
			}
		}

		Items = itemInfos;
	}

	private Bitmap CaptureScreen()
	{
		var bmp = new Bitmap(Rectangle.Width, Rectangle.Height, PixelFormat.Format32bppArgb);

		using var graphics = Graphics.FromImage(bmp);
		
		graphics.CopyFromScreen(Rectangle.Left, Rectangle.Top, 0, 0, Rectangle.Size, CopyPixelOperation.SourceCopy);

		return bmp;
	}
}

public class ItemInfo
{
	public int Index { get; set; }
	public int Column { get; set; }
	public int Row { get; set; }
	public string Sha { get; set; }
	public string Name { get; set; }
}

public static class CastleExtensions
{
	public static void ThrowIfNumLock(this InputSimulator input)
	{
		if (input.IsNumLockOn())
		{
			throw new Exception("Halting because numlock on");
		}
	}

	public static bool IsNumLockOn(this InputSimulator input)
	{
		return input.InputDeviceState.IsTogglingKeyInEffect(VirtualKeyCode.NUMLOCK);
	}

	public static int Seek(this BinaryReader reader, int offset, SeekOrigin seekOrigin = SeekOrigin.Current) => (int)reader.BaseStream.Seek(offset, seekOrigin);
	
	public static int Skip(this BinaryReader reader, int count) => (int)reader.BaseStream.Seek(count, SeekOrigin.Current);

	public static int GetPosition(this BinaryReader reader) => (int)reader.BaseStream.Position;

	public static string ToBitString(this BitArray bits, int skipBits = 0, int? takeBits = default)
	{
		var builder = new StringBuilder();

		if (skipBits < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(skipBits));
		}

		if (skipBits >= bits.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(skipBits));
		}

		if (takeBits < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(takeBits));
		}

		var lastBit = takeBits.HasValue
			? skipBits + takeBits - 1
			: bits.Length - 1;

		if (lastBit >= bits.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(takeBits));
		}

		for (var i = skipBits; i <= lastBit; i++)
		{
			builder.Append(bits[i] ? '1' : '0');
		}

		return builder.ToString();
	}

	public static string ToBitString(this IEnumerable<byte> bytes, int skipBits = 0, int? takeBits = default) => ToBitString(new BitArray(bytes.ToArray()), skipBits, takeBits);

	public static string ToHexString(this byte[] bytes, string separator = "")
	{
		return BitConverter.ToString(bytes).Replace("-", separator);
	}

	public static string ToShaString(this byte[] bytes)
	{
		return ToHexString(System.Security.Cryptography.SHA1.HashData(bytes)).ToLower();
	}
}

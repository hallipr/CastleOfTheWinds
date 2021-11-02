<Query Kind="Program" />

#load ".\Common"

void Main()
{
	var markdown = new MarkdownBuilder();
	var game = "castle1";
	using var reader = new BinaryReader(File.OpenRead(@$"D:\repos\CastleOfTheWinds\Notes\SaveFile\{game}\{game}.cwg"));

	var gameVersion = reader.ReadByte() == 0x76 ? 1 : 2;
	reader.Seek(0, SeekOrigin.Begin);

	markdown.AddHeadingLine("File Header", 1);

	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowU1(reader, "file version?, 76 == Castle1, 77 = Castle2");
		reader.Skip(7);
		m.AddValueRowU4(reader, "character data offset");
		m.AddValueRowU4(reader, "unknown offset");
		m.AddValueRowU4(reader, "unknown offset");
		m.AddValueRowU4(reader, "shops offset");
		m.AddValueRowU4(reader, "unknown offset");
		m.AddValueRowU4(reader, "unknown offset");
		m.AddValueRowU4(reader, "end of file offset, 4 bytes before EOF");
	});

	markdown.AddHeadingLine("Unused?", 1);
	markdown.AddHexBlock(reader, 92);

	markdown.AddHeadingLine("Player Header", 1);
	markdown.AddImage("character", $"./{game}/character.png");
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS1(reader, "Strength base");
		m.AddValueRowS1(reader, "Intelligence base");
		m.AddValueRowS1(reader, "Constitution base");
		m.AddValueRowS1(reader, "Dexterity base");
		m.AddValueRowS1(reader, "Strength");
		m.AddValueRowS1(reader, "Intelligence");
		m.AddValueRowS1(reader, "Constitution");
		m.AddValueRowS1(reader, "Dexterity");
		m.AddValueRowS1(reader, "Strength penalty");
		m.AddValueRowS1(reader, "Intelligence penalty");
		m.AddValueRowS1(reader, "Constitution penalty");
		m.AddValueRowS1(reader, "Dexterity penalty");
		m.AddValueRowUnknown(reader, 4);
		m.AddValueRowS1(reader, "Strength bonus");
		m.AddValueRowS1(reader, "Intelligence bonus");
		m.AddValueRowS1(reader, "Constitution bonus");
		m.AddValueRowS1(reader, "Dexterity bonus");
		m.AddValueRowS2(reader, "Hit points");
		m.AddValueRowS2(reader, "Hit points max");
		m.AddValueRowS2(reader, "Mana");
		m.AddValueRowS2(reader, "Mana max");
		m.AddValueRowS2(reader, "Character level");
		m.AddValueRowS4(reader, "Experience  (can be less than max?)");
		m.AddValueRowS4(reader, "Experience max");
		m.AddValueRowS2(reader, "Armor Value");
		m.AddValueRowS2(reader, "To Hit bonus");
		m.AddValueRowS2(reader, "To Damage bonus");
		m.AddValueRowS2(reader, "Speed");
		m.AddValueRowS2(reader, "Speed Base");
		m.AddValueRowS2(reader, "Speed Burden (as % from weight vs weight max)");
		m.AddValueRowS2(reader, "Game Version? (observed 00 in 1 and 02 in 2)");
		m.AddValueRowS1(reader, "Position, current Y");
		m.AddValueRowS1(reader, "Position, current X");
		m.AddValueRowS1(reader, "Position, previous Y");
		m.AddValueRowS1(reader, "Position, previous X");
		m.AddValueRowUnknown(reader, 2);
		m.AddValueRowS1(reader, "Gender");
	});

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 12);

	markdown.AddHeadingLine("Story Progression", 1);
	markdown.AddImage("character", $"./{game}/story.png");
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();

		if (gameVersion == 1)
		{
			m.AddValueRowS1(reader, "burining_farm");
			m.AddValueRowS1(reader, "parchment");
			m.AddValueRowS1(reader, "burining_hamlet");
			m.AddValueRowS1(reader, "page_gap_0");
			m.AddValueRowS1(reader, "patrol_orders");
			m.AddValueRowS1(reader, "bandit_chest");
			m.AddValueRowS1(reader, "hrugnir_first");
			m.AddValueRowS1(reader, "page_gap_1");
			m.AddValueRowS1(reader, "hrugnir_spell");
			m.AddValueRowS1(reader, "hrugnir_final");
			m.AddValueRowS1(reader, "page_gap_2");
			m.AddValueRowS1(reader, "father_speach");
			m.AddValueRowS1(reader, "page_gap_3");
			m.AddValueRowS1(reader, "page_gap_4");
			m.AddValueRowS1(reader, "end_game_1");
		}
		else
		{
			m.AddValueRowS1(reader, "Introductory text");
			m.AddValueRowS1(reader, "The Keep Guard's First speech");
			m.AddValueRowS1(reader, "King Lifthransir's First Speech");
			m.AddValueRowS1(reader, "page_gap_0");
			m.AddValueRowS1(reader, "King Lifthransir's Second Speech");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
			m.AddValueRowS1(reader, "unknown");
		}
	});

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 42);

	markdown.AddHeadingLine("Player name", 1);
	markdown.AddHexBlock(reader, m =>
	{
		var nameBytes = reader.ReadBytes(80)
			.TakeWhile(x => x != 0)
			.ToArray();

		var name = Encoding.ASCII.GetString(nameBytes);

		m.AddLine($"nul terminator at: {0x100 + nameBytes.Length:X}");
		m.AddLine($"Hex: {nameBytes.ToHexString(" ")}");
		m.AddLine($"Ascii: {name}");
	});

	markdown.AddHeadingLine("Player Resistances", 1);
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS2(reader, "Resist Fire");
		m.AddValueRowS2(reader, "Resist Cold");
		m.AddValueRowS2(reader, "Resist Lightning");
		m.AddValueRowS2(reader, "Resist Acid");
		m.AddValueRowS2(reader, "Resist Fear");
		m.AddValueRowS2(reader, "Resist Drain Life");
	});

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 82);

	markdown.AddHeadingLine("Spell Book", 1);
	AddSpellBlock(markdown, reader, "Heal Minor Wounds");
	AddSpellBlock(markdown, reader, "Detect Objects");
	AddSpellBlock(markdown, reader, "Light");
	AddSpellBlock(markdown, reader, "Magic Arrow");
	AddSpellBlock(markdown, reader, "Phase Door");
	AddSpellBlock(markdown, reader, "Shield");
	AddSpellBlock(markdown, reader, "Clairvoyance");
	AddSpellBlock(markdown, reader, "Cold Bolt");
	AddSpellBlock(markdown, reader, "Detect Monsters");
	AddSpellBlock(markdown, reader, "Detect Traps");
	AddSpellBlock(markdown, reader, "Identify");
	AddSpellBlock(markdown, reader, "Levitation");
	AddSpellBlock(markdown, reader, "Neutralize Poison");
	AddSpellBlock(markdown, reader, "Cold Ball");
	AddSpellBlock(markdown, reader, "Heal Medium Wounds");
	AddSpellBlock(markdown, reader, "Fire Bolt");
	AddSpellBlock(markdown, reader, "Lightening Bolt");
	AddSpellBlock(markdown, reader, "Remove Curse");
	AddSpellBlock(markdown, reader, "Resist Fire");
	AddSpellBlock(markdown, reader, "Resist Cold");
	AddSpellBlock(markdown, reader, "Resist Lightning");
	AddSpellBlock(markdown, reader, "Resist Acid");
	AddSpellBlock(markdown, reader, "Resist Fear");
	AddSpellBlock(markdown, reader, "Sleep Monster");
	AddSpellBlock(markdown, reader, "Slow Monster");
	AddSpellBlock(markdown, reader, "Teleport");
	AddSpellBlock(markdown, reader, "Rune of Return");
	AddSpellBlock(markdown, reader, "Heal Major Wounds");
	AddSpellBlock(markdown, reader, "Fireball");
	AddSpellBlock(markdown, reader, "Ball Lightning");
	AddSpellBlock(markdown, reader, "Healing");
	AddSpellBlock(markdown, reader, "Transmorgify Monster");
	AddSpellBlock(markdown, reader, "Create Traps");
	AddSpellBlock(markdown, reader, "Haste Monster");
	AddSpellBlock(markdown, reader, "Teleport Away");
	AddSpellBlock(markdown, reader, "Clone Monster");

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 48);

	markdown.AddHeadingLine("Spell Bar", 1);
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS2(reader, "Spell 0");
		m.AddValueRowS2(reader, "Spell 1");
		m.AddValueRowS2(reader, "Spell 2");
		m.AddValueRowS2(reader, "Spell 3");
		m.AddValueRowS2(reader, "Spell 4");
		m.AddValueRowS2(reader, "Spell 5");
		m.AddValueRowS2(reader, "Spell 6");
		m.AddValueRowS2(reader, "Spell 7");
		m.AddValueRowS2(reader, "Spell 8");
		m.AddValueRowS2(reader, "Spell 9");
	});

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 92);

	markdown.AddHeadingLine("Weight and Bulk", 1);
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS4(reader, "Weight");
		m.AddValueRowS4(reader, "Bulk");
		m.AddValueRowS4(reader, "Weight max");
		m.AddValueRowS4(reader, "Bulk max");
	});

	markdown.AddHeadingLine("Unknown", 1);
	markdown.AddHexBlock(reader, 8);

	markdown.AddHeadingLine("Inventory", 1);
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS2(reader, "Slot count");
		m.AddValueRowS2(reader, "Allocated slot count");
	});

	reader.Skip(-2);
	var slotCount = reader.ReadInt16();

	try
	{
		AddItems(markdown, reader, slotCount);

		markdown.AddHeadingLine("Unknown", 1);
		markdown.AddHexBlock(reader, reader.GetPosition()..);
	}
	finally
	{
		File.WriteAllLines(@$"D:\repos\CastleOfTheWinds\Notes\SaveFile\{game}.md", markdown.Lines);
	}
}

public void AddItems(MarkdownBuilder markdown, BinaryReader reader, short slotCount, string prefix = "")
{
	for (var x = 0; x < slotCount; x++)
	{
		markdown.AddHeadingLine($"Slot {prefix}{x}", 2);
		markdown.AddHexBlock(reader, m =>
		{
			m.AddValueHeader();
			m.AddValueRowS2(reader, "Required Type");
			m.AddValueRowS1(reader, "Required Sub Type");
			m.AddValueRowS2(reader, "Object Id (probably from a runtime pointer)");
		});
	}

	for (var x = 0; x < slotCount; x++)
	{
		markdown.AddHeadingLine($"Item {prefix}{x}", 2);
		markdown.AddHexBlock(reader, m =>
		{
			m.AddValueHeader();
			m.AddValueRowS1(reader, "Type");
			m.AddValueRowS1(reader, "Sub Type");
			m.AddValueRowS4(reader, "Value");
			m.AddValueRowS2(reader, "Name offset (has custom name if non-zero)");

			var position = reader.GetPosition();
			var byteValue = reader.ReadByte();
			var bits = new BitArray(new[] { byteValue });


			m.AddValueRow($"{position:X}.0", "b1", byteValue.ToString("X"), bits[0] ? "1" : "0", "Wield");
			m.AddValueRow($"{position:X}.1", "b1", byteValue.ToString("X"), bits[1] ? "1" : "0", "Unwield");
			m.AddValueRow($"{position:X}.2", "b1", byteValue.ToString("X"), bits[2] ? "1" : "0", "Activate");
			m.AddValueRow($"{position:X}.3", "b1", byteValue.ToString("X"), bits[3] ? "1" : "0", "Use");
			m.AddValueRow($"{position:X}.4", "b1", byteValue.ToString("X"), bits[4] ? "1" : "0", "Delete On Activate");
			m.AddValueRow($"{position:X}.5", "b1", byteValue.ToString("X"), bits[5] ? "1" : "0", "Charged");

			var identify = (bits[6] ? 2 : 0) + (bits[7] ? 1 : 0);
			m.AddValueRow($"{position:X}.6", "b2", byteValue.ToString("X"), identify.ToString(), "Identify");

			position = reader.GetPosition();
			byteValue = reader.ReadByte();
			bits = new BitArray(new[] { byteValue });

			m.AddValueRow($"{position:X}.0", "b1", byteValue.ToString("X"), bits[0] ? "1" : "0", "Identified");

			var enchantment = (bits[1] ? 2 : 0) + (bits[2] ? 1 : 0);
			m.AddValueRow($"{position:X}.1", "b2", byteValue.ToString("X"), enchantment.ToString(), "Enchantment");
			m.AddValueRow($"{position:X}.3", "b1", byteValue.ToString("X"), bits[3] ? "1" : "0", "Multiple");
			m.AddValueRow($"{position:X}.4", "b1", byteValue.ToString("X"), bits[4] ? "1" : "0", "HasFixedWeight");
			m.AddValueRow($"{position:X}.5", "b1", byteValue.ToString("X"), bits[5] ? "1" : "0", "HasFixedBulk");
			m.AddValueRow($"{position:X}.6", "b1", byteValue.ToString("X"), bits[6] ? "1" : "0", "NoExpand");
			
			var hasObjectList = bits[7];
			m.AddValueRow($"{position:X}.7", "b1", byteValue.ToString("X"), hasObjectList ? "1" : "0", "ObjectList");

			if (hasObjectList)
			{
				m.AddValueRowS2(reader, "Parent object list handle");
				m.AddValueRowS4(reader, "Weight");
				m.AddValueRowS4(reader, "Bulk");
				m.AddValueRowS4(reader, "Weight max");
				m.AddValueRowS4(reader, "Bulk max");
				m.AddValueRowS4(reader, "Weight fixed");
				m.AddValueRowS4(reader, "Bulk fixed");
				m.AddValueRowS2(reader, "Slot count");
				m.AddValueRowS2(reader, "Allocated slot count");

				reader.Skip(-2);
				
				var childSlotCount = reader.ReadInt16();
				AddItems(m, reader, childSlotCount, $"{prefix}{x}.");
			}
			else
			{
				position = reader.GetPosition();
				byteValue = reader.ReadByte();
				var attributeCount = (byte)(byteValue >> 4);
				var allocatedAttributeCount = (byte)(byteValue & 0b00001111);

				m.AddValueRow($"{position:X}.0", "b4", byteValue.ToString("X"), attributeCount.ToString(), "Attribute count");
				m.AddValueRow($"{position:X}.4", "b4", byteValue.ToString("X"), allocatedAttributeCount.ToString(), "Allocated attribute count");

				position = reader.GetPosition();
				byteValue = reader.ReadByte();

				var descriptionType = (byte)(byteValue >> 5);
				m.AddValueRow($"{position:X}.0", "b3", byteValue.ToString("X"), descriptionType.ToString(), "Description type");
				m.AddValueRow($"{position:X}.5", "b5", byteValue.ToString("X"), string.Empty, "unused");

				for (var i = 0; i < Math.Max(allocatedAttributeCount, (byte)1); i++)
				{
					m.AddHeadingLine($"Item {prefix}{x}, Attribute {i}", 2);
					m.AddHexBlock(reader, m =>
					{
						m.AddValueHeader();
						
						position = reader.GetPosition();
						var bytes = reader.ReadBytes(2);
						bits = new BitArray(bytes);

						m.AddValueRow($"{position:X}.0", "b10", bytes.ToHexString(" "), bits.ToBitString(0, 10), "Attributes");
						
						position += 1;
						m.AddValueRow($"{position:X}.2", "b1", bytes[1].ToString("X"), bits.ToBitString(10, 1), "Wield");
						m.AddValueRow($"{position:X}.3", "b1", bytes[1].ToString("X"), bits.ToBitString(11, 1), "Unwield");
						m.AddValueRow($"{position:X}.4", "b1", bytes[1].ToString("X"), bits.ToBitString(12, 1), "Activate");
						m.AddValueRow($"{position:X}.5", "b1", bytes[1].ToString("X"), bits.ToBitString(13, 1), "Use");
						m.AddValueRow($"{position:X}.6", "b1", bytes[1].ToString("X"), bits.ToBitString(14, 1), "TimeActivate");
						m.AddValueRow($"{position:X}.7", "b1", bytes[1].ToString("X"), bits.ToBitString(15, 1), "Fuse");

						position = reader.GetPosition();
						bytes = reader.ReadBytes(2);
						
						var count = BitConverter.ToInt16(new[] {bytes[0], (byte)(bytes[0] >> 6)});
						var countx = (byte)(bytes[1] & 0b00001111);

						m.AddValueRow($"{position:X}.0", "b14", bytes.ToHexString(" "), count.ToString(), "Count");
						
						position += 1;
						m.AddValueRow($"{position:X}.7", "b2", bytes.ToHexString(" "), countx.ToString(), "Count x");						
						m.AddValueRowS2(reader, "WParam");
						m.AddValueRowS4(reader, "LParam");
					});
				}
			}
		});
	}
}

public void AddSpellBlock(MarkdownBuilder markdown, BinaryReader reader, string name)
{
	markdown.AddHeadingLine(name, 2);
	markdown.AddHexBlock(reader, m =>
	{
		m.AddValueHeader();
		m.AddValueRowS2(reader, "Identity?");
		m.AddValueRowS1(reader, "Level");
		m.AddValueRowS1(reader, "Default mana cost");
		m.AddValueRowS1(reader, "Mana cost");
		m.AddValueRowS1(reader, "Category");
		m.AddValueRowS2(reader, "Time to cast * 1/10 sec");

		var position = reader.GetPosition();
		var bytes = reader.ReadBytes(2);
		m.AddValueRow(position, "b16", bytes.ToHexString(), bytes.ToBitString(), "Effect?");

		position = reader.GetPosition();
		bytes = reader.ReadBytes(2);
		m.AddValueRow(position, "b16", bytes.ToHexString(), bytes.ToBitString(), "Unknown");
	});
}

public class MarkdownBuilder
{
	public List<string> Lines { get; } = new();
	public string LastLine => Lines.Count > 0 ? Lines[Lines.Count-1] : null;

	public void AppendToLastLine(string value)
	{
		if (Lines.Count == 0)
		{
			Lines.Add(value);
		}
		else
		{
			var last = Lines[Lines.Count - 1];
			Lines.RemoveAt(Lines.Count - 1);
			Lines.Add(last + value);
		}
	}

	public void AddLine(string value)
	{
		Lines.Add(value);
	}

	public void EnsureBlankLine()
	{
		if (!string.IsNullOrEmpty(LastLine))
		{
			AddLine(string.Empty);
		}
	}

	public void AddHeadingLine(string heading, int level)
	{
		EnsureBlankLine();

		AddLine($"{new string('#', level)} {heading}");
	}

	public void AddHexBlock(BinaryReader reader, Range range)
	{
		EnsureBlankLine();
		AddLine("```");
		var (start, length) = range.GetOffsetAndLength((int)reader.BaseStream.Length);
		var end = start + length;
		
		reader.Seek(start, SeekOrigin.Begin);
		
		var builder = new StringBuilder();
		for (var position = start; position < end; position++)
		{
			// every 100 lines, write a column header
			if (position == start || position % 1600 == 0)
			{
				if (position > start)
				{
					AddLine(string.Empty);
				}

				AddLine("       00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F");
			}

			// add an offset to the front of each line
			if (position == start)
			{
				var skip = (int)(position % 16);
				builder.Append($"{position - skip:X5}:");
				builder.Append(' ', skip * 3);

			}
			else if (position % 16 == 0)
			{
				AddLine(builder.ToString());
				builder.Clear();
				builder.Append($"{position:X5}:");
			}

			builder.Append($" {reader.ReadByte():X2}");
		}

		AddLine(builder.ToString());
		AddLine("```");
	}

	public void AddHexBlock(BinaryReader reader, int length)
	{
		var start = reader.GetPosition();
		var end = start + length;
		AddHexBlock(reader, start..end);
	}

	public void AddHexBlock(BinaryReader reader, Action<MarkdownBuilder> detailAction)
	{
		var start = (int)reader.GetPosition();
		
		var markdown = new MarkdownBuilder();
		detailAction.Invoke(markdown);
		
		var end = (int)reader.GetPosition();
		
		AddHexBlock(reader, start..end);
		EnsureBlankLine();	
		Lines.AddRange(markdown.Lines);	
	}

	public void AddValueHeader()
	{
		AddLine(@"| offset | type | value h | value d | description |");
		AddLine(@"|-------:|------|--------:|--------:|-------------|");
	}

	public void AddValueRow(string position, string type, string hexValue, string decimalValue, string description)
	{
		AddLine(@$"| {position} | {type} | {hexValue} | {decimalValue} | {description} |");
	}

	public void AddValueRow(long position, string type, string hexValue, string decimalValue, string description)
	{
		AddValueRow(position.ToString("X"), type, hexValue, decimalValue, description);
	}

	public void AddValueRowU1(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadByte();
		AddValueRow(position, "u1", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowS1(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadSByte();
		AddValueRow(position, "s1", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowU2(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadUInt16();
		AddValueRow(position, "u2", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowS2(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadInt16();
		AddValueRow(position, "s2", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowU4(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadUInt32();
		AddValueRow(position, "u4", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowS4(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadInt32();
		AddValueRow(position, "s4", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowU8(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadUInt64();
		AddValueRow(position, "u8", value.ToString("X"), value.ToString(), description);
	}

	public void AddValueRowS8(BinaryReader reader, string description)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadInt64();
		AddValueRow(position, "s8", value.ToString("X"), value.ToString(), description);
	}

	internal void AddImage(string description, string url)
	{
		AddLine($"![{description}]({url})");
	}

	internal void AddValueRowUnknown(BinaryReader reader, int length)
	{
		var position = reader.BaseStream.Position;
		var value = reader.ReadBytes(length);
		
		var dec = length switch
		{
			1 => value[0].ToString(),
			2 => BitConverter.ToInt16(value, 0).ToString(),
			4 => BitConverter.ToInt32(value, 0).ToString(),
			_ => throw new ArgumentOutOfRangeException(nameof(length))
		};
		 
		AddValueRow(position, "??", value.ToHexString(" "), dec, $"unknown");
	}
}



// You can define other methods, fields, classes and namespaces here

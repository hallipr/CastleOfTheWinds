<Query Kind="Program">
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

public static class ResourceTypes
{
    public const ushort RT_CURSOR = 0x8001;
    public const ushort RT_BITMAP = 0x8002;
    public const ushort RT_ICON = 0x8003;
    public const ushort RT_MENU = 0x8004;
    public const ushort RT_DIALOG = 0x8005;
    public const ushort RT_STRING = 0x8006;
    public const ushort RT_FONTDIR = 0x8007;
    public const ushort RT_FONT = 0x8008;
    public const ushort RT_ACCELERATOR = 0x8009;
    public const ushort RT_RCDATA = 0x800a;
    public const ushort RT_MESSAGETABLE = 0x800b;
    public const ushort RT_GROUP_CURSOR = RT_CURSOR + 11;
    public const ushort RT_GROUP_ICON = RT_ICON + 11;
}

void Main()
{
    using var stream = new MemoryStream(File.ReadAllBytes(@"D:\Castle\CASTLE2.EXE"));
    using var reader = new BinaryReader(stream);

    // read and seek SEGMENTED EXE HEADER
    var headerOffset = reader.ReadAtOffset(0x3C, reader.ReadUInt16);
    var sig = reader.ReadAtOffset(headerOffset, () => new string(reader.ReadChars(2)));

    if (sig != "NE")
    {
        throw new Exception("Bad signature");
    }

    var resourceTableOffset = headerOffset + reader.ReadAtOffset(headerOffset + 0x24, reader.ReadUInt16);

    new
    {
        headerOffset,
        sig,
        resourceTableOffset,
        resourceTableOffsetHex = resourceTableOffset.AsHex(),
    }.Dump();

    var resourceTable = reader.ReadAtOffset(resourceTableOffset, () => ResourceTable.Read(reader));
    var blockLength = resourceTable.rscAlignShift;

    var icons = resourceTable.rscTypes[ResourceTypes.RT_ICON].rtNameInfo
        .Select(x => new { NameInfo = x, Bytes = reader.ReadAtOffset(x.rnOffset * blockLength, () => reader.ReadBytes(x.rnLength * blockLength)) })
        .ToArray();

    var iconGroups = resourceTable.rscTypes[ResourceTypes.RT_GROUP_ICON].rtNameInfo
        .Select(x => new { NameInfo = x, Parsed = IconGroupInfo.Read(reader.ReadAtOffset(x.rnOffset * blockLength, () => reader.ReadBytes(x.rnLength * blockLength))) })
        .ToArray()
        .Dump();

    foreach (var iconGroup in iconGroups)
    {
        var groupInfo = iconGroup.Parsed;
      
        var iconBytes = new List<byte>();
        using var outputStream = new MemoryStream();
        using var writer = new BinaryWriter(outputStream);
        writer.Write((ushort)0);
        writer.Write(groupInfo.Type);
        writer.Write(groupInfo.ImageCount);
        
        var imageOffset = 16 * groupInfo.ImageCount + 6;
        
        foreach (var entry in groupInfo.Entries)
        {
            writer.Write(entry.Width);
            writer.Write(entry.Height);
            writer.Write(entry.ColorCount);
            writer.Write((byte)0);
            writer.Write(entry.ColorPlanes);
            writer.Write(entry.BitsPerPixel);
            writer.Write(entry.ImageSize);
            writer.Write((uint)imageOffset);
            
            var nextEntryOffset = (int)writer.BaseStream.Position;
            
            writer.Seek(imageOffset, SeekOrigin.Begin);

            var icon = icons[entry.ImageOffset - 1];

            if (icon.Bytes.Length != entry.ImageSize)
            {
                icon.Bytes[(int)entry.ImageSize..].Dump();
            }

            var usedBytes = icon.Bytes[..(int)entry.ImageSize];

            writer.Write(usedBytes);
            
            imageOffset = (int)writer.BaseStream.Position;
            
            writer.Seek(nextEntryOffset, SeekOrigin.Begin);
        }
        
        var fileName = @$"d:\temp\{iconGroup.NameInfo.id}.ico";
        
        if(File.Exists(fileName))
            fileName = @$"d:\temp\{iconGroup.NameInfo.id}_2.ico";
            
        File.WriteAllBytes(fileName, outputStream.ToArray());
    }
    
    
    
    //var bitmaps = bitmapNameInfos.Select(x => reader.ReadAtOffset(x.rnOffset * resourceTable.rscAlignShift, () => BitmapInfo.Read(reader))).Dump();
    //
    //File.WriteAllBytes("d:\\temp3.bmp", bitmapHeader.Concat(bytes).ToArray());
    // 61572 - Len in bmp  & size of mbp file
    // f084 - End of bmp
    //   66 - end of header - 61470 to end     61454 - size in header
    // 

    //   61454  - from bitmap info
    //  118  40 + 78
    // 310880 - 4be60
    // 310968 - post header   
    // 372437 - 5AED5 End of bitmap 
    // 372447 - 5AEDF End padded
    
    // 61558 - Actual without bmp header (with dib header)
    // 61568 - from resource table entry
}

public class IconDirectoryEntry
{
    public byte Width;
    public byte Height;
    public byte ColorCount;
    private byte reserved;
    public ushort ColorPlanes;
    public ushort BitsPerPixel;
    public uint ImageSize;
    public uint ImageOffset;

    public static IconDirectoryEntry Read(BinaryReader reader, bool asGroupEntry = false)
    {
        var info = new IconDirectoryEntry
        {
            Width = reader.ReadByte(),
            Height = reader.ReadByte(),
            ColorCount = reader.ReadByte(),
            reserved = reader.ReadByte(),
            ColorPlanes = reader.ReadUInt16(),
            BitsPerPixel = reader.ReadUInt16(),
            ImageSize = reader.ReadUInt32(),
            ImageOffset = asGroupEntry ? reader.ReadUInt16() : reader.ReadUInt32(),
        };

        return info;
    }
}

public class IconGroupInfo
{
    public ushort Type;
    public ushort ImageCount;
    public IconDirectoryEntry[] Entries;
    
    public static IconGroupInfo Read(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);
        return Read(reader);
    }

    public static IconGroupInfo Read(BinaryReader reader)
    {
        reader.ReadUInt16();
        
        var info = new IconGroupInfo
        {
            Type = reader.ReadUInt16(),
            ImageCount = reader.ReadUInt16(),
        };
        
        info.Entries = Enumerable.Range(0, info.ImageCount)
            .Select(x => IconDirectoryEntry.Read(reader, true))
            .ToArray();
        
        return info;
    }
}

public class ResourceTable
{
    public ushort rscAlignShift;
    public Dictionary<ushort, TypeInfo> rscTypes = new();
    public byte[] rscResourceNames;
    
    public static ResourceTable Read(BinaryReader reader)
    {
        var resourceTable = new ResourceTable();
        
        resourceTable.rscAlignShift = (ushort)Math.Pow(2, reader.ReadUInt16());

        while (TypeInfo.TryRead(reader, out var typeInfo))
        {
            resourceTable.rscTypes.Add(typeInfo.rtTypeID, typeInfo);
        }
        
        return resourceTable;
    }
}

public class TypeInfo
{
    public ushort rtTypeID;
    public ushort rtResourceCount;
    public uint rtReserved;
    public List<NameInfo> rtNameInfo = new();
    public string typeIDHex => ((ushort)rtTypeID).AsHex();
    
    public static bool TryRead(BinaryReader reader, out TypeInfo typeInfo)
    {
        var typeId = reader.ReadUInt16();
        
        if (typeId == 0)
        {
            typeInfo = null;
            return false;
        }

        typeInfo = new TypeInfo();
        typeInfo.rtTypeID = typeId;
        typeInfo.rtResourceCount = reader.ReadUInt16();
        typeInfo.rtReserved = reader.ReadUInt32();
        
        var remaining = typeInfo.rtResourceCount;
        
        while (remaining-- > 0 && NameInfo.TryRead(reader, out var nameInfo))
        {
            typeInfo.rtNameInfo.Add(nameInfo);
        }
        
        return true;
    }
}

public class NameInfo
{
    public ushort rnOffset;
    public ushort rnLength;
    public ushort rnFlags;
    public ushort rnID;
    public ushort rnHandle;
    public ushort rnUsage;
    public string rnFlagsHex => rnFlags.AsHex();
    public int id => (rnID & 0x8000) == 0x8000 ? rnID - 0x8000 : rnID;
    
    public static bool TryRead(BinaryReader reader, out NameInfo nameInfo)
    {
        var offset = reader.ReadUInt16();
        
        if (offset == 0)
        {
            nameInfo = null;
            return false;
        }

        nameInfo = new NameInfo();
        nameInfo.rnOffset = offset;
        nameInfo.rnLength = reader.ReadUInt16();
        nameInfo.rnFlags = reader.ReadUInt16();
        nameInfo.rnID = reader.ReadUInt16();
        nameInfo.rnHandle = reader.ReadUInt16();
        nameInfo.rnUsage = reader.ReadUInt16();
        
        return true;        
    }
}

public static class Extensions
{
    public static void Seek(this BinaryReader reader, long offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        reader.BaseStream.Seek(offset, origin);
    }

    public static void Skip(this BinaryReader reader, long offset)
    {
        reader.Seek(offset, SeekOrigin.Current);
    }

    public static T ReadAtOffset<T>(this BinaryReader reader, long offset, Func<T> readFunction)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        return readFunction();
    }

    public static string AsHex(this ushort value) => BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    public static string AsHex(this uint value) => BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    public static string AsHex(this long value) => BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
    public static string AsHex(this int value) => BitConverter.ToString(BitConverter.GetBytes(value).Reverse().ToArray()).Replace("-", "");
}

// You can define other methods, fields, classes and namespaces here

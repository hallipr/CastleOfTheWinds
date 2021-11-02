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
    public const ushort RT_GROUP_CURSOR = 0x800c;
    public const ushort RT_GROUP_ICON = 0x800d;
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

    new {
        headerOffset,
        sig,
        resourceTableOffset,
        resourceTableOffsetHex = resourceTableOffset.AsHex(),
    }.Dump();
    
    var resourceTable = reader.ReadAtOffset(resourceTableOffset, () => ResourceTable.Read(reader));
    var bitmapNameInfos = resourceTable.rscTypes[ResourceTypes.RT_BITMAP].rtNameInfo;
    foreach (var bitmapInfo in bitmapNameInfos)
    {
        reader.Seek(bitmapInfo.rnOffset * resourceTable.rscAlignShift);
        
        var bytes = reader.ReadBytes(bitmapInfo.rnLength * resourceTable.rscAlignShift);
        var info = BitmapInfo.Read(bytes);
        
        var colorCount = info.ColorCount == 0 && info.BitsPerPixel < 16
            ? (uint)(1 << info.BitsPerPixel)
            : info.ColorCount;
        
        var bitmapHeaderSize = 14;
        
        var colorTableSize = colorCount * 4;
        
        var pixelArrayOffset = bitmapHeaderSize + info.HeaderSize + colorTableSize;
        var totalFileSize = bytes.Length + bitmapHeaderSize;
        var bitmapHeader = new byte[] { 0x42, 0x4D }
            .Concat(BitConverter.GetBytes((uint)(totalFileSize)))
            .Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 })
            .Concat(BitConverter.GetBytes((uint)pixelArrayOffset));

        var fileName = @$"d:\repos\castle\originalart\bitmaps\{bitmapInfo.id}.bmp";
        
        if(File.Exists(fileName))
            fileName = @$"d:\repos\castle\originalart\bitmaps\{bitmapInfo.id}_2.bmp";
            
        File.WriteAllBytes(fileName, bitmapHeader.Concat(bytes).ToArray());
    }
}

public class BitmapInfo
{
    public uint HeaderSize;
    public int Width;
    public int Height;
    public ushort ColorPaneCount;
    public ushort BitsPerPixel;
    public uint CompressionMethod;
    public uint ImageSize;
    public uint HorizontalResolution;
    public uint VerticalResolution;
    public uint ColorCount;
    public uint ImportantColorCount;
    public uint PixelArraySize;
    
    public Color[] ColorTable;

    public Image ColorSamples
    {
        get
        {
            var bitmap = new Bitmap(ColorTable.Length * 5, 5);
            var graphic = Graphics.FromImage(bitmap);
            for(var i = 0; i < ColorTable.Length; i++)
            {
               graphic.FillRectangle(new SolidBrush(ColorTable[i]), i * 5, 0, 5, 5);
            }
            graphic.Save();
            return bitmap;
        }
    }

    public static BitmapInfo Read(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);
        return Read(reader);
    }

    public static BitmapInfo Read(BinaryReader reader)
    {
        var start = reader.BaseStream.Position;
        
        var bitmap = new BitmapInfo
        {
            HeaderSize = reader.ReadUInt32(),
            Width = reader.ReadInt32(),
            Height = reader.ReadInt32(),
            ColorPaneCount = reader.ReadUInt16(),
            BitsPerPixel = reader.ReadUInt16(),
            CompressionMethod = reader.ReadUInt32(),
            ImageSize = reader.ReadUInt32(),
            HorizontalResolution = reader.ReadUInt32(),
            VerticalResolution = reader.ReadUInt32(),
            ColorCount = reader.ReadUInt32(),
            ImportantColorCount = reader.ReadUInt32(),
        };
        
        var rowSize = (bitmap.BitsPerPixel * Math.Abs(bitmap.Width) + 31) / 32 * 4;
        
        bitmap.PixelArraySize = (uint)(rowSize * Math.Abs(bitmap.Height));
        
        var colorCount = (int)Math.Pow(2, bitmap.BitsPerPixel);
        
        var colorTableStart = reader.BaseStream.Position;
        var colorTable = Enumerable.Range(0, colorCount).Select(x =>
        {
            var red = reader.ReadByte();
            var green = reader.ReadByte();
            var blue = reader.ReadByte();
            reader.ReadByte();
            
            return Color.FromArgb(red, green, blue);
        })
        .ToArray();

        bitmap.ColorTable = colorTable;
        
        var pixelsStart = reader.BaseStream.Position;
        
        return bitmap;
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

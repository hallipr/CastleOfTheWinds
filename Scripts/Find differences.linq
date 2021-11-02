<Query Kind="Program" />

void Main()
{
    // Find all the bytes that are the same between files in sets, then
	// find the differences between the sets..
	
	// i.e.  Find all the bytes that are the same for rats but differ from
	// goblins, those are bytes that differentiate rats and goblins

	var directory = @"D:\castle\test\creatures";
	var range = 0x19D0..;
	
	var sames = Directory.EnumerateFiles(directory, $"*.cwg")
		.GroupBy(x => Regex.Match(x, @"(\w+)\d\.cwg").Groups[1].Value).Dump()
		.Select(g => AllSame(g.Select(f => ByteDictionary(f, range)).ToArray()))
		.ToArray();

	AllDifferent(sames)
		.ToDictionary(x => x.Key.ToString("X4"), x => x.Value)
		.Dump();
}

public Dictionary<int, byte> ByteDictionary(string filePath, Range? range)
{
	var bytes = File.ReadAllBytes(filePath);
	var results = bytes.Select((Value, Index) => (Value, Index));

	if(range != null)
	{
		var calcRange = range.Value.GetOffsetAndLength(bytes.Length);
		var start = calcRange.Offset;
		var end = calcRange.Length + start;
		
		results = results.Where(x => x.Index >= start && x.Index < end);
	}

	return results.ToDictionary(x => x.Index, x => x.Value);
}

public Dictionary<TKey, TValue> AllSame<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
{
	var results = dictionaries[0].ToDictionary(x => x.Key, x => x.Value);
	var allKeys = results.Keys.ToArray();

	foreach (var (key, value) in results.ToArray())
	{
		foreach (var dictionary in dictionaries.Skip(1))
		{
			if(!dictionary.TryGetValue(key, out var otherValue) || !object.Equals(otherValue, value))
			{
				results.Remove(key);
				break;
			}
		}	
	}
	return results;
}

public Dictionary<TKey, TValue[]> AllDifferent<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
{
	var results = new Dictionary<TKey, TValue[]>();
	var allKeys = dictionaries.SelectMany(x => x.Keys).Distinct().ToArray();
	foreach (var key in allKeys)
	{
		var seen = new HashSet<TValue>();
		var values = new TValue[dictionaries.Length];
		var addKey = true;
		
		for(var i = 0; i < dictionaries.Length; i++)
		{
			if(dictionaries[i].TryGetValue(key, out var value) && seen.Add(value))
			{
				values[i] = value;
			}
			else
			{
				addKey = false;
				break;
			}
		}
		
		if(addKey)
		{
			results[key] = values;
		}
	}
	return results;
}

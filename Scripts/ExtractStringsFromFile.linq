<Query Kind="Statements">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var text = File.ReadAllText(@"D:\Castle\CASTLE1.EXE", Encoding.UTF8)
+ "\r\n" + File.ReadAllText(@"D:\Castle\CASTLE2.EXE", Encoding.UTF8);

var matches = Regex.Matches(text, @"[a-z0-9 \t\r\n`~!@#%&=:;'""<>,/\-\.\$\^\{\}\[\]\(\)\|\*\+\?\\]{2,}".Dump(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

File.WriteAllText(@"D:\castle\castle strings.json", JsonConvert.SerializeObject(matches
    .Select(x => x.Value.Trim())
    .Distinct(), Newtonsoft.Json.Formatting.Indented));

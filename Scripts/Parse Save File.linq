<Query Kind="Program">
  <Output>DataGrids</Output>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Input</Namespace>
</Query>

#load ".\Common"

void Main()
{
	Directory.EnumerateFiles(@"D:\castle\saves", "*.cwg", SearchOption.AllDirectories)
		.Select(SaveData.Read)
		.Dump();
}
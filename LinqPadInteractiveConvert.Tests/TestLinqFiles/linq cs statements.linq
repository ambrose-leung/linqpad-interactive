<Query Kind="Program">
  <NuGetReference>FlysEngine.Desktop</NuGetReference>
  <NuGetReference Version="0.9.1">Rock.Core.Newtonsoft</NuGetReference>
  <Namespace>Rock.Serialization</Namespace>
  <Namespace>SharpDX.Mathematics.Interop</Namespace>
  <Namespace>SharpDX.Multimedia</Namespace>
  <Namespace>SharpDX.Text</Namespace>
  <Namespace>SharpDX.Win32</Namespace>
</Query>

void Main()
{
	var x = new Dictionary<string,string>();
	DictionarySetter(x);
	x.Dump("asdf2");
}

void DictionarySetter(Dictionary<string,string> x){
	x["asdf"] ="adfgadfg";
}
// You can define other methods, fields, classes and namespaces here
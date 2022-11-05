<Query Kind="Program">

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
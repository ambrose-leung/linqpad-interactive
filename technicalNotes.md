This is where the magic happens:

```
#r "nuget:LINQPad.Runtime, 6.14.10"
using LINQPad;
using Microsoft.AspNetCore.Html;

public static void Dump<T>(this T objectToSerialize)
{
    var writer = LINQPad.Util.CreateXhtmlWriter(true);
    writer.Write(objectToSerialize);
    string strHTML = writer.ToString();
    
    display(new HtmlString(strHTML.Replace("background:white","").Replace("background-color:#ddd;","")));
}
```

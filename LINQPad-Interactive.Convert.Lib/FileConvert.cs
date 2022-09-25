using LINQPad_Interactive.Convert.Lib.LinqPad;
using Microsoft.DotNet.Interactive.Documents;
using Microsoft.DotNet.Interactive.Documents.Jupyter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LINQPad_Interactive.Convert.Lib
{
    public class FileConvert
    {
        /// <summary>
        /// Converts a .linq file to a .ipynb file that can be opened in VSCode
        /// </summary>
        /// <param name="linqPadFilePath">Must end with .linq</param>
        /// <param name="outputFilePath"></param>
        /// <param name="initCodeFilePath"></param>
        /// <param name="internalNugetPaths">Ends with .json (or a file path containing nupkg files)</param>
        /// <returns></returns>
        public static ConvertStatus ConvertToInteractiveNotebook(string linqPadFilePath, string outputFilePath = null, string[] internalNugetPaths = null)
        {
            outputFilePath = outputFilePath ?? linqPadFilePath.Substring(0, linqPadFilePath.LastIndexOf(".")) + ".ipynb";

            if (File.Exists(outputFilePath))
            {
                return new ConvertStatus { Status = "Output File Already Exists - operation canceled", InputFile = linqPadFilePath, OutputFile = outputFilePath };
            }

            internalNugetPaths = internalNugetPaths ?? new string[0];

            var linqPadFile = File.ReadAllText(linqPadFilePath);

            //get xml part
            var fileSplit = linqPadFile.Split(new string[] { $"{Environment.NewLine}{Environment.NewLine}" }, 2, StringSplitOptions.None);
            if (fileSplit.Length != 2)
            {
                fileSplit = linqPadFile.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
            }
            if (fileSplit.Length != 2)
            {
                Console.WriteLine("Error: could not split .linq file by 2 new lines.");
                return new ConvertStatus { Status = "Error: could not split .linq file by 2 new lines.", InputFile = linqPadFilePath, OutputFile = "" };
            }
            var xml = fileSplit[0];
            var code = fileSplit[1];

            var queryMetadata = GetLinqPadQueryMetadata(xml);

            var interactiveDoc = new InteractiveDocument();
            var codeForInteractiveNotebook = new StringBuilder();

            if (queryMetadata.Kind == "Statements" || queryMetadata.Kind == "Program")
            {
                foreach (var internalNuget in internalNugetPaths)
                {
                    codeForInteractiveNotebook.AppendLine($"#i \"{internalNuget}\"");
                }

                var nugetImports = ParseAndGetNugetImports(queryMetadata);//add the #r nuget statements at the top
                nugetImports = nugetImports.Append(@"#r ""nuget: LINQPad.Runtime""");
                
                var usingStatements = ParseNamespacesGetUsingStatements(queryMetadata);
                usingStatements = usingStatements.Append($"using LINQPad;{Environment.NewLine}using Microsoft.AspNetCore.Html;");

                
                codeForInteractiveNotebook.AppendLine(string.Join(Environment.NewLine, nugetImports));
                codeForInteractiveNotebook.AppendLine(string.Join(Environment.NewLine, usingStatements));
                
                codeForInteractiveNotebook.Append(@"public static T Dump<T>(this T objectToSerialize)
{
    var writer = LINQPad.Util.CreateXhtmlWriter(true);
    writer.Write(objectToSerialize);
    string strHTML = writer.ToString();

    display(new HtmlString(strHTML.Replace(""background:white"", """").Replace("";background-color:white"", """").Replace(""background-color:#ddd;"", """")));
    return objectToSerialize;
}

public static T Dump<T>(this T objectToSerialize, string heading)
{
    Util.WithHeading(objectToSerialize, heading).Dump();
    return objectToSerialize;
}
");

                interactiveDoc.Add(new InteractiveDocumentElement()
                {
                    Contents = codeForInteractiveNotebook.ToString(),
                    Language = "csharp"
                });

                if(queryMetadata.Kind == "Program")
                {
                    string awaitPrepend = "";
                    if(code.Contains("async Task Main"))
                    {
                        awaitPrepend = "await "; 
                    }
                    code = awaitPrepend + "Main();" + Environment.NewLine + code;
                }

                interactiveDoc.Add(new InteractiveDocumentElement()
                {
                    Contents = code,
                    Language = "csharp"
                });

                File.WriteAllText(outputFilePath, interactiveDoc.Serialize());
            }
            return new ConvertStatus { Status = "Ok", InputFile = linqPadFilePath, OutputFile = outputFilePath };

            
        }

        private static IEnumerable<string> ParseNamespacesGetUsingStatements(Query queryMetadata)
        {
           
            if (queryMetadata.Namespace != null)
            {
                foreach (var lpnamespace in queryMetadata.Namespace)
                {

                    yield return $"using {lpnamespace};";
                }
            }
            var defaultNamespaces = new[] { "System", "System.Collections", "System.Collections.Generic", "System.Data", "System.Diagnostics", "System.IO", "System.Linq", "System.Linq.Expressions", "System.Reflection", "System.Text", "System.Text.RegularExpressions", "System.Threading", "System.Transactions", "System.Xml", "System.Xml.Linq", "System.Xml.XPath" };
            foreach (var defaultNs in defaultNamespaces)
            {
                yield return $"using {defaultNs};";
            }
        }

        private static IEnumerable<string> ParseAndGetNugetImports(Query queryMetadata)
        {
            if (queryMetadata.NuGetReference != null)
            {
                foreach (var nugetRef in queryMetadata.NuGetReference)
                {
                    if (nugetRef.Version == null)
                    {
                        yield return $"#r \"nuget:{nugetRef.Value}\"";
                    }
                    else
                    {
                        yield return $"#r \"nuget:{nugetRef.Value}, {nugetRef.Version}\"";
                    }
                }
            }
        }

        /// <summary>
        /// returns 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns>the LinqPadQuery metadata (the type and what it references)</returns>
        public static LinqPad.Query GetLinqPadQueryMetadata(string xmlString)
        {
            XmlSerializer ser = new XmlSerializer(typeof(LinqPad.Query));
            LinqPad.Query q;
            using (XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.ASCII.GetBytes(xmlString))))
            {
                q = (LinqPad.Query)ser.Deserialize(reader);
            }
            return q;
        }
    }
}

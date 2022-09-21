using Microsoft.DotNet.Interactive.Documents;
using Microsoft.DotNet.Interactive.Documents.Jupyter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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
        public static ConvertStatus ConvertToInteractiveNotebook(string linqPadFilePath, string outputFilePath = null, string initCodeFilePath = @"Data\InteractiveNotebookBootstrap.txt", string[] internalNugetPaths = null)
        {
            outputFilePath = outputFilePath ?? linqPadFilePath.Substring(0, linqPadFilePath.LastIndexOf(".")) + ".ipynb";
            internalNugetPaths = internalNugetPaths ?? new string[0];

            var linqPadFile = File.ReadAllText(linqPadFilePath);

            //get xml part
            var fileSplit = linqPadFile.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
            var xml = fileSplit[0];
            var code = fileSplit[1];

            var queryMetadata = GetLinqPadQueryMetadata(xml);

            InteractiveDocument interactiveDoc = new InteractiveDocument();
            var codeForInteractiveNotebook = new StringBuilder();

            if (queryMetadata.Kind == "Statements" || queryMetadata.Kind == "Program")
            {
                AddNugetImports();//add the #r nuget statements at the top

                if (File.Exists(initCodeFilePath))
                {
                    codeForInteractiveNotebook.Append(File.ReadAllText(@"Data\InteractiveNotebookBootstrap.txt"));
                }
                else
                {
                    codeForInteractiveNotebook.Append(@"#r ""nuget: LINQPad.Runtime""
using LINQPad;
using Microsoft.AspNetCore.Html;

public static void Dump<T>(this T objectToSerialize)
{
    var writer = LINQPad.Util.CreateXhtmlWriter(true);
    writer.Write(objectToSerialize);
    string strHTML = writer.ToString();

    display(new HtmlString(strHTML.Replace(""background:white"", """").Replace("";background-color:white"", """").Replace(""background-color:#ddd;"", """")));
}

public static void Dump<T>(this T objectToSerialize, string heading)
{
    Util.WithHeading(objectToSerialize, heading).Dump();
}
");
                }

                interactiveDoc.Add(new InteractiveDocumentElement()
                {
                    Contents = codeForInteractiveNotebook.ToString(),
                    Language = "csharp"
                });

                if(queryMetadata.Kind == "Program")
                {
                    code = "Main();" + Environment.NewLine + code;
                }

                interactiveDoc.Add(new InteractiveDocumentElement()
                {
                    Contents = code,
                    Language = "csharp"
                });

                File.WriteAllText(outputFilePath, interactiveDoc.Serialize());
            }
            return new ConvertStatus { Status = "Ok", InputFile = linqPadFilePath, OutputFile = outputFilePath, InitializeCodeFile = Path.GetFullPath(initCodeFilePath) };

            void AddNugetImports()
            {
                foreach (var internalNugetPath in internalNugetPaths)
                {
                    codeForInteractiveNotebook.Append($"#i \"nuget:{internalNugetPath}\"").Append(Environment.NewLine);
                }
                foreach (var nugetRef in queryMetadata.NuGetReference)
                {
                    if (nugetRef.Version == null)
                    {
                        codeForInteractiveNotebook.Append($"#r \"nuget:{nugetRef.Value}\"").Append(Environment.NewLine);
                    }
                    else
                    {
                        codeForInteractiveNotebook.Append($"#r \"nuget:{nugetRef.Value}, {nugetRef.Version}\"").Append(Environment.NewLine);
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

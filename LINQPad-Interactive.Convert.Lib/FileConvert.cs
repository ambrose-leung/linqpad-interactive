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
        public static ConvertStatus ConvertToInteractiveNotebook(string linqPadFilePath, string outputFilePath = null)
        {
            outputFilePath = outputFilePath ?? linqPadFilePath.Substring(0, linqPadFilePath.LastIndexOf(".")) + ".ipynb";

            var linqPadFile = File.ReadAllText(linqPadFilePath);

            //get xml part
            var fileSplit = linqPadFile.Split(new string[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
            var xml = fileSplit[0];
            var code = fileSplit[1];

            var queryMetadata = GetLinqPadQueryMetadata(xml);
            if (queryMetadata.Kind == "Statements")
            {
                var codeForInteractiveNotebook = new StringBuilder();

                foreach(var nugetRef in queryMetadata.NuGetReference)
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
                codeForInteractiveNotebook.Append(File.ReadAllText(@"Data\InteractiveNotebookBootstrap.txt"));

                var interactiveNotebook = File.ReadAllText(@"Data\SampleNotebook.ipynb.json");
                var notebookObj = JsonConvert.DeserializeObject<LINQPad_Interactive.Convert.Lib.InternactiveNotebookJson>(interactiveNotebook);


                notebookObj.cells[0].source = ConvertCodeToInteractiveNotebookFormat(codeForInteractiveNotebook.ToString());
                notebookObj.cells[1].source = ConvertCodeToInteractiveNotebookFormat(code);

                File.WriteAllText(outputFilePath, JsonConvert.SerializeObject(notebookObj));
            }
            return new ConvertStatus { Status = "Ok" };
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

        private static string[] ConvertCodeToInteractiveNotebookFormat(string code)
        {
            var codeSplit =  code.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            return codeSplit;
        }
    }
}

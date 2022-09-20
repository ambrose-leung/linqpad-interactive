using LINQPad_Interactive.Convert.Lib;
using LINQPad_Interactive.Convert.Lib.LinqPad;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LinqPadInteractiveConvert.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ParseXML()
        {
            var xml = @"<Query Kind=""Statements"">
  <NuGetReference>FlysEngine.Desktop</NuGetReference>
  <NuGetReference Version=""0.9.1"">Rock.Core.Newtonsoft</NuGetReference>
</Query>";

            XmlSerializer ser = new XmlSerializer(typeof(Query));
            Query q;
            using (XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.ASCII.GetBytes(xml))))
            {
                q = (Query)ser.Deserialize(reader);
            }
        }

        [TestMethod]
        public void ParseInteractiveNotebook()
        {
            var interactiveNotebook = File.ReadAllText("Data\\SampleNotebook.ipynb.json");
            var notebookObj = JsonConvert.DeserializeObject<LINQPad_Interactive.Convert.Lib.InternactiveNotebookJson>(interactiveNotebook);
        }

        [TestMethod]
        public void ConvertToInteractiveNotebook_KindStatements()
        {
            FileConvert.ConvertToInteractiveNotebook(@"TestLinqFiles\linq cs statements.linq");
        }

        [TestMethod]
        public void ConvertToInteractiveNotebook_KindProgram()
        {
            FileConvert.ConvertToInteractiveNotebook(@"TestLinqFiles\linq c# program.linq");
        }
    }
}

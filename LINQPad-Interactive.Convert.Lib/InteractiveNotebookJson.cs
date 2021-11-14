using System;
using System.Collections.Generic;
using System.Text;

namespace LINQPad_Interactive.Convert.Lib
{

    public class InternactiveNotebookJson
    {
        public Cell[] cells { get; set; }
        public Metadata metadata { get; set; }
        public int nbformat { get; set; }
        public int nbformat_minor { get; set; }
    }

    public class Metadata
    {
        public Kernelspec kernelspec { get; set; }
        public Language_Info language_info { get; set; }
        public int orig_nbformat { get; set; }
    }

    public class Kernelspec
    {
        public string display_name { get; set; }
        public string language { get; set; }
        public string name { get; set; }
    }

    public class Language_Info
    {
        public string file_extension { get; set; }
        public string mimetype { get; set; }
        public string name { get; set; }
        public string pygments_lexer { get; set; }
        public string version { get; set; }
    }

    public class Cell
    {
        public string cell_type { get; set; }
        public object execution_count { get; set; }
        public Metadata1 metadata { get; set; }
        public object[] outputs { get; set; }
        public string[] source { get; set; }
    }

    public class Metadata1
    {
        public Dotnet_Interactive dotnet_interactive { get; set; }
        public string generatedWith { get; set; }
    }

    public class Dotnet_Interactive
    {
        public string language { get; set; }
    }

}

using LINQPad_Interactive.Convert.Lib;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ambrose.Linqpad.Convert.KernelExt
{
    public class ConvertLinqpadExtension : IKernelExtension
    {
        const string MAGIC_CMD = "#!convert2dnn";
        static Command importCommand;

        static string[] SUPPORTED_LANGUAGES = { "csharp","fsharp","javascript", "pwsh" };

        public Task OnLoadAsync(Kernel kernel)
        {
            importCommand = new Command(MAGIC_CMD, "Convert basic .linq scripts to a .NET interactive notebook");
            importCommand.AddArgument(new Argument<string>("linqfile"));
            //importCommand.AddOption(new Option<bool>(new[] { "-v", "--verbose" }, "Display imported notebook output"));

            importCommand.Handler = CommandHandler.Create(
                async (string linqfile, bool? ver, KernelInvocationContext context) =>
                {
                    if (string.IsNullOrWhiteSpace(linqfile))
                    {
                        context.Display(new HtmlString($@"<b>Missing path argument</b>"));
                        return;
                    }

                    var notebookPathNoExt = Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                        Path.GetDirectoryName(linqfile),
                        Path.GetFileNameWithoutExtension(linqfile));

                    context.Display(new HtmlString($@"Converting <b>{linqfile}</b> [{notebookPathNoExt}] ..."));

                    // load notebook cells
                    var convertStatus = FileConvert.ConvertToInteractiveNotebook(notebookPathNoExt + ".linq");

                    var fileopener = new System.Diagnostics.Process();
                    try
                    {
                        fileopener.StartInfo.FileName = "explorer";
                        fileopener.StartInfo.Arguments = $"\"{notebookPathNoExt}.ipynb\"";
                        fileopener.Start();
                    }
                    catch (Exception) { }

                    context.Display(new HtmlString($@"Linqpad script converted to notebook: <code>{notebookPathNoExt}.ipynb</code>"));
                });

            kernel.AddDirective(importCommand);

            if (KernelInvocationContext.Current is { } context)
            {
                $"`{nameof(ConvertLinqpadExtension)}` is loaded. It will convert .linq to .ipynb.  Try it by running: `{MAGIC_CMD} path\\linqpadScript.linq`".DisplayAs("text/markdown");
            }

            return Task.CompletedTask;
        }

        private static string GetValidNamespace(string ns)
        {
            return string.Join(".", ns.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(chunk => chunk)); // TODO: replace invalid char in chunk 
        }
    }
}

# LINQPad ➡️ .NET Notebook
Convert .linq to .ipynb so that you can run your [LINQPad](https://www.linqpad.net/) scripts in VSCode.  

This is currently in the "Proof of concept" stage

# Usage

## Convert from VS Code
Install Extension from VS Code Extensions Marketplace, open linqpad file, right click and choose Convert
![Open .linq file, Right click editor, Convert](https://github.com/ambrose-leung/vscode-ext-convert2dnn/blob/main/ExampleUsage.gif)

[Repo for extension](https://github.com/ambrose-leung/vscode-ext-convert2dnn)

## Convert from .NET notebook
User creates .ipynb and runs 2 cells of C# code
New .ipynb is generated and opened in VS Code (on Windows).

In a new/existing .NET Interactive Notebook (.ipynb) - use VS Code's .NET Interactive Extension to open it

Cell 1
```
#i "C:\Users\amb12\source\repos\ambrose-leung\build\nupkg" //if building locally
#r "nuget:Ambrose.Linqpad.Convert.KernelExt"
```

Cell 2 (Requires the path to the .linq file to be entered manually)
```
#!convert2dnn "C:\path\to\linqpadscript.linq" //dnn = Dot Net Notebook
```

# Technical
- Uses [`Microsoft.DotNet.Interactive.Documents`](https://www.nuget.org/packages/Microsoft.DotNet.Interactive.Documents/) nuget to create .ipynb
- Will use [`Chino.Jupyter.Extensions`](https://github.com/roberchi/Chino.Jupyter.Extensions) to implement `#load` functionality
- Logic for conversion is packaged as a .NET library
- Implements IKernelExtension to expose the magic command #!convert2dnn

# Backlog and Feature Requests
Go to Issues tab

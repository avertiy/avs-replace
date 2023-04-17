# search-and-replace

A .NET Global tool for search and replace in files, similar to Ctrl+Shift+F or Ctrl+Shift+H in Visual Studio

## How to install?

To install it, run the following command in CMD:

```
dotnet tool install -g replace
```

## How to use?

In command line, you can invoke this tool on any file using the following syntax:

```
replace "file-path.txt" -set "placeholder1" "value1" -set "placeholder2" "value2" -set ...
```

It will simply load the file as text into memory, replace each of the specified placeholders with the provided value, and overwrite the file.


dotnet tool install -g dotnetsay
You can uninstall the tool using the following command.
dotnet tool uninstall -g dotnetsay
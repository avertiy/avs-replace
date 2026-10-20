# search-and-replace

A .NET Global tool for search and replace in files, similar to Ctrl+Shift+F or Ctrl+Shift+H in Visual Studio

## How to install?

To install it, run the following command in CMD:

```
dotnet tool install -g avs-replace
>You can invoke the tool using the following command: replace
>Tool 'avs-replace' (version '9.0.1') was successfully installed
```

To uninstall the tool

```
dotnet tool uninstall -g avs-replace
```
## How to use?

Basically the tool has been designed for use case when you need to quickly add another implementation
which is based on some template e.g. you have project structure:

 /Clients 
     /Binance
		BinanceClient.cs
		BinanceAuthenticator.cs
		/BinanceObjects
			BinanceTrade.cs
			...
		..
	 /Bytbit
		BybitClient.cs
		BybitAuthenticator.cs
		/BybitObjects
			BybitTrade.cs
			...
		..
You want to add another client let's say BitGet
you copy an existing client e.g. Binance folder, and you need to rename files, classes, namespaces etc. 

/BitGet
		BinanceClient.cs
		BinanceAuthenticator.cs
		/BinanceObjects
			BinanceTrade.cs
			...

```
> repl Binance BitGet -w Clients\BitGet
```
/BitGet
		BitGetClient.cs
		BitGetAuthenticator.cs
		/BitGetObjects
			BitGetTrade.cs
			...
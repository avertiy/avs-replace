namespace AVS.Replace.Enums;

[Flags]
public enum FileTypeClass
{
	/// <summary>
	/// text, code, configuration files
	/// </summary>
	Text =1,
	/// <summary>
	/// *.cs;*.js;*.ts
	/// </summary>
	Code = 2,
	/// <summary>
	/// *.json;*.env;*.yml;dot-files like .gitignore;*.config
	/// </summary>
	Config = 4,
	/// <summary>
	/// *.xml; *.csproj; *.sln etc.
	/// </summary>
	Xml = 8,
	AllText = Text | Code | Config | Xml,
	///// <summary>
	///// *.mp4; *.mp3 etc. also images *.jpeg;*.gif;*.icon etc.
	///// </summary>
	//Media =4,
	/// <summary>
	/// *.exe;*.dll;*.pdb;*.msi; also archives *.zip;*.7z;*.rar etc.
	/// </summary>
	Bin =16,
	/// <summary>
	/// media, images, office & documents etc.
	/// </summary>
	Other = 64,
	AllNonText = Bin | Other	
}

public static class FileTypeClassExtensions
{
	public static string[] GetFileTypes(this FileTypeClass fileTypeClass)
	{
		var list = new List<string>();

		if (fileTypeClass.HasFlag(FileTypeClass.Bin))
		{
			list.Add("*.exe");
			list.Add("*.pdb");
			list.Add("*.dll");
			list.Add("*.msi");
			list.Add("*.zip");
			list.Add("*.7z");
			list.Add("*.bin");
			list.Add("*.mdf");
			list.Add("*.ldf");
			list.Add("*.dat");
		}

		if (fileTypeClass.HasFlag(FileTypeClass.Other))
		{
			list.Add("*.csv");
			list.Add("*.doc");
			list.Add("*.docx");
			list.Add("*.hlp");
			list.Add("*.pdf");
			list.Add("*.ppt");
			list.Add("*.xls");
			list.Add("*.xlsx");

			list.Add("*.jpg");
			list.Add("*.jpeg");
			list.Add("*.png");
			list.Add("*.ico");
			list.Add("*.icon");
			list.Add("*.tiff");
			list.Add("*.gif");

			list.Add("*.mp3");
			list.Add("*.mp4");
		}

		return list.ToArray();
	}
}
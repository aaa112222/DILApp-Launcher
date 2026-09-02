namespace DILApp;

public class InstalledVersion
{
	public string Id { get; set; } = "";


	public string Type { get; set; } = "release";


	public string LoaderName { get; set; } = "";


	public bool HasJar { get; set; }

	public bool IsForge { get; set; }

	public bool IsFabric { get; set; }

	public bool IsQuilt { get; set; }

	public bool IsOptifine { get; set; }

	public long LastModified { get; set; }
}
using System.Collections.Generic;

namespace DILApp;

public class ModrinthDependency
{
	public string ProjectId { get; set; } = "";
	public string VersionId { get; set; } = "";
	public string DependencyType { get; set; } = "required";
	public string ProjectTitle { get; set; } = "";
}

public class ModrinthVersion
{
	public string Id { get; set; } = "";


	public string Name { get; set; } = "";


	public string VersionNumber { get; set; } = "";


	public string DatePublished { get; set; } = "";


	public string VersionType { get; set; } = "release";


	public List<string> GameVersions { get; set; } = new List<string>();


	public List<string> Loaders { get; set; } = new List<string>();


	public string DownloadUrl { get; set; } = "";


	public string FileName { get; set; } = "";


	public List<ModrinthDependency> Dependencies { get; set; } = new List<ModrinthDependency>();

}
using System.Collections.Generic;
using System.IO;
using NeoModLoader.api;

namespace NeoModLoader.utils;

public class ModDependencyNode
{
	public HashSet<ModDependencyNode> depend_by;

	public HashSet<ModDependencyNode> depend_on;

	public HashSet<ModDependencyNode> necessary_depend_on;

	public ModDeclare mod_decl { get; }

	public ModDependencyNode(ModDeclare pModDecl)
	{
		mod_decl = pModDecl;
		necessary_depend_on = new HashSet<ModDependencyNode>();
		depend_on = new HashSet<ModDependencyNode>();
		depend_by = new HashSet<ModDependencyNode>();
	}

	public List<string> GetAdditionReferences(bool recursive = true)
	{
		List<string> list = new List<string>();
		string path = Path.Combine(mod_decl.FolderPath, "Assemblies");
		if (Directory.Exists(path))
		{
			list.AddRange(Directory.GetFiles(path, "*.dll"));
		}
		if (recursive)
		{
			foreach (ModDependencyNode item in depend_on)
			{
				list.AddRange(item.GetAdditionReferences());
			}
		}
		return list;
	}
}

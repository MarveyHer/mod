using System.Collections.Generic;
using NeoModLoader.api;

namespace NeoModLoader.utils;

public class ModDependencyGraph
{
	public HashSet<ModDependencyNode> nodes;

	public ModDependencyGraph(ICollection<ModDeclare> mods)
	{
		Dictionary<string, ModDependencyNode> dictionary = new Dictionary<string, ModDependencyNode>();
		foreach (ModDeclare mod in mods)
		{
			dictionary.Add(mod.UID, new ModDependencyNode(mod));
		}
		foreach (ModDeclare mod2 in mods)
		{
			ModDependencyNode modDependencyNode = dictionary[mod2.UID];
			string[] dependencies = mod2.Dependencies;
			foreach (string key in dependencies)
			{
				if (dictionary.TryGetValue(key, out var value))
				{
					value.depend_by.Add(modDependencyNode);
					modDependencyNode.necessary_depend_on.Add(value);
				}
			}
			modDependencyNode.depend_on.UnionWith(modDependencyNode.necessary_depend_on);
			string[] optionalDependencies = mod2.OptionalDependencies;
			foreach (string key2 in optionalDependencies)
			{
				if (dictionary.TryGetValue(key2, out var value2))
				{
					value2.depend_by.Add(modDependencyNode);
					modDependencyNode.depend_on.Add(value2);
				}
			}
		}
		nodes = new HashSet<ModDependencyNode>();
		nodes.UnionWith(dictionary.Values);
		ModDependencyUtils.RemoveModsWithoutRequiredDependencies(this);
	}
}

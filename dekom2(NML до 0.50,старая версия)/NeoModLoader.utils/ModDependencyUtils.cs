using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoModLoader.api;
using NeoModLoader.services;

namespace NeoModLoader.utils;

internal static class ModDependencyUtils
{
	public static string ParseDepenNameToPreprocessSymbol(string pDepenName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in pDepenName)
		{
			stringBuilder.Append((!char.IsLetterOrDigit(c) && c <= 'Ā') ? '_' : char.ToUpper(c));
		}
		return stringBuilder.ToString();
	}

	public static ModDependencyNode TryToAppendMod(ModDependencyGraph pGraph, ModDeclare pModAppend)
	{
		bool flag = true;
		StringBuilder stringBuilder = new StringBuilder();
		if (pModAppend.IncompatibleWith != null && pModAppend.IncompatibleWith.Length != 0)
		{
			bool flag2 = false;
			foreach (ModDependencyNode node in pGraph.nodes)
			{
				if (pModAppend.IncompatibleWith.Contains(node.mod_decl.UID))
				{
					if (!flag2)
					{
						stringBuilder.AppendLine("Mod " + pModAppend.UID + " is incompatible with mods:");
						flag2 = true;
						flag = false;
					}
					stringBuilder.AppendLine("    " + node.mod_decl.UID);
				}
			}
		}
		ModDependencyNode modDependencyNode = new ModDependencyNode(pModAppend);
		bool flag3 = false;
		string[] dependencies = pModAppend.Dependencies;
		foreach (string dependency in dependencies)
		{
			try
			{
				ModDependencyNode modDependencyNode2 = pGraph.nodes.First((ModDependencyNode n) => n.mod_decl.UID == dependency);
				if (!flag3 && flag)
				{
					modDependencyNode.necessary_depend_on.Add(modDependencyNode2);
					modDependencyNode2.depend_by.Add(modDependencyNode);
				}
			}
			catch (InvalidOperationException)
			{
				if (!flag3)
				{
					stringBuilder.AppendLine("Mod " + pModAppend.UID + " has missing dependencies:");
					flag3 = true;
					flag = false;
				}
				else
				{
					stringBuilder.AppendLine("    " + dependency);
				}
			}
		}
		if (!flag)
		{
			LogService.LogError(stringBuilder.ToString());
			pModAppend.FailReason.AppendLine(stringBuilder.ToString());
			return null;
		}
		string[] optionalDependencies = pModAppend.OptionalDependencies;
		foreach (string text in optionalDependencies)
		{
			foreach (ModDependencyNode node2 in pGraph.nodes)
			{
				if (node2.mod_decl.UID == text)
				{
					modDependencyNode.depend_on.Add(node2);
					node2.depend_by.Add(modDependencyNode);
				}
			}
		}
		pGraph.nodes.Add(modDependencyNode);
		return modDependencyNode;
	}

	public static void RemoveCircleDependencies(ModDependencyGraph pGraph)
	{
	}

	public static void RemoveIncompatibleMods(ModDependencyGraph pGraph)
	{
		Queue<ModDependencyNode> queue = new Queue<ModDependencyNode>();
		foreach (ModDependencyNode node in pGraph.nodes)
		{
			queue.Enqueue(node);
		}
		while (queue.Count > 0)
		{
			ModDependencyNode modDependencyNode = queue.Dequeue();
			if (!pGraph.nodes.Contains(modDependencyNode) || modDependencyNode.mod_decl.IncompatibleWith.Length == 0)
			{
				continue;
			}
			foreach (ModDependencyNode item2 in modDependencyNode.depend_by)
			{
				queue.Enqueue(item2);
			}
			pGraph.nodes.Remove(modDependencyNode);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Mod " + modDependencyNode.mod_decl.UID + " is incompatible with mods:");
			string[] incompatibleWith = modDependencyNode.mod_decl.IncompatibleWith;
			foreach (string incompatible_with in incompatibleWith)
			{
				try
				{
					ModDependencyNode item = pGraph.nodes.First((ModDependencyNode node) => node.mod_decl.UID == incompatible_with);
					if (modDependencyNode.necessary_depend_on.Contains(item))
					{
						stringBuilder.AppendLine("    " + incompatible_with);
					}
				}
				catch (InvalidOperationException)
				{
					stringBuilder.AppendLine("    " + incompatible_with);
				}
			}
			modDependencyNode.mod_decl.FailReason.AppendLine(stringBuilder.ToString());
			LogService.LogWarning(stringBuilder.ToString());
		}
	}

	public static void RemoveModsWithoutRequiredDependencies(ModDependencyGraph pGraph)
	{
		Queue<ModDependencyNode> queue = new Queue<ModDependencyNode>();
		foreach (ModDependencyNode node in pGraph.nodes)
		{
			queue.Enqueue(node);
		}
		while (queue.Count > 0)
		{
			ModDependencyNode modDependencyNode = queue.Dequeue();
			if (!pGraph.nodes.Contains(modDependencyNode))
			{
				continue;
			}
			if (modDependencyNode.necessary_depend_on.Count < modDependencyNode.mod_decl.Dependencies.Length)
			{
				foreach (ModDependencyNode item3 in modDependencyNode.depend_by)
				{
					queue.Enqueue(item3);
				}
				foreach (ModDependencyNode item4 in modDependencyNode.depend_on)
				{
					item4.depend_by.Remove(modDependencyNode);
				}
				pGraph.nodes.Remove(modDependencyNode);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Mod " + modDependencyNode.mod_decl.UID + " has missing dependencies:");
				string[] dependencies = modDependencyNode.mod_decl.Dependencies;
				foreach (string dependency in dependencies)
				{
					try
					{
						ModDependencyNode item = pGraph.nodes.First((ModDependencyNode node) => node.mod_decl.UID == dependency);
						if (!modDependencyNode.necessary_depend_on.Contains(item))
						{
							stringBuilder.AppendLine("    " + dependency);
						}
					}
					catch (InvalidOperationException)
					{
						stringBuilder.AppendLine("    " + dependency);
					}
				}
				modDependencyNode.mod_decl.FailReason.AppendLine(stringBuilder.ToString());
				LogService.LogError(stringBuilder.ToString());
				continue;
			}
			string[] optionalDependencies = modDependencyNode.mod_decl.OptionalDependencies;
			foreach (string optional_dependency in optionalDependencies)
			{
				if (!pGraph.nodes.All((ModDependencyNode node) => node.mod_decl.UID != optional_dependency))
				{
					continue;
				}
				try
				{
					ModDependencyNode item2 = pGraph.nodes.First((ModDependencyNode node) => node.mod_decl.UID == optional_dependency);
					if (modDependencyNode.depend_on.Contains(item2))
					{
						modDependencyNode.depend_on.Remove(item2);
					}
				}
				catch (InvalidOperationException)
				{
				}
			}
		}
	}

	public static List<ModDependencyNode> SortModsCompileOrderFromDependencyTopology(ModDependencyGraph pGraph)
	{
		Dictionary<ModDependencyNode, int> dictionary = new Dictionary<ModDependencyNode, int>();
		Queue<ModDependencyNode> queue = new Queue<ModDependencyNode>();
		foreach (ModDependencyNode node in pGraph.nodes)
		{
			dictionary.Add(node, node.depend_on.Count);
			if (node.depend_on.Count == 0)
			{
				queue.Enqueue(node);
			}
		}
		List<ModDependencyNode> list = new List<ModDependencyNode>();
		while (queue.Count > 0)
		{
			ModDependencyNode modDependencyNode = queue.Dequeue();
			list.Add(modDependencyNode);
			foreach (ModDependencyNode item in modDependencyNode.depend_by)
			{
				try
				{
					dictionary[item]--;
					if (dictionary[item] == 0)
					{
						queue.Enqueue(item);
					}
				}
				catch (KeyNotFoundException)
				{
					LogService.LogError("Key " + item.mod_decl.UID + " not found in node_in_degree when checking " + modDependencyNode.mod_decl.UID);
				}
			}
		}
		return list;
	}
}

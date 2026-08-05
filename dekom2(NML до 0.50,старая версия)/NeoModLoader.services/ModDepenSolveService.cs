using System.Collections.Generic;
using NeoModLoader.api;
using NeoModLoader.utils;

namespace NeoModLoader.services;

internal static class ModDepenSolveService
{
	private static ModDependencyGraph graph;

	public static List<ModDependencyNode> SolveModDependencies(List<ModDeclare> mods)
	{
		graph = new ModDependencyGraph(mods);
		mods.Clear();
		ModDependencyUtils.RemoveCircleDependencies(graph);
		ModDependencyUtils.RemoveModsWithoutRequiredDependencies(graph);
		return ModDependencyUtils.SortModsCompileOrderFromDependencyTopology(graph);
	}

	public static ModDependencyNode SolveModDependencyRuntime(ModDeclare mod)
	{
		return ModDependencyUtils.TryToAppendMod(graph, mod);
	}
}

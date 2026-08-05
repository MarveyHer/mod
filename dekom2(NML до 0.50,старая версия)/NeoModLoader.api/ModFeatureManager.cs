using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace NeoModLoader.api;

public class ModFeatureManager<TMod> : IModFeatureManager, IStagedLoad where TMod : BasicMod<TMod>
{
	private class FeatureTreeNode
	{
		internal IModFeature ModFeature { get; }

		internal List<FeatureTreeNode> DependentFeatures { get; } = new List<FeatureTreeNode>();

		internal FeatureTreeNode(IModFeature modFeature)
		{
			ModFeature = modFeature;
		}

		internal static FeatureTreeNode[] CreateFeatureTrees(IModFeature[] features)
		{
			Dictionary<string, FeatureTreeNode> dictionary = new Dictionary<string, FeatureTreeNode>();
			List<FeatureTreeNode> list = new List<FeatureTreeNode>();
			foreach (IModFeature modFeature in features)
			{
				FeatureTreeNode featureTreeNode = new FeatureTreeNode(modFeature);
				dictionary.Add(modFeature.GetType().AssemblyQualifiedName ?? throw new Exception("AssemblyQualifiedName is null, apparently."), featureTreeNode);
				if (!modFeature.RequiredModFeatures.Concat(modFeature.OptionalModFeatures).Any())
				{
					list.Add(featureTreeNode);
				}
			}
			foreach (FeatureTreeNode value2 in dictionary.Values)
			{
				foreach (Type item in value2.ModFeature.RequiredModFeatures.Concat(value2.ModFeature.OptionalModFeatures))
				{
					if (dictionary.TryGetValue(item.AssemblyQualifiedName ?? throw new Exception("AssemblyQualifiedName is null, apparently."), out var value))
					{
						value.DependentFeatures.Add(value2);
					}
				}
			}
			return list.ToArray();
		}
	}

	private class FeatureLoadPathNode
	{
		private class PlaceholderRootModFeature : ModFeature
		{
			public override bool Init()
			{
				return true;
			}
		}

		internal IModFeature ModFeature { get; }

		internal FeatureLoadPathNode DependentFeature { get; private set; }

		internal FeatureLoadPathNode DependencyFeature { get; private set; }

		internal FeatureLoadPathNode(IModFeature modFeature)
		{
			ModFeature = modFeature;
		}

		[CanBeNull]
		internal static FeatureLoadPathNode CreateFeatureLoadPath(FeatureTreeNode[] featureTrees)
		{
			FeatureTreeNode featureTreeNode = new FeatureTreeNode(new PlaceholderRootModFeature());
			foreach (FeatureTreeNode item in featureTrees)
			{
				featureTreeNode.DependentFeatures.Add(item);
			}
			FeatureLoadPathNode featureLoadPathNode = new FeatureLoadPathNode(featureTreeNode.ModFeature);
			FeatureLoadPathNode featureLoadPathNode2 = featureLoadPathNode;
			List<FeatureTreeNode> list = new List<FeatureTreeNode>(featureTreeNode.DependentFeatures);
			while (list.Count > 0)
			{
				FeatureTreeNode featureTreeNode2 = list.Pop();
				for (FeatureLoadPathNode featureLoadPathNode3 = featureLoadPathNode2; featureLoadPathNode3 != null; featureLoadPathNode3 = featureLoadPathNode3.DependencyFeature)
				{
					if (featureLoadPathNode3.ModFeature == featureTreeNode2.ModFeature)
					{
						if (featureLoadPathNode3.DependentFeature != null)
						{
							featureLoadPathNode3.DependentFeature.DependencyFeature = featureLoadPathNode3.DependencyFeature;
						}
						if (featureLoadPathNode3.DependencyFeature != null)
						{
							featureLoadPathNode3.DependencyFeature.DependentFeature = featureLoadPathNode3.DependentFeature;
						}
					}
				}
				FeatureLoadPathNode featureLoadPathNode4 = (featureLoadPathNode2.DependentFeature = new FeatureLoadPathNode(featureTreeNode2.ModFeature));
				featureLoadPathNode4.DependencyFeature = featureLoadPathNode2;
				featureLoadPathNode2 = featureLoadPathNode4;
				list.AddRange(featureTreeNode2.DependentFeatures);
			}
			return featureLoadPathNode.DependentFeature;
		}
	}

	private readonly BasicMod<TMod> _mod;

	private readonly List<IModFeature> _foundFeatures = new List<IModFeature>();

	private FeatureLoadPathNode _featureLoadPath;

	private StackTrace _firstInstantiationStackTrace;

	private readonly List<IModFeature> _loadedFeatures = new List<IModFeature>();

	private StackTrace _firstLoadStackTrace;

	public ModFeatureManager(BasicMod<TMod> mod)
	{
		_mod = mod;
	}

	public bool IsFeatureLoaded<T>() where T : IModFeature
	{
		return IsFeatureLoaded(typeof(T));
	}

	private bool IsFeatureLoaded(Type featureType)
	{
		return _loadedFeatures.Any((IModFeature feature) => feature.GetType() == featureType);
	}

	public T GetFeature<T>(IModFeature askingModFeature) where T : IModFeature
	{
		if (!askingModFeature.RequiredModFeatures.Contains(typeof(T)))
		{
			throw new InvalidOperationException("Feature " + typeof(T).FullName + " is not set as a requirement for feature " + askingModFeature.GetType().FullName + ".");
		}
		if (!IsFeatureLoaded<T>())
		{
			throw new InvalidOperationException("Feature " + typeof(T).FullName + " is not loaded.");
		}
		return (T)GetFeature(typeof(T));
	}

	private IModFeature GetFeature(Type featureType)
	{
		return _foundFeatures.FirstOrDefault((IModFeature feature) => feature.GetType() == featureType);
	}

	public bool TryGetFeature<T>(IModFeature askingModFeature, out T feature) where T : IModFeature
	{
		if (!askingModFeature.RequiredModFeatures.Contains(typeof(T)) && !askingModFeature.OptionalModFeatures.Contains(typeof(T)))
		{
			throw new InvalidOperationException("Feature " + typeof(T).FullName + " is not set as a requirement or optional feature for feature " + askingModFeature.GetType().FullName + ".");
		}
		if (!IsFeatureLoaded<T>())
		{
			feature = default(T);
			return false;
		}
		feature = (T)GetFeature(typeof(T));
		return true;
	}

	public void InstantiateFeatures()
	{
		if (_featureLoadPath != null)
		{
			throw new InvalidOperationException($"Features have already been instantiated for this ModFeatureManager. Stack trace of first instantiation:\n{_firstInstantiationStackTrace}");
		}
		List<IModFeature> features = FindAndInstantiateModFeatures();
		_featureLoadPath = ParseModFeaturesIntoLoadPath(features);
		if (_foundFeatures.Count > 0)
		{
			_firstInstantiationStackTrace = new StackTrace();
		}
	}

	public void Init()
	{
		if (_loadedFeatures.Count > 0)
		{
			throw new InvalidOperationException($"Features have already been loaded for this ModFeatureManager. Stack trace of first load:\n{_firstLoadStackTrace}");
		}
		for (FeatureLoadPathNode featureLoadPathNode = _featureLoadPath; featureLoadPathNode != null; featureLoadPathNode = featureLoadPathNode.DependentFeature)
		{
			InitFeature(featureLoadPathNode.ModFeature);
		}
		if (_loadedFeatures.Count > 0)
		{
			_firstLoadStackTrace = new StackTrace();
		}
	}

	public void PostInit()
	{
		for (FeatureLoadPathNode featureLoadPathNode = _featureLoadPath; featureLoadPathNode != null; featureLoadPathNode = featureLoadPathNode.DependentFeature)
		{
			SafePerformActionOnFeature(featureLoadPathNode.ModFeature, "Post-Loading", (IModFeature feature) => feature.PostInit());
		}
	}

	private static FeatureLoadPathNode ParseModFeaturesIntoLoadPath(List<IModFeature> features)
	{
		FeatureTreeNode[] featureTrees = FeatureTreeNode.CreateFeatureTrees(features.ToArray());
		return FeatureLoadPathNode.CreateFeatureLoadPath(featureTrees);
	}

	private List<IModFeature> FindAndInstantiateModFeatures()
	{
		List<IModFeature> list = new List<IModFeature>();
		foreach (var (featureType, instanceConstructor) in from type in ((object)_mod).GetType().Assembly.Modules.SelectMany((Module m) => m.GetTypes())
			where typeof(IModFeature).IsAssignableFrom(type)
			where !type.IsAbstract
			where !type.IsNestedPrivate
			select (featureType: type, type.GetConstructors().FirstOrDefault((ConstructorInfo constructor) => constructor.GetParameters().Length < 1)))
		{
			InstantiateModFeature(featureType, instanceConstructor, list);
		}
		_foundFeatures.AddRange(list);
		return list;
	}

	private void InstantiateModFeature(Type featureType, ConstructorInfo instanceConstructor, List<IModFeature> features)
	{
		BasicMod<TMod>.LogInfo("Creating instance of Feature " + featureType.FullName + "...");
		if ((object)instanceConstructor == null)
		{
			BasicMod<TMod>.LogError("No suitable constructor found for Feature " + featureType.FullName + ".");
			return;
		}
		IModFeature modFeature;
		try
		{
			modFeature = instanceConstructor.Invoke(new object[0]) as IModFeature;
		}
		catch (Exception arg)
		{
			BasicMod<TMod>.LogError($"An error occurred while trying to create an instance of Feature {featureType.FullName}:\n{arg}");
			return;
		}
		if (modFeature == null)
		{
			BasicMod<TMod>.LogError("Failed to create instance of Feature " + featureType.FullName + " for unknown reasons.");
			return;
		}
		modFeature.ModFeatureManager = this;
		List<Type> list = modFeature.RequiredModFeatures.Where((Type requiredFeature) => !typeof(IModFeature).IsAssignableFrom(requiredFeature)).ToList();
		if (list.Any())
		{
			throw new InvalidOperationException("Feature " + featureType.FullName + " has required features that are not a subclass of IModFeature:\n" + string.Join("\n", list.Select((Type type) => type.FullName)));
		}
		List<Type> list2 = modFeature.OptionalModFeatures.Where((Type optionalFeature) => !typeof(IModFeature).IsAssignableFrom(optionalFeature)).ToList();
		if (list2.Any())
		{
			throw new InvalidOperationException("Feature " + featureType.FullName + " has optional features that are not a subclass of IModFeature:\n" + string.Join("\n", list2.Select((Type type) => type.FullName)));
		}
		features.Add(modFeature);
		BasicMod<TMod>.LogInfo("Successfully created instance of Feature " + featureType.FullName + ".");
	}

	private void InitFeature(IModFeature modFeature)
	{
		SafePerformActionOnFeature(modFeature, "Loading", delegate(IModFeature feature)
		{
			bool flag = feature.Init();
			if (flag)
			{
				_loadedFeatures.Add(modFeature);
			}
			return flag;
		});
	}

	private void SafePerformActionOnFeature(IModFeature modFeature, string actionVerb, Func<IModFeature, bool> performAction, bool log = true)
	{
		if (log)
		{
			BasicMod<TMod>.LogInfo(actionVerb + " feature " + modFeature.GetType().FullName + "...");
		}
		try
		{
			List<Type> list = modFeature.RequiredModFeatures.Where((Type requiredFeature) => !IsFeatureLoaded(requiredFeature)).ToList();
			if (list.Count > 0)
			{
				if (log)
				{
					BasicMod<TMod>.LogError(actionVerb + " feature " + modFeature.GetType().FullName + " failed due missing requirement features:\n" + string.Join("\n", list.Select((Type type) => type.FullName)));
				}
			}
			else if (!performAction(modFeature))
			{
				if (log)
				{
					BasicMod<TMod>.LogError(actionVerb + " feature " + modFeature.GetType().FullName + " failed due to a failing condition.");
				}
			}
			else if (log)
			{
				BasicMod<TMod>.LogInfo(actionVerb + " feature " + modFeature.GetType().FullName + " succeeded.");
			}
		}
		catch (Exception arg)
		{
			if (log)
			{
				BasicMod<TMod>.LogError($"{actionVerb} feature {modFeature.GetType().FullName} caused an error:\n{arg}");
			}
		}
	}
}

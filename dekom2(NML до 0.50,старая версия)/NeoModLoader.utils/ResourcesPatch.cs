using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.api.exceptions;
using NeoModLoader.services;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.U2D;

namespace NeoModLoader.utils;

public static class ResourcesPatch
{
	private class ResourceTree
	{
		internal Dictionary<string, Object> direct_objects = new Dictionary<string, Object>();

		private ResourceTreeNode root = new ResourceTreeNode(null);

		public ResourceTree()
		{
			root.parent = root;
		}

		public ResourceTreeNode Find(string path, bool createNodeAlong = false, bool visitLast = true)
		{
			path = path.ToLower();
			string[] array = ((!path.EndsWith("/")) ? path.Split('/') : path.Substring(0, path.Length - 1).Split('/'));
			ResourceTreeNode resourceTreeNode = root;
			for (int i = 0; i < array.Length - ((!visitLast) ? 1 : 0); i++)
			{
				string text = array[i];
				if (text == "..")
				{
					resourceTreeNode = resourceTreeNode.parent;
				}
				else
				{
					if (text == ".")
					{
						continue;
					}
					if (!resourceTreeNode.children.ContainsKey(text))
					{
						if (!createNodeAlong)
						{
							return null;
						}
						resourceTreeNode.children[text] = new ResourceTreeNode(resourceTreeNode);
					}
					resourceTreeNode = resourceTreeNode.children[text];
				}
			}
			return resourceTreeNode;
		}

		public Object Get(string path)
		{
			if (direct_objects.TryGetValue(path.ToLower(), out var value))
			{
				return value;
			}
			ResourceTreeNode resourceTreeNode = Find(path, createNodeAlong: true, visitLast: false);
			if (resourceTreeNode == null)
			{
				return null;
			}
			if (resourceTreeNode.objects.TryGetValue(Path.GetFileNameWithoutExtension(path.ToLower()), out value))
			{
				direct_objects[path] = value;
				return value;
			}
			return null;
		}

		public void Add(string path, Object obj)
		{
			string text = path.ToLower();
			direct_objects[text] = obj;
			ResourceTreeNode resourceTreeNode = Find(path, createNodeAlong: true, visitLast: false);
			resourceTreeNode.objects[Path.GetFileNameWithoutExtension(text)] = obj;
		}

		public void AddFromFile(string path, string absPath)
		{
			string text = path.ToLower();
			if (text.EndsWith(".meta") || text.EndsWith("sprites.json"))
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(text);
			Object[] array;
			try
			{
				string pLowerPath = absPath.ToLower();
				array = LoadResourceFile(ref absPath, ref pLowerPath);
				Object[] array2 = array;
				foreach (Object val in array2)
				{
					if (directoryName == null)
					{
						direct_objects[val.name] = val;
					}
					else
					{
						direct_objects[Path.Combine(directoryName, val.name).Replace('\\', '/').ToLower()] = val;
					}
				}
			}
			catch (UnrecognizableResourceFileException)
			{
				LogService.LogWarning("Cannot recognize resource file " + path);
				return;
			}
			if (array.Length != 0)
			{
				ResourceTreeNode resourceTreeNode = Find(path, createNodeAlong: true, visitLast: false);
				Object[] array3 = array;
				foreach (Object val2 in array3)
				{
					resourceTreeNode.objects[val2.name.ToLower()] = val2;
				}
			}
		}
	}

	private class ResourceTreeNode
	{
		public readonly Dictionary<string, ResourceTreeNode> children = new Dictionary<string, ResourceTreeNode>();

		public readonly Dictionary<string, Object> objects = new Dictionary<string, Object>();

		public ResourceTreeNode parent { get; internal set; }

		public ResourceTreeNode(ResourceTreeNode parent)
		{
			this.parent = parent;
		}

		public List<Object> GetAllObjects(Type systemTypeInstance)
		{
			List<Object> list = new List<Object>(objects.Count);
			Queue<ResourceTreeNode> queue = new Queue<ResourceTreeNode>(children.Count);
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				ResourceTreeNode resourceTreeNode = queue.Dequeue();
				foreach (Object value in resourceTreeNode.objects.Values)
				{
					if (systemTypeInstance.IsInstanceOfType(value))
					{
						list.Add(value);
					}
				}
				foreach (ResourceTreeNode value2 in resourceTreeNode.children.Values)
				{
					queue.Enqueue(value2);
				}
			}
			return list;
		}
	}

	private static ResourceTree tree;

	public static Dictionary<string, Object> GetAllPatchedResources()
	{
		return tree.direct_objects;
	}

	public static void PatchResource(string pPath, Object pObject)
	{
		tree.Add(pPath, pObject);
	}

	internal static void Initialize()
	{
		CustomAudioManager.Initialize();
		tree = new ResourceTree();
		SpriteAtlas val = Resources.FindObjectsOfTypeAll<SpriteAtlas>().FirstOrDefault((SpriteAtlas x) => ((Object)x).name == "SpriteAtlasUI");
		Sprite[] array = (Sprite[])(object)new Sprite[val.spriteCount];
		val.GetSprites(array);
		Sprite[] array2 = array;
		foreach (Sprite val2 in array2)
		{
			((Object)val2).name = ((Object)val2).name.Replace("(Clone)", "");
			tree.Add("ui/special/" + ((Object)val2).name, (Object)(object)val2);
		}
		MethodInfo[] methods = typeof(InternalResourcesGetter).GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.ReturnType != typeof(Sprite)) && methodInfo.GetParameters().Length == 0)
			{
				methodInfo.Invoke(null, null);
			}
		}
	}

	internal static void PatchSomeResources()
	{
		Sprite[] array = Resources.LoadAll<Sprite>("actors/races/items");
		Sprite[] array2 = array;
		foreach (Sprite val in array2)
		{
			if (!ActorAnimationLoader._dict_items.ContainsKey(((Object)val).name))
			{
				ActorAnimationLoader._dict_items[((Object)val).name] = new List<Sprite> { val };
			}
		}
	}

	public static Object[] LoadResourceFile(ref string path, ref string pLowerPath)
	{
		if (pLowerPath.EndsWith(".png") || pLowerPath.EndsWith(".jpg") || pLowerPath.EndsWith(".jpeg"))
		{
			return (Object[])(object)SpriteLoadUtils.LoadSprites(path);
		}
		if (pLowerPath.EndsWith(".wav"))
		{
			LoadWavFile(path);
		}
		return (Object[])(object)new Object[1] { (Object)LoadTextAsset(path) };
	}

	private static void LoadWavFile(string path)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
		if (CustomAudioManager.AudioWavLibrary.ContainsKey(fileNameWithoutExtension))
		{
			LogService.LogError("The Sound file " + fileNameWithoutExtension + " has already been loaded!");
			return;
		}
		WavContainer value;
		try
		{
			value = JsonConvert.DeserializeObject<WavContainer>(File.ReadAllText(Path.GetDirectoryName(path) + "/" + fileNameWithoutExtension + ".json"));
			value.Path = path;
		}
		catch (Exception)
		{
			value = new WavContainer(path, _3D: true, 50f);
		}
		CustomAudioManager.AudioWavLibrary.Add(fileNameWithoutExtension, value);
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(MusicBox), "playSound", new Type[]
	{
		typeof(string),
		typeof(float),
		typeof(float),
		typeof(bool),
		typeof(bool)
	})]
	private static bool LoadCustomSound(string pSoundPath, float pX, float pY, bool pGameViewOnly)
	{
		if (!MusicBox.sounds_on)
		{
			return false;
		}
		if (pGameViewOnly && World.world.quality_changer.isLowRes())
		{
			return false;
		}
		if (!CustomAudioManager.AudioWavLibrary.ContainsKey(pSoundPath))
		{
			return true;
		}
		CustomAudioManager.LoadCustomSound(pX, pY, pSoundPath);
		return false;
	}

	private static TextAsset LoadTextAsset(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		TextAsset val = new TextAsset(File.ReadAllText(path));
		((Object)val).name = Path.GetFileNameWithoutExtension(path);
		return val;
	}

	internal static void LoadResourceFromFolder(string pFolder)
	{
		if (!Directory.Exists(pFolder))
		{
			return;
		}
		List<string> list = SystemUtils.SearchFileRecursive(pFolder, (string filename) => !filename.StartsWith("."), (string dirname) => !dirname.StartsWith("."));
		foreach (string item in list)
		{
			tree.AddFromFile(item.Replace(pFolder, "").Replace('\\', '/').Substring(1), item);
		}
	}

	internal static void LoadAssetBundlesFromFolder(string pFolder)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected I4, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		if (!Directory.Exists(pFolder))
		{
			return;
		}
		RuntimePlatform platform = Application.platform;
		if (1 == 0)
		{
		}
		string text;
		if ((int)platform <= 7)
		{
			switch ((int)platform)
			{
			case 2:
				goto IL_004f;
			case 1:
				goto IL_005f;
			case 0:
				goto IL_0067;
			}
			if ((int)platform != 7)
			{
				goto IL_007f;
			}
			text = "win";
		}
		else if ((int)platform != 13)
		{
			if ((int)platform != 16)
			{
				goto IL_007f;
			}
			text = "linux";
		}
		else
		{
			text = "linux";
		}
		goto IL_0087;
		IL_0087:
		if (1 == 0)
		{
		}
		string path = text;
		string text2 = Path.Combine(pFolder, path);
		if (Directory.Exists(text2))
		{
			AssetBundleUtils.LoadFromFolder(text2);
		}
		return;
		IL_0067:
		text = "osx";
		goto IL_0087;
		IL_005f:
		text = "osx";
		goto IL_0087;
		IL_007f:
		text = "win";
		goto IL_0087;
		IL_004f:
		text = "win";
		goto IL_0087;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Resources), "LoadAll", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	private static void LoadAll_Prefix(ref string path)
	{
		if (!path.Contains(".."))
		{
			return;
		}
		string[] array = path.Split('/');
		List<string> list = new List<string>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == ".." && list.Count > 0)
			{
				list.RemoveAt(list.Count - 1);
			}
			else
			{
				list.Add(array[i]);
			}
		}
		path = string.Join("/", list);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Resources), "LoadAll", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	private static Object[] LoadAll_Postfix(Object[] __result, string path, Type systemTypeInstance)
	{
		if (tree == null)
		{
			return __result;
		}
		ResourceTreeNode resourceTreeNode = tree.Find(path);
		if (resourceTreeNode == null)
		{
			return __result;
		}
		List<Object> allObjects = resourceTreeNode.GetAllObjects(systemTypeInstance);
		if (allObjects.Count == 0)
		{
			return __result;
		}
		List<Object> list = new List<Object>(__result);
		HashSet<string> names = new HashSet<string>(allObjects.Select((Object x) => x.name));
		list.RemoveAll((Object x) => names.Contains(x.name));
		list.AddRange(allObjects);
		return list.ToArray();
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	private static void Load_Prefix(ref string path)
	{
		if (!path.Contains(".."))
		{
			return;
		}
		string[] array = path.Split('/');
		List<string> list = new List<string>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == ".." && list.Count > 0)
			{
				list.RemoveAt(list.Count - 1);
			}
			else
			{
				list.Add(array[i]);
			}
		}
		path = string.Join("/", list);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Resources), "Load", new Type[]
	{
		typeof(string),
		typeof(Type)
	})]
	private static Object Load_Postfix(Object __result, string path, Type systemTypeInstance)
	{
		if (tree == null)
		{
			return __result;
		}
		Object val = tree.Get(path);
		if (val != (Object)null && systemTypeInstance.IsInstanceOfType(val))
		{
			return val;
		}
		return __result;
	}
}

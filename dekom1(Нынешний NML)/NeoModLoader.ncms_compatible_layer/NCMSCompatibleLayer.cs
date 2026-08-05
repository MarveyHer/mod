using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NCMS;
using NCMS.Utils;
using NeoModLoader.api;
using NeoModLoader.services;
using NeoModLoader.utils;

namespace NeoModLoader.ncms_compatible_layer;

internal static class NCMSCompatibleLayer
{
	public const string modGlobalObject = "\r\n    using System;\r\n    using System.IO;\r\n    using System.Reflection;\r\n    using UnityEngine;\r\n    using UnityEngine.Events;\r\n    using UnityEngine.UI;\r\n    using NeoModLoader.services;\r\n    using System.Collections.Generic;\r\n\r\n\r\n    internal class Mod\r\n    {\r\n        public static ModDeclaration.Info Info;\r\n        public static GameObject GameObject;\r\n        public static Action OnDebug;\r\n\r\n        private static int debugClicked = 0;\r\n\r\n        public static void Initialize(Button button)\r\n        {\r\n            OnDebug += new Action(() => { LogService.LogInfo($\"Debug toggled for mod {Info.Name}\"); });\r\n\r\n            button.onClick.AddListener(new UnityAction(() =>\r\n            {\r\n                if (debugClicked < 10)\r\n                {\r\n                    debugClicked++;\r\n                    return;\r\n                }\r\n\r\n                OnDebug();\r\n            }));\r\n        }\r\n\r\n        public class EmbededResources\r\n        {\r\n            private static Assembly this_assembly = Assembly.GetExecutingAssembly();\r\n\r\n            public static Sprite LoadSprite(string name, float pivotX = 0, float pivotY = 0, float pixelsPerUnit = 1f)\r\n            {\r\n                string hash = $\"{name}-{pivotX}-{pivotY}-{pixelsPerUnit}\";\r\n                if (sprite_cache.TryGetValue(hash, out var sprite))\r\n                    return sprite;\r\n                Texture2D texture2D = new Texture2D(0, 0);\r\n                texture2D.LoadImage(GetBytes(name));\r\n                texture2D.anisoLevel = 0;\r\n                texture2D.filterMode = FilterMode.Point;\r\n                sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, (float)texture2D.width, (float)texture2D.height),\r\n                    new Vector2(pivotX, pivotY), pixelsPerUnit);\r\n                sprite_cache.Add(hash, sprite);\r\n                return sprite;\r\n            }\r\n\r\n            private static Dictionary<string, Sprite> sprite_cache = new();\r\n\r\n            public static byte[] GetBytes(string name)\r\n            {\r\n                return ReadFully(this_assembly.GetManifestResourceStream(name));\r\n            }\r\n\r\n            internal static byte[] ReadFully(Stream input)\r\n            {\r\n                using var ms = new MemoryStream();\r\n                input.CopyTo(ms);\r\n                return ms.ToArray();\r\n            }\r\n        }\r\n    }";

	public static void PreInit()
	{
		NCMS.Utils.Windows.init();
		if (NCMS.Utils.ResourcesPatch.modsResources == null)
		{
			NCMS.Utils.ResourcesPatch.modsResources = NeoModLoader.utils.ResourcesPatch.GetAllPatchedResources();
		}
	}

	public static void Init()
	{
		if (NCMS.ModLoader.Mods == null)
		{
			NCMS.ModLoader.Mods = new List<NCMod>();
		}
		foreach (IMod loadedMod in WorldBoxMod.LoadedMods)
		{
			ModDeclare declaration = loadedMod.GetDeclaration();
			NCMS.ModLoader.Mods.Add(GenerateNCMSMod(declaration));
		}
		LogService.LogInfo("NCMS Compatible Layer has been initialized.");
	}

	public static NCMod GenerateNCMSMod(ModDeclare modDeclare)
	{
		return new NCMod
		{
			author = modDeclare.Author,
			description = modDeclare.Description,
			iconPath = modDeclare.IconPath,
			name = modDeclare.Name,
			path = modDeclare.FolderPath,
			version = modDeclare.Version,
			targetGameBuild = modDeclare.TargetGameBuild
		};
	}

	public static bool IsNCMSMod(SyntaxTree syntaxTree)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		CompilationUnitSyntax compilationUnitRoot = CSharpExtensions.GetCompilationUnitRoot(syntaxTree, default(CancellationToken));
		foreach (SyntaxNode item in ((SyntaxNode)compilationUnitRoot).DescendantNodes((Func<SyntaxNode, bool>)null, false))
		{
			ClassDeclarationSyntax val = (ClassDeclarationSyntax)(object)((item is ClassDeclarationSyntax) ? item : null);
			if (val == null || !((IEnumerable<AttributeListSyntax>)(object)((MemberDeclarationSyntax)val).AttributeLists).Any(delegate(AttributeListSyntax a)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((IEnumerable<AttributeSyntax>)(object)a.Attributes).Any((AttributeSyntax val2) => ((object)val2.Name).ToString().Contains("ModEntry"));
			}))
			{
				continue;
			}
			return true;
		}
		return false;
	}
}

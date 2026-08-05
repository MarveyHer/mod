using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using HarmonyLib.Tools;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Collections.Generic;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using NeoModLoader.api;
using NeoModLoader.api.attributes;
using NeoModLoader.constants;
using NeoModLoader.services;

namespace NeoModLoader.utils;

internal static class ModReloadUtils
{
	private static IReloadable _mod;

	private static ModDeclare _mod_declare;

	private static string _new_compiled_dll_path;

	private static string _new_compiled_pdb_path;

	private static AssemblyDefinition _old_assembly_definition;

	private static Dictionary<string, MethodDefinition> _old_method_definitions = new Dictionary<string, MethodDefinition>();

	private static Dictionary<OpCode, OpCode> _op_code_map = new Dictionary<OpCode, OpCode>();

	private static Dictionary<MethodDefinition, MethodInfo> _regenerated_brand_new_methods = new Dictionary<MethodDefinition, MethodInfo>();

	private static Dictionary<Type, MethodInfo> _emit_method_cache = new Dictionary<Type, MethodInfo>();

	private static readonly Dictionary<MethodInfo, ILHook> _create_hooks = new Dictionary<MethodInfo, ILHook>();

	public static bool Prepare(IReloadable pMod, ModDeclare pModDeclare)
	{
		_mod = pMod;
		_mod_declare = pModDeclare;
		_new_compiled_dll_path = Path.Combine(Paths.CompiledModsPath, _mod_declare.UID + ".dll");
		_new_compiled_pdb_path = Path.Combine(Paths.CompiledModsPath, _mod_declare.UID + ".pdb");
		try
		{
			_old_assembly_definition.Dispose();
			_old_assembly_definition = null;
			_old_method_definitions.Clear();
		}
		catch (Exception)
		{
		}
		if (!File.Exists(_new_compiled_dll_path))
		{
			LogService.LogError("No compiled dll found for mod " + _mod_declare.UID);
			return false;
		}
		if (File.Exists(_new_compiled_pdb_path + ".bak"))
		{
			File.Delete(_new_compiled_pdb_path + ".bak");
		}
		File.Copy(_new_compiled_dll_path, _new_compiled_dll_path + ".bak", overwrite: true);
		_old_assembly_definition = AssemblyDefinition.ReadAssembly(_new_compiled_dll_path + ".bak");
		return true;
	}

	public static bool CompileNew()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!ModCompileLoadService.TryCompileModAtRuntime(_mod_declare, pForce: true))
		{
			return false;
		}
		Enumerator<TypeDefinition> enumerator = _old_assembly_definition.MainModule.Types.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TypeDefinition current = enumerator.Current;
				Enumerator<MethodDefinition> enumerator2 = current.Methods.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						MethodDefinition current2 = enumerator2.Current;
						_old_method_definitions[((MemberReference)current2).FullName] = current2;
					}
				}
				finally
				{
					((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
				}
				foreach (MethodDefinition item in ((IEnumerable<TypeDefinition>)current.NestedTypes).SelectMany((TypeDefinition nested_type) => (IEnumerable<MethodDefinition>)nested_type.Methods))
				{
					_old_method_definitions[((MemberReference)item).FullName] = item;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return true;
	}

	public static bool PatchHotfixMethods()
	{
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		HarmonyFileLog.Enabled = true;
		AssemblyDefinition val = AssemblyDefinition.ReadAssembly(_new_compiled_dll_path);
		List<MethodDefinition> list = new List<MethodDefinition>();
		list.AddRange(((IEnumerable<TypeDefinition>)val.MainModule.Types).SelectMany((TypeDefinition type) => (IEnumerable<MethodDefinition>)type.Methods));
		foreach (TypeDefinition item in ((IEnumerable<TypeDefinition>)val.MainModule.Types).SelectMany((TypeDefinition type) => (IEnumerable<TypeDefinition>)type.NestedTypes))
		{
			list.AddRange((IEnumerable<MethodDefinition>)item.Methods);
		}
		Assembly assembly = _mod.GetType().Assembly;
		Harmony pHarmony = new Harmony(_mod_declare.UID);
		if (_op_code_map.Count == 0)
		{
			InitializeOpcodeMap();
		}
		HashSet<MethodDefinition> hashSet = new HashSet<MethodDefinition>();
		foreach (MethodDefinition item2 in list)
		{
			if (!item2.HasBody)
			{
				continue;
			}
			bool flag = false;
			Enumerator<CustomAttribute> enumerator3 = item2.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					CustomAttribute current3 = enumerator3.Current;
					if (((MemberReference)current3.AttributeType).FullName == typeof(HotfixableAttribute).FullName)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
			}
			if (flag)
			{
				MethodInfo method = assembly.GetType(((MemberReference)item2.DeclaringType).FullName).GetMethod(((MemberReference)item2).Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ((IEnumerable<ParameterDefinition>)((MethodReference)item2).Parameters).Select((ParameterDefinition x) => ReflectionHelper.ResolveReflection(((ParameterReference)x).ParameterType)).ToArray(), null);
				if (!(method != null))
				{
					LogService.LogWarning("No found method " + ((MemberReference)item2.DeclaringType).FullName + "::" + ((MemberReference)item2).Name + " in old assembly");
					hashSet.Add(item2);
				}
			}
		}
		if (hashSet.Count > 0)
		{
			CreateBrandNewMethods(hashSet);
		}
		foreach (MethodDefinition item3 in list)
		{
			if (!item3.HasBody)
			{
				continue;
			}
			bool flag2 = false;
			Enumerator<CustomAttribute> enumerator5 = item3.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator5.MoveNext())
				{
					CustomAttribute current5 = enumerator5.Current;
					if (((MemberReference)current5.AttributeType).FullName == typeof(HotfixableAttribute).FullName)
					{
						flag2 = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator5/*cast due to constrained. prefix*/).Dispose();
			}
			if (!flag2 || hashSet.Contains(item3))
			{
				continue;
			}
			try
			{
				MethodInfo method2 = assembly.GetType(((MemberReference)item3.DeclaringType).FullName).GetMethod(((MemberReference)item3).Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ((IEnumerable<ParameterDefinition>)((MethodReference)item3).Parameters).Select((ParameterDefinition x) => ReflectionHelper.ResolveReflection(((ParameterReference)x).ParameterType)).ToArray(), null);
				if (!(method2 == null))
				{
					if (!NeedHotfix(method2, item3))
					{
						LogService.LogInfo("Method " + ((MemberReference)item3).Name + " does not need hotfix");
						continue;
					}
					LogService.LogInfo($"Hotfixing method {((MemberReference)item3).Name} with following instructions(total {item3.Body.Instructions.Count}):");
					HotfixMethod(pHarmony, item3, method2);
				}
			}
			catch (Exception ex)
			{
				LogService.LogError("Failed to hotfix method " + ((MemberReference)item3).Name + ", Most likely because NeoModLoader does not support such method hotfix now.");
				LogService.LogError(ex.Message);
				LogService.LogError(ex.StackTrace);
			}
		}
		val.Dispose();
		return true;
	}

	private static void CreateBrandNewMethods(HashSet<MethodDefinition> pBrandNewMethods)
	{
		LogService.LogWarning($"Find {pBrandNewMethods.Count} brand new methods, creating...");
		int count = pBrandNewMethods.Count;
		HashSet<MethodDefinition> hashSet = new HashSet<MethodDefinition>(pBrandNewMethods);
		while (count-- > 0)
		{
			foreach (MethodDefinition item in hashSet)
			{
				try
				{
					DynamicMethodDefinition val = regenerate(item);
					MethodInfo value = val.Generate();
					_regenerated_brand_new_methods[item] = value;
				}
				catch (Exception ex)
				{
					LogService.LogError("Failed to create brand new method " + ((MemberReference)item).FullName);
					LogService.LogError(ex.Message);
					LogService.LogError(ex.StackTrace);
					continue;
				}
				pBrandNewMethods.Remove(item);
			}
		}
	}

	private static bool NeedHotfix(MethodInfo pOldMethod, MethodDefinition pNewMethod)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (!_old_method_definitions.TryGetValue(((MemberReference)pNewMethod).FullName, out var value))
		{
			LogService.LogWarning("No found method " + ((MemberReference)pNewMethod).FullName + " in old assembly");
			return true;
		}
		Collection<Instruction> instructions = value.Body.Instructions;
		Collection<Instruction> instructions2 = pNewMethod.Body.Instructions;
		if (instructions.Count != instructions2.Count)
		{
			return true;
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		Enumerator<Instruction> enumerator = instructions.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Instruction current = enumerator.Current;
				object operand = current.Operand;
				Instruction val = (Instruction)((operand is Instruction) ? operand : null);
				if (val != null)
				{
					stringBuilder.AppendLine($"{current.OpCode} {val.Offset - current.Offset}");
				}
				else
				{
					stringBuilder.AppendLine(((object)current).ToString().Substring("IL_0000: ".Length));
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		Enumerator<Instruction> enumerator2 = instructions2.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				Instruction current2 = enumerator2.Current;
				object operand2 = current2.Operand;
				Instruction val2 = (Instruction)((operand2 is Instruction) ? operand2 : null);
				if (val2 != null)
				{
					stringBuilder2.AppendLine($"{current2.OpCode} {val2.Offset - current2.Offset}");
				}
				else
				{
					stringBuilder2.AppendLine(((object)current2).ToString().Substring("IL_0000: ".Length));
				}
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		return stringBuilder.ToString().GetHashCode() != stringBuilder2.ToString().GetHashCode();
	}

	private static void InitializeOpcodeMap()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		FieldInfo[] fields = typeof(OpCodes).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType != typeof(OpCode)))
			{
				OpCode value = (OpCode)fieldInfo.GetValue(null);
				try
				{
					_op_code_map.Add((OpCode)typeof(OpCodes).GetField(fieldInfo.Name).GetValue(null), value);
				}
				catch (Exception)
				{
				}
			}
		}
		_op_code_map.Add(OpCodes.Stelem_Any, OpCodes.Stelem);
		_op_code_map.Add(OpCodes.Ldelem_Any, OpCodes.Ldelem);
		_op_code_map.Add(OpCodes.Tail, OpCodes.Tailcall);
	}

	private static void HotfixMethod(Harmony pHarmony, MethodDefinition pNewMethod, MethodInfo pOldMethod)
	{
		ReplaceMethod(pOldMethod, regenerate(pNewMethod));
	}

	public static bool PatchHotfixMethodsNT()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_004c: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		byte[] buffer = File.ReadAllBytes(_new_compiled_dll_path);
		byte[] buffer2 = File.ReadAllBytes(_new_compiled_pdb_path);
		using MemoryStream memoryStream = new MemoryStream(buffer);
		using MemoryStream symbolStream = new MemoryStream(buffer2);
		AssemblyDefinition val = AssemblyDefinition.ReadAssembly((Stream)memoryStream, new ReaderParameters
		{
			ReadSymbols = true,
			SymbolStream = symbolStream,
			SymbolReaderProvider = (ISymbolReaderProvider)new PdbReaderProvider()
		});
		List<MethodDefinition> list = new List<MethodDefinition>();
		list.AddRange(((IEnumerable<TypeDefinition>)val.MainModule.Types).SelectMany((TypeDefinition val3) => (IEnumerable<MethodDefinition>)val3.Methods));
		foreach (TypeDefinition item in ((IEnumerable<TypeDefinition>)val.MainModule.Types).SelectMany((TypeDefinition val3) => (IEnumerable<TypeDefinition>)val3.NestedTypes))
		{
			list.AddRange((IEnumerable<MethodDefinition>)item.Methods);
		}
		HashSet<MethodDefinition> hashSet = new HashSet<MethodDefinition>();
		List<(MethodInfo, MethodDefinition)> list2 = new List<(MethodInfo, MethodDefinition)>();
		foreach (MethodDefinition item2 in list)
		{
			if (!item2.HasBody)
			{
				continue;
			}
			bool flag = false;
			Enumerator<CustomAttribute> enumerator3 = item2.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					CustomAttribute current3 = enumerator3.Current;
					if (((MemberReference)current3.AttributeType).FullName == typeof(HotfixableAttribute).FullName)
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
			}
			if (!flag)
			{
				continue;
			}
			Type type = AccessTools.TypeByName(((MemberReference)item2.DeclaringType).FullName);
			if (!(type == null))
			{
				MethodInfo method = type.GetMethod(((MemberReference)item2).Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ((IEnumerable<ParameterDefinition>)((MethodReference)item2).Parameters).Select((ParameterDefinition x) => ReflectionHelper.ResolveReflection(((ParameterReference)x).ParameterType)).ToArray(), null);
				if (!(method == null))
				{
					list2.Add((method, item2));
				}
			}
		}
		while (hashSet.Count > 0)
		{
			HashSet<MethodDefinition> hashSet2 = new HashSet<MethodDefinition>();
			foreach (MethodDefinition item3 in hashSet)
			{
				try
				{
					_regenerated_brand_new_methods[item3] = CreateMethod(item3);
				}
				catch (Exception ex)
				{
					LogService.LogError("Failed to create brand new method " + ((MemberReference)item3).FullName);
					LogService.LogError(ex.Message);
					LogService.LogError(ex.StackTrace);
					continue;
				}
				hashSet2.Add(item3);
			}
			if (hashSet2.Count == 0)
			{
				break;
			}
			hashSet.ExceptWith(hashSet2);
		}
		foreach (var (oldMethod, val2) in list2)
		{
			try
			{
				Replace(oldMethod, val2);
			}
			catch (Exception ex2)
			{
				LogService.LogError("Failed to hotfix method " + ((MemberReference)val2).FullName);
				LogService.LogError(ex2.Message);
				LogService.LogError(ex2.StackTrace);
			}
		}
		return true;
	}

	private static MethodInfo CreateMethod(MethodDefinition newMethod)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		DynamicMethodDefinition val = new DynamicMethodDefinition(((MemberReference)newMethod).Name, ReflectionHelper.ResolveReflection(((MethodReference)newMethod).ReturnType), ((IEnumerable<ParameterDefinition>)((MethodReference)newMethod).Parameters).Select((ParameterDefinition x) => ReflectionHelper.ResolveReflection(((ParameterReference)x).ParameterType)).ToArray());
		if (!newMethod.IsStatic)
		{
			((MethodReference)val.Definition).Parameters.Insert(0, new ParameterDefinition((TypeReference)(object)newMethod.DeclaringType));
		}
		MethodBody body = val.Definition.Body;
		MethodBody body2 = newMethod.Body;
		body.Variables.Clear();
		body.Instructions.Clear();
		body.ExceptionHandlers.Clear();
		Extensions.AddRange<VariableDefinition>(body.Variables, (IEnumerable<VariableDefinition>)body2.Variables);
		Extensions.AddRange<Instruction>(body.Instructions, (IEnumerable<Instruction>)body2.Instructions);
		Extensions.AddRange<ExceptionHandler>(body.ExceptionHandlers, (IEnumerable<ExceptionHandler>)body2.ExceptionHandlers);
		return val.Generate();
	}

	private static void Replace(MethodInfo oldMethod, MethodDefinition newMethod)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		if (_create_hooks.ContainsKey(oldMethod))
		{
			_create_hooks[oldMethod].Dispose();
		}
		ILHook val = new ILHook((MethodBase)oldMethod, (Manipulator)delegate(ILContext il)
		{
			il.Body.Variables.Clear();
			il.Body.Instructions.Clear();
			il.Body.ExceptionHandlers.Clear();
			Extensions.AddRange<VariableDefinition>(il.Body.Variables, (IEnumerable<VariableDefinition>)newMethod.Body.Variables);
			Extensions.AddRange<Instruction>(il.Body.Instructions, (IEnumerable<Instruction>)newMethod.Body.Instructions);
			Extensions.AddRange<ExceptionHandler>(il.Body.ExceptionHandlers, (IEnumerable<ExceptionHandler>)newMethod.Body.ExceptionHandlers);
		});
		val.Apply();
		_create_hooks[oldMethod] = val;
	}

	private unsafe static void ReplaceMethod(MethodInfo pOldMethod, DynamicMethodDefinition pNewMethod)
	{
		MethodInfo methodInfo = pNewMethod.Generate();
		RuntimeHelpers.PrepareMethod(pOldMethod.MethodHandle);
		IntPtr functionPointer = pOldMethod.MethodHandle.GetFunctionPointer();
		RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
		IntPtr functionPointer2 = methodInfo.MethodHandle.GetFunctionPointer();
		LogService.LogInfo($"Is 64bit: {Environment.Is64BitProcess}");
		byte* ptr = (byte*)functionPointer.ToPointer();
		byte* ptr2 = (byte*)functionPointer2.ToPointer();
		long num = ptr2 - ptr - 5;
		if (num < uint.MaxValue && num > -4294967295L)
		{
			LogService.LogInfo($"diff is {num} doing relative jmp");
			LogService.LogInfo($"patching on {(ulong)ptr:X}, target: {(ulong)ptr2:X}");
			*ptr = 233;
			*(int*)(ptr + 1) = (int)num;
		}
		else
		{
			LogService.LogInfo($"diff is {num} doing push+ret trampoline");
			LogService.LogInfo($"patching on {(ulong)ptr:X}, target: {(ulong)ptr2:X}");
			if (Environment.Is64BitProcess)
			{
				byte* ptr3 = ptr;
				*(ptr3++) = 104;
				*(int*)ptr3 = (int)ptr2;
				ptr3 += 4;
				*(ptr3++) = 199;
				*(ptr3++) = 68;
				*(ptr3++) = 36;
				*(ptr3++) = 4;
				*(int*)ptr3 = (int)((ulong)ptr2 >> 32);
				ptr3 += 4;
				*(ptr3++) = 195;
			}
			else
			{
				*ptr = 104;
				*(int*)(ptr + 1) = (int)ptr2;
				ptr[5] = 195;
			}
		}
		LogService.LogInfo($"Patched 0x{(ulong)ptr:X} to 0x{(ulong)ptr2:X}.");
	}

	private static DynamicMethodDefinition regenerate(MethodDefinition pMethodDefinition)
	{
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Expected O, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		DynamicMethodDefinition val = new DynamicMethodDefinition(((MemberReference)pMethodDefinition).Name, ReflectionHelper.ResolveReflection(((MethodReference)pMethodDefinition).ReturnType), ((IEnumerable<ParameterDefinition>)((MethodReference)pMethodDefinition).Parameters).Select((ParameterDefinition x) => ReflectionHelper.ResolveReflection(((ParameterReference)x).ParameterType)).ToArray());
		if (!pMethodDefinition.IsStatic)
		{
			((MethodReference)val.Definition).Parameters.Insert(0, new ParameterDefinition((TypeReference)(object)pMethodDefinition.DeclaringType));
		}
		Enumerator<ParameterDefinition> enumerator = ((MethodReference)pMethodDefinition).Parameters.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ParameterDefinition current = enumerator.Current;
				LogService.LogInfo("\tDeclare parameter " + ((object)current).ToString() + "(" + ((MemberReference)((ParameterReference)current).ParameterType).FullName + ")");
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		ILGenerator iLGenerator = val.GetILGenerator();
		if (pMethodDefinition.Body.InitLocals)
		{
			val.Definition.Body.InitLocals = true;
		}
		Enumerator<VariableDefinition> enumerator2 = pMethodDefinition.Body.Variables.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				VariableDefinition current2 = enumerator2.Current;
				LogService.LogInfo("\tDeclare local variable " + ((object)current2).ToString() + "(" + ((MemberReference)((VariableReference)current2).VariableType).FullName + ")");
				iLGenerator.DeclareLocal(ReflectionHelper.ResolveReflection(((VariableReference)current2).VariableType));
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
		}
		Dictionary<Instruction, Label> dictionary = new Dictionary<Instruction, Label>();
		Enumerator<Instruction> enumerator3 = pMethodDefinition.Body.Instructions.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				Instruction current3 = enumerator3.Current;
				object operand = current3.Operand;
				Instruction val2 = (Instruction)((operand is Instruction) ? operand : null);
				if (val2 != null)
				{
					LogService.LogInfo("\tDeclare label for " + ((object)val2).ToString());
					dictionary[val2] = iLGenerator.DefineLabel();
				}
				else if (current3.Operand is Instruction[] array)
				{
					Instruction[] array2 = array;
					foreach (Instruction key in array2)
					{
						dictionary[key] = iLGenerator.DefineLabel();
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
		}
		Dictionary<Instruction, ExceptionHandler> dictionary2 = new Dictionary<Instruction, ExceptionHandler>();
		Enumerator<ExceptionHandler> enumerator4 = pMethodDefinition.Body.ExceptionHandlers.GetEnumerator();
		try
		{
			while (enumerator4.MoveNext())
			{
				ExceptionHandler current4 = enumerator4.Current;
				LogService.LogInfo("\tDeclare exception handler for " + ((object)current4).ToString());
				dictionary2[current4.TryStart] = current4;
				dictionary2[current4.TryEnd] = current4;
				dictionary2[current4.HandlerStart] = current4;
				dictionary2[current4.HandlerEnd] = current4;
				if (current4.TryStart == null)
				{
				}
			}
		}
		finally
		{
			((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
		}
		try
		{
			Enumerator<Instruction> enumerator5 = pMethodDefinition.Body.Instructions.GetEnumerator();
			try
			{
				while (enumerator5.MoveNext())
				{
					Instruction current5 = enumerator5.Current;
					if (dictionary.TryGetValue(current5, out var value))
					{
						iLGenerator.MarkLabel(value);
					}
					if (dictionary2.TryGetValue(current5, out var value2))
					{
						if (current5 == value2.TryEnd)
						{
							LogService.LogWarning("TryEnd");
						}
						else if (current5 == value2.HandlerStart)
						{
							LogService.LogWarning("HandlerStart");
						}
						else if (current5 == value2.HandlerEnd)
						{
							LogService.LogWarning("HandlerEnd");
						}
						else
						{
							LogService.LogWarning("TryStart");
						}
					}
					OpCode opCode = _op_code_map[current5.OpCode];
					if (opCode == OpCodes.Endfinally)
					{
						continue;
					}
					LogService.LogInfo($"\t{opCode}\t\t {current5.Operand}({current5.Operand?.GetType().FullName})");
					if (current5.Operand == null)
					{
						iLGenerator.Emit(opCode);
						continue;
					}
					if (current5.Operand is Instruction)
					{
						iLGenerator.Emit(opCode, dictionary[(Instruction)current5.Operand]);
						continue;
					}
					Type type = current5.Operand.GetType();
					object operand2 = current5.Operand;
					MemberReference val3 = (MemberReference)((operand2 is MemberReference) ? operand2 : null);
					if (val3 != null)
					{
						MemberInfo memberInfo = null;
						try
						{
							memberInfo = ReflectionHelper.ResolveReflection(val3);
							if (memberInfo == null)
							{
								throw new Exception("Failed to resolve member reference " + val3.FullName);
							}
						}
						catch (Exception ex)
						{
							try
							{
								MethodReference val4 = (MethodReference)(object)((val3 is MethodReference) ? val3 : null);
								if (val4 != null)
								{
									memberInfo = _regenerated_brand_new_methods[val4.Resolve()];
								}
							}
							catch (Exception)
							{
								LogService.LogError("Failed to resolve member reference " + val3.FullName);
								LogService.LogError(ex.Message);
								LogService.LogError(ex.StackTrace);
							}
						}
						type = memberInfo.GetType();
						if (!_emit_method_cache.TryGetValue(type, out var value3))
						{
							value3 = AccessTools.Method(typeof(ILGenerator), "Emit", new Type[2]
							{
								typeof(OpCode),
								type
							}, (Type[])null);
							_emit_method_cache[type] = value3;
						}
						if (value3 == null)
						{
							throw new Exception("Failed to get emit method for " + type.FullName);
						}
						value3.Invoke(iLGenerator, new object[2] { opCode, memberInfo });
						continue;
					}
					object operand3 = current5.Operand;
					VariableReference val5 = (VariableReference)((operand3 is VariableReference) ? operand3 : null);
					if (val5 != null)
					{
						iLGenerator.Emit(opCode, val5.Index);
						continue;
					}
					if (current5.Operand is Instruction[] array3)
					{
						Label[] array4 = new Label[array3.Length];
						for (int num2 = 0; num2 < array3.Length; num2++)
						{
							array4[num2] = dictionary[array3[num2]];
						}
						iLGenerator.Emit(OpCodes.Switch, array4);
						continue;
					}
					object operand4 = current5.Operand;
					ParameterDefinition val6 = (ParameterDefinition)((operand4 is ParameterDefinition) ? operand4 : null);
					if (val6 != null)
					{
						iLGenerator.Emit(opCode, val6.Sequence);
						continue;
					}
					if (!_emit_method_cache.TryGetValue(type, out var value4))
					{
						value4 = AccessTools.Method(typeof(ILGenerator), "Emit", new Type[2]
						{
							typeof(OpCode),
							type
						}, (Type[])null);
						_emit_method_cache[type] = value4;
					}
					if (value4 == null)
					{
						throw new Exception("Failed to get emit method for " + type.FullName);
					}
					try
					{
						value4.Invoke(iLGenerator, new object[2] { opCode, current5.Operand });
					}
					catch (Exception ex3)
					{
						if (current5.Operand is sbyte arg)
						{
							iLGenerator.Emit(opCode, (int)arg);
							continue;
						}
						LogService.LogError($"Failed to emit {opCode} {current5.Operand}({current5.Operand?.GetType().FullName})");
						LogService.LogError(ex3.Message);
						LogService.LogError(ex3.StackTrace);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator5/*cast due to constrained. prefix*/).Dispose();
			}
		}
		catch (Exception ex4)
		{
			LogService.LogError(ex4.Message);
			LogService.LogError(ex4.StackTrace);
		}
		finally
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Current instructions:");
			Enumerator<Instruction> enumerator6 = val.GetILProcessor().Body.Instructions.GetEnumerator();
			try
			{
				while (enumerator6.MoveNext())
				{
					Instruction current6 = enumerator6.Current;
					stringBuilder.AppendLine($"\t{current6.OpCode}\t\t {current6.Operand}({current6.Operand?.GetType().FullName})");
				}
			}
			finally
			{
				((IDisposable)enumerator6/*cast due to constrained. prefix*/).Dispose();
			}
			LogService.LogWarning(stringBuilder.ToString());
		}
		return val;
	}

	public static bool Reload()
	{
		try
		{
			_mod.Reload();
		}
		catch (Exception ex)
		{
			LogService.LogError(ex.Message);
			LogService.LogError(ex.StackTrace);
			return false;
		}
		return true;
	}
}

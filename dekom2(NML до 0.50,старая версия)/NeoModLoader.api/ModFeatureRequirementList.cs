using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeoModLoader.api;

public class ModFeatureRequirementList : IEnumerable<Type>, IEnumerable
{
	private List<Type> RequiredFeatureList { get; } = new List<Type>();

	public ModFeatureRequirementList(params Type[] types)
	{
		foreach (Type type in types)
		{
			if ((object)type == null)
			{
				throw new ArgumentNullException("types", "A required feature type was null.");
			}
			if (!typeof(IModFeature).IsAssignableFrom(type))
			{
				throw new ArgumentException("The type " + type.Name + " is not a valid feature type.");
			}
		}
		RequiredFeatureList.AddRange(types);
	}

	public static ModFeatureRequirementList operator +(ModFeatureRequirementList list, Type type)
	{
		return list.RequiredFeatureList.Append(type).ToList();
	}

	public static implicit operator ModFeatureRequirementList(List<Type> list)
	{
		return new ModFeatureRequirementList(list.ToArray());
	}

	public static implicit operator List<Type>(ModFeatureRequirementList list)
	{
		return list.RequiredFeatureList.ToList();
	}

	public static implicit operator ModFeatureRequirementList(Type type)
	{
		return new ModFeatureRequirementList(type);
	}

	public static implicit operator ModFeatureRequirementList(Type[] list)
	{
		return new ModFeatureRequirementList(list);
	}

	public IEnumerator<Type> GetEnumerator()
	{
		return RequiredFeatureList.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

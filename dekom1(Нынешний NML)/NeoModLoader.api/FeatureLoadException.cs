using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NeoModLoader.api;

public class FeatureLoadException : Exception
{
	protected FeatureLoadException([NotNull] SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public FeatureLoadException(string message)
		: base(message)
	{
	}

	public FeatureLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}

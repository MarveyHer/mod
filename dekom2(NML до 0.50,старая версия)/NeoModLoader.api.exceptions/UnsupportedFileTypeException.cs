using System.IO;

namespace NeoModLoader.api.exceptions;

public class UnsupportedFileTypeException : IOException
{
	public UnsupportedFileTypeException(string filePath)
		: base("Unsupported file type for path " + filePath)
	{
	}
}

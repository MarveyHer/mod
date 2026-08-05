using System.IO;
using Newtonsoft.Json;

public class CustomJsonArrayWriter : JsonTextWriter
{
	public CustomJsonArrayWriter(TextWriter writer)
		: base(writer)
	{
	}

	protected override void WriteIndent()
	{
		if (base.WriteState != WriteState.Array)
		{
			base.WriteIndent();
		}
		else
		{
			WriteIndentSpace();
		}
	}
}

using Newtonsoft.Json;

public static class JsonHelper
{
	private static JsonSerializer _writer;

	private static JsonSerializer _reader;

	private static JsonSerializerSettings _settings;

	public static JsonSerializer writer
	{
		get
		{
			if (_writer == null)
			{
				_writer = JsonSerializer.Create(new JsonSerializerSettings
				{
					DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
				});
			}
			return _writer;
		}
	}

	public static JsonSerializer reader
	{
		get
		{
			if (_reader == null)
			{
				_reader = JsonSerializer.Create(read_settings);
			}
			return _reader;
		}
	}

	public static JsonSerializerSettings read_settings
	{
		get
		{
			if (_settings == null)
			{
				_settings = new JsonSerializerSettings();
				_settings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
				_settings.Converters.Add(new LongJsonConverter());
				_settings.Converters.Add(new LongListJsonConverter());
				_settings.Converters.Add(new NullableLongJsonConverter());
				_settings.Converters.Add(new NullableLongListJsonConverter());
			}
			return _settings;
		}
	}
}

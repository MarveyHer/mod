using System;

namespace NeoModLoader.api;

[Obsolete("This interface is deprecated, it is useless and it has not actual effect now.")]
public interface IUnloadable
{
	void OnUnload();
}

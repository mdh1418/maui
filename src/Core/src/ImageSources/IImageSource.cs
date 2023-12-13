#nullable enable
using System;

namespace Microsoft.Maui
{
	public interface IImageSource
	{
		bool IsEmpty { get; }
#pragma warning disable RS0016
		object ImageSourceServiceKey { get; }
#pragma warning restore RS0016
	}
}
#nullable enable

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui
{
	public interface IImageSourceServiceProvider : IServiceProvider
	{
		IKeyedServiceProvider HostServiceProvider { get; }

		IImageSourceService? GetImageSourceService(Type imageSource);

		Type GetImageSourceServiceType(Type imageSource);

		Type GetImageSourceType(Type imageSource);
	}
}
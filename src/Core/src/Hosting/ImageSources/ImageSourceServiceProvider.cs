#nullable enable

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui.Hosting
{
	sealed class ImageSourceServiceProvider : IImageSourceServiceProvider
	{
		public ImageSourceServiceProvider(IImageSourceServiceCollection collection, IServiceProvider hostServiceProvider)
		{
			_ = collection;
			HostServiceProvider = (IKeyedServiceProvider)hostServiceProvider;
		}

		public IKeyedServiceProvider HostServiceProvider { get; }

		public object? GetService(Type serviceType)
		{
			if (serviceType is IImageSource imageSourceType)
			{
				try
				{
					return HostServiceProvider.GetKeyedService(typeof(IImageSourceService), imageSourceType.GetType());
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Unable to find a image source service for {nameof(imageSourceType)}.", ex);
				}
			}

			throw new InvalidOperationException($"Cannot retrieve ImageSourceService for non ImageSource type {nameof(serviceType)}");
		}

		public IImageSourceService? GetImageSourceService(Type imageSource) =>
			(IImageSourceService?)GetService(imageSource);

		public Type GetImageSourceServiceType(Type imageSource) =>
			throw new PlatformNotSupportedException();

		public Type GetImageSourceType(Type imageSource) =>
			throw new PlatformNotSupportedException();
	}
}
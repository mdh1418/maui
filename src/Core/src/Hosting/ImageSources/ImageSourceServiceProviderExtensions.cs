#nullable enable
using System;

namespace Microsoft.Maui
{
	public static class ImageSourceServiceProviderExtensions
	{
		public static IImageSourceService? GetImageSourceService(this IImageSourceServiceProvider provider, IImageSource imageSource) =>
			throw new PlatformNotSupportedException();

		public static IImageSourceService? GetImageSourceService<T>(this IImageSourceServiceProvider provider)
			where T : IImageSource =>
			throw new PlatformNotSupportedException();

		public static IImageSourceService GetRequiredImageSourceService(this IImageSourceServiceProvider provider, IImageSource imageSource) =>
			throw new PlatformNotSupportedException();

		public static IImageSourceService GetRequiredImageSourceService<T>(this IImageSourceServiceProvider provider)
			where T : IImageSource =>
			throw new PlatformNotSupportedException();

		public static IImageSourceService GetRequiredImageSourceService(this IImageSourceServiceProvider provider, Type imageSourceType) =>
			throw new PlatformNotSupportedException();
	}
}
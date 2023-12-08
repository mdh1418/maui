using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui.Hosting
{
	public static class ImageSourcesMauiAppBuilderExtensions
	{
		public static MauiAppBuilder ConfigureImageSources(this MauiAppBuilder builder)
		{
			builder.ConfigureImageSources(services =>
			{
				services.AddKeyedService<IFileImageSource>(typeof(IImageSourceService<IFileImageSource>), svcs => new FileImageSourceService(svcs.CreateLogger<FileImageSourceService>()));
				services.AddKeyedService<IFontImageSource>(typeof(IImageSourceService<IFontImageSource>), svcs => new FontImageSourceService(svcs.GetRequiredService<IFontManager>(), svcs.CreateLogger<FontImageSourceService>()));
				services.AddKeyedService<IStreamImageSource>(typeof(IImageSourceService<IStreamImageSource>), svcs => new StreamImageSourceService(svcs.CreateLogger<StreamImageSourceService>()));
				services.AddKeyedService<IUriImageSource>(typeof(IImageSourceService<IUriImageSource>), svcs => new UriImageSourceService(svcs.CreateLogger<UriImageSourceService>()));
			});
			return builder;
		}

		public static MauiAppBuilder ConfigureImageSources(this MauiAppBuilder builder, Action<IImageSourceServiceCollection>? configureDelegate)
		{
			if (configureDelegate != null)
			{
				builder.Services.AddKeyedSingleton<ImageSourceRegistration>(new ImageSourceRegistration(configureDelegate));
			}

			builder.Services.TryAddKeyedSingleton<IImageSourceServiceProvider>(typeof(IImageSourceServiceProvider), (svcs, _) => new ImageSourceServiceProvider(svcs.GetRequiredService<IImageSourceServiceCollection>(), svcs));
			builder.Services.TryAddKeyedSingleton<IImageSourceServiceCollection>(typeof(IImageSourceServiceCollection), (svcs, _) => new ImageSourceServiceBuilder(svcs.GetServices<ImageSourceRegistration>()));

			return builder;
		}

		class ImageSourceRegistration
		{
			private readonly Action<IImageSourceServiceCollection> _registerAction;

			public ImageSourceRegistration(Action<IImageSourceServiceCollection> registerAction)
			{
				_registerAction = registerAction;
			}

			internal void AddRegistration(IImageSourceServiceCollection builder)
			{
				_registerAction(builder);
			}
		}

		class ImageSourceServiceBuilder : MauiServiceCollection, IImageSourceServiceCollection
		{
			public ImageSourceServiceBuilder(IEnumerable<ImageSourceRegistration> registrationActions)
			{
				if (registrationActions != null)
				{
					foreach (var effectRegistration in registrationActions)
					{
						effectRegistration.AddRegistration(this);
					}
				}
			}
		}
	}
}

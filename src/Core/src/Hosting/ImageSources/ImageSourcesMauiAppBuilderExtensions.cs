﻿using System;
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
			return builder.ConfigureImageSources(null);
		}

		public static MauiAppBuilder ConfigureImageSources(this MauiAppBuilder builder, Action<IImageSourceServiceCollection>? configureDelegate)
		{
			builder.Services.AddKeyedSingleton<IImageSourceService>(typeof(IFileImageSource), (svcs, _) => new FileImageSourceService(svcs.CreateLogger<FileImageSourceService>()));
			builder.Services.AddKeyedSingleton<IImageSourceService>(typeof(IFontImageSource), (svcs, _) => new FontImageSourceService(svcs.GetRequiredService<IFontManager>(), svcs.CreateLogger<FontImageSourceService>()));
			builder.Services.AddKeyedSingleton<IImageSourceService>(typeof(IStreamImageSource), (svcs, _) => new StreamImageSourceService(svcs.CreateLogger<StreamImageSourceService>()));
			builder.Services.AddKeyedSingleton<IImageSourceService>(typeof(IUriImageSource), (svcs, _) => new UriImageSourceService(svcs.CreateLogger<UriImageSourceService>()));

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

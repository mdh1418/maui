using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Maui.Hosting
{
	internal sealed class ImageSourceToImageSourceServiceTypeMapping
	{
		private static readonly ConcurrentDictionary<IImageSourceServiceCollection, ImageSourceToImageSourceServiceTypeMapping> s_instances = new();

		internal static ImageSourceToImageSourceServiceTypeMapping GetInstance(IImageSourceServiceCollection collection) =>
			s_instances.GetOrAdd(collection, static _ => new ImageSourceToImageSourceServiceTypeMapping());

		private readonly object _concretelock = new();
		private readonly object _interfacelock = new();
		private readonly Dictionary<Type, Type> _concreteTypeMapping = new(4); // MAUI registers 4 concrete image source services at startup
		private readonly Dictionary<Type, Type> _interfaceTypeMapping = new(4); // MAUI registers 4 interface source services at startup

		public void Add<TImageSource, TImageSourceService>()
			where TImageSource : IImageSource
			where TImageSourceService : class, IImageSourceService<TImageSource>
		{
			if (typeof(TImageSource).IsInterface)
			{
				lock (_interfacelock)
				{
					_interfaceTypeMapping[typeof(TImageSource)] = typeof(TImageSourceService);
				}
			}
			else
			{
				lock (_concretelock)
				{
					_concreteTypeMapping[typeof(TImageSource)] = typeof(TImageSourceService);
				}
			}
		}

		public Type FindImageSourceServiceType(Type type)
		{
			Debug.Assert(typeof(IImageSource).IsAssignableFrom(type));

			if (_concreteTypeMapping.TryGetValue(type, out var exactImageSourceService) || _interfaceTypeMapping.TryGetValue(type, out exactImageSourceService))
				return exactImageSourceService;

			Type? bestImageSource = null;
			Type? bestImageSourceService = null;

			foreach (var kvp in _concreteTypeMapping)
			{
				var imageSource = kvp.Key;
				var imageSourceService = kvp.Value;
				if (!imageSource.IsAssignableFrom(type))
					continue;

				if (bestImageSource is null || bestImageSource.IsAssignableFrom(imageSource))
				{
					bestImageSource = imageSource;
					bestImageSourceService = imageSourceService;
				}

				if (!imageSource.IsAssignableFrom(bestImageSource) && !bestImageSource.IsAssignableFrom(imageSource))
					throw new InvalidOperationException($"{type} should not be assignable from multiple unrelated concrete types");
			}

			if (bestImageSourceService != null)
				return bestImageSourceService;

			foreach (var kvp in _interfaceTypeMapping)
			{
				var imageSource = kvp.Key;
				var imageSourceService = kvp.Value;
				if (!imageSource.IsAssignableFrom(type))
					continue;

				if (bestImageSource is null || bestImageSource.IsAssignableFrom(imageSource))
				{
					bestImageSource = imageSource;
					bestImageSourceService = imageSourceService;
				}

				if (!imageSource.IsAssignableFrom(bestImageSource) && !bestImageSource.IsAssignableFrom(imageSource))
					throw new InvalidOperationException($"Unable to find a single {nameof(IImageSourceService)} corresponding to {type}. There is an ambiguous match between {bestImageSourceService} ({bestImageSource}) and {imageSourceService} ({imageSource}).");
			}

			if (bestImageSourceService is null)
				throw new InvalidOperationException($"Unable to find any configured {nameof(IImageSourceService)} corresponding to {type}.");

			return bestImageSourceService;
		}
	}
}

﻿#nullable enable

using System;
using System.Collections.Concurrent;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui.Hosting
{
	sealed class ImageSourceServiceProvider : MauiFactory, IImageSourceServiceProvider
	{
		readonly ConcurrentDictionary<Type, Type> _imageSourceCache = new ConcurrentDictionary<Type, Type>();
		readonly ConcurrentDictionary<Type, Type> _serviceCache = new ConcurrentDictionary<Type, Type>();
		readonly IImageSourceServiceCollection _collection;

		public ImageSourceServiceProvider(IImageSourceServiceCollection collection, IServiceProvider hostServiceProvider)
			: base(collection)
		{
			_collection = collection;
			HostServiceProvider = hostServiceProvider;
		}

		public IServiceProvider HostServiceProvider { get; }

		public IImageSourceService? GetImageSourceService(Type imageSource) =>
			(IImageSourceService?)GetService(GetImageSourceServiceType(imageSource));

		public Type GetImageSourceServiceType(Type imageSource) =>
			_serviceCache.GetOrAdd(imageSource, _collection.FindImageSourceServiceType);

		public Type GetImageSourceType(Type imageSource) =>
			_imageSourceCache.GetOrAdd(imageSource, _collection.FindImageSourceType);
	}
}
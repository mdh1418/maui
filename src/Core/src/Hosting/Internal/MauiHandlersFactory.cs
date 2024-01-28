#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Maui.Hosting.Internal
{
	sealed class MauiHandlersFactory : MauiFactory, IMauiHandlersFactory
	{
		readonly List<Func<object, IElementHandler?>> _handlerFactories = new ();

		public MauiHandlersFactory(IEnumerable<HandlerMauiAppBuilderExtensions.HandlerRegistration> registrationActions, IEnumerable<HandlerMauiAppBuilderExtensions.HandlerFactory> handlerFactory) :
			base(CreateHandlerCollection(registrationActions))
		{
			if (handlerFactory != null)
			{
				foreach (var factory in handlerFactory)
					_handlerFactories.Add(factory.GetHandler);
			}
		}

		static MauiHandlersCollection CreateHandlerCollection(IEnumerable<HandlerMauiAppBuilderExtensions.HandlerRegistration> registrationActions)
		{
			var collection = new MauiHandlersCollection();
			if (registrationActions != null)
			{
				foreach (var registrationAction in registrationActions)
				{
					registrationAction.AddRegistration(collection);
				}
			}
			HotReload.MauiHotReloadHelper.RegisterHandlers(collection);
			return collection;
		}

		public IElementHandler? GetHandler(Type type)
			=> GetService(type) as IElementHandler;

#pragma warning disable RS0016
		public IElementHandler? GetHandler(object type)
		{
			IElementHandler? handler = null;

			foreach (var factory in _handlerFactories)
			{
				handler = factory(type);
				if (handler != null)
					return handler;
			}

			return handler;
		}
#pragma warning restore RS0016

		public IElementHandler? GetHandler<T>() where T : IElement
			=> GetHandler(typeof(T));

		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
		public Type? GetHandlerType(Type iview)
		{
			if (!TryGetServiceDescriptors(ref iview, out var single, out var enumerable))
				return default;

			if (single != null)
				return single.ImplementationType;

			if (enumerable != null)
			{
				foreach (var descriptor in enumerable)
				{
					return descriptor.ImplementationType;
				}
			}

			return default;
		}

		public IMauiHandlersCollection GetCollection() => (IMauiHandlersCollection)InternalCollection;
	}
}
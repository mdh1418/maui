using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui.Hosting
{
	public static class HandlerMauiAppBuilderExtensions
	{
#pragma warning disable RS0016
		public static MauiAppBuilder ConfigureMauiHandlers(this MauiAppBuilder builder, Action<IMauiHandlersCollection>? configureDelegate, Func<object, IElementHandler?>? handlerFactory = null)
		{
			ConfigureMauiHandlers(builder.Services, configureDelegate, handlerFactory);
			return builder;
		}

		public static IServiceCollection ConfigureMauiHandlers(this IServiceCollection services, Action<IMauiHandlersCollection>? configureDelegate, Func<object, IElementHandler?>? handlerFactory = null)
		{
			services.TryAddSingleton<IMauiHandlersFactory>(sp => new MauiHandlersFactory(sp.GetServices<HandlerRegistration>(), sp.GetServices<HandlerFactory>()));
			if (configureDelegate != null)
			{
				services.AddSingleton<HandlerRegistration>(new HandlerRegistration(configureDelegate));
			}
            if (handlerFactory  != null)
            {
                services.AddSingleton<HandlerFactory>(new HandlerFactory(handlerFactory));
            }

			return services;
		}
#pragma warning restore RS0016

		internal class HandlerRegistration
		{
			private readonly Action<IMauiHandlersCollection> _registerAction;

			public HandlerRegistration(Action<IMauiHandlersCollection> registerAction)
			{
				_registerAction = registerAction;
			}

			internal void AddRegistration(IMauiHandlersCollection builder)
			{
				_registerAction(builder);
			}
		}

        internal class HandlerFactory
        {
            private readonly Func<object, IElementHandler?> _handlerFactory;

            public HandlerFactory(Func<object, IElementHandler?> handlerFactory)
            {
                _handlerFactory = handlerFactory;
            }

            internal IElementHandler? GetHandler(object type)
            {
                return _handlerFactory(type);
            }
        }
	}
}

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui
{
	public interface IMauiHandlersFactory : IMauiFactory
	{
		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
		Type? GetHandlerType(Type iview);

		IElementHandler? GetHandler(Type type);

#pragma warning disable RS0016
		IElementHandler? GetHandler(object type);
#pragma warning restore RS0016

		IElementHandler? GetHandler<T>() where T : IElement;

		IMauiHandlersCollection GetCollection();
	}
}
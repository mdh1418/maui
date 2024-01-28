using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Platform;


#if ANDROID
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#elif WINDOWS
using ResourcesProvider = Microsoft.Maui.Controls.Compatibility.Platform.UWP.WindowsResourcesProvider;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
#elif IOS || MACCATALYST
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Handlers.Compatibility;
#elif TIZEN
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.Tizen;
#endif

namespace Microsoft.Maui.Controls.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		/// <summary>
		/// Configures the <see cref="MauiAppBuilder"/> to use the specified <typeparamref name="TApp"/> as the main application type.
		/// </summary>
		/// <typeparam name="TApp">The type to use as the application.</typeparam>
		/// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
		/// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
		public static MauiAppBuilder UseMauiApp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(this MauiAppBuilder builder)
			where TApp : class, IApplication
		{
#pragma warning disable RS0030 // Do not used banned APIs - don't want to use a factory method here
			builder.Services.TryAddSingleton<IApplication, TApp>();
#pragma warning restore RS0030
			builder.SetupDefaults();
			return builder;
		}

		/// <summary>
		/// Configures the <see cref="MauiAppBuilder"/> to use the specified <typeparamref name="TApp"/> as the main application type.
		/// </summary>
		/// <typeparam name="TApp">The type to use as the application.</typeparam>
		/// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
		/// <param name="implementationFactory">A factory to create the specified <typeparamref name="TApp"/> using the services provided in a <see cref="IServiceProvider"/>.</param>
		/// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
		public static MauiAppBuilder UseMauiApp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(this MauiAppBuilder builder, Func<IServiceProvider, TApp> implementationFactory)
			where TApp : class, IApplication
		{
			builder.Services.TryAddSingleton<IApplication>(implementationFactory);
			builder.SetupDefaults();
			return builder;
		}

		public static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlersCollection) => handlersCollection;

#pragma warning disable RS0016
		public static IElementHandler GetBarebonesHandlers(object type)
		{
			return type switch
			{
				CollectionView => new CollectionViewHandler(),
				CarouselView => new CarouselViewHandler(),
				Application => new ApplicationHandler(),
				ActivityIndicator => new ActivityIndicatorHandler(),
				BoxView => new BoxViewHandler(),
				Button => new ButtonHandler(),
				CheckBox => new CheckBoxHandler(),
				DatePicker => new DatePickerHandler(),
				Editor => new EditorHandler(),
				Entry => new EntryHandler(),
				GraphicsView => new GraphicsViewHandler(),
				Image => new ImageHandler(),
				Label => new LabelHandler(),
				IndicatorView => new IndicatorViewHandler(),
				RadioButton => new RadioButtonHandler(),
				RefreshView => new RefreshViewHandler(),
				SwipeView => new SwipeViewHandler(),
#if ANDROID || IOS || MACCATALYST || TIZEN
				SwipeItemView => new SwipeItemViewHandler(),
#if ANDROID || IOS || MACCATALYST
				Shell => new ShellRenderer(),
#else
				Shell => new ShellHandler(),
				ShellItem => new ShellItemHandler(),
				ShellSection => new ShellSectionHandler(),
#endif
#endif
				Layout => new LayoutHandler(),
				Picker => new PickerHandler(),
				ProgressBar => new ProgressBarHandler(),
				ScrollView => new ScrollViewHandler(),
				SearchBar => new SearchBarHandler(),
				Slider => new SliderHandler(),
				Stepper => new StepperHandler(),
				Switch => new SwitchHandler(),
				TimePicker => new TimePickerHandler(),
#if IOS || MACCATALYST
				NavigationPage => new Handlers.Compatibility.NavigationRenderer(),
				TabbedPage => new Handlers.Compatibility.TabbedRenderer(),
				FlyoutPage => new Handlers.Compatibility.PhoneFlyoutPageRenderer(),
#endif
#if WINDOWS || ANDROID || TIZEN
				NavigationPage => new NavigationViewHandler(),
				Toolbar => new ToolbarHandler(),
				FlyoutPage => new FlyoutViewHandler(),
				TabbedPage => new TabbedViewHandler(),
#endif
				Page => new PageHandler(),
				WebView => new WebViewHandler(),
				Border => new BorderHandler(),
				Shapes.Ellipse => new ShapeViewHandler(),
				Shapes.Line => new LineHandler(),
				Shapes.Path => new PathHandler(),
				Shapes.Polygon => new PolygonHandler(),
				Shapes.Polyline => new PolylineHandler(),
				Shapes.Rectangle => new RectangleHandler(),
				Shapes.RoundRectangle => new RoundRectangleHandler(),
				Window => new WindowHandler(),
				ImageButton => new ImageButtonHandler(),
				SwipeItem => new SwipeItemMenuItemHandler(),

#pragma warning disable CA1416 //  'MenuBarHandler' => new MenuFlyoutSubItemHandler => new MenuFlyoutSubItemHandler => new MenuBarItemHandler is only supported on: 'ios' 13.0 and later
				MenuBar => new MenuBarHandler(),
				MenuFlyoutSubItem => new MenuFlyoutSubItemHandler(),
				MenuFlyoutSeparator => new MenuFlyoutSeparatorHandler(),
				MenuFlyoutItem => new MenuFlyoutItemHandler(),
				MenuBarItem => new MenuBarItemHandler(),
#pragma warning restore CA1416

#if WINDOWS || ANDROID || IOS || MACCATALYST || TIZEN
				// ListView => new Handlers.Compatibility.ListViewRenderer(),
#if !TIZEN
				ImageCell => new Handlers.Compatibility.ImageCellRenderer(),
				EntryCell => new Handlers.Compatibility.EntryCellRenderer(),
				TextCell => new Handlers.Compatibility.TextCellRenderer(),
				ViewCell => new Handlers.Compatibility.ViewCellRenderer(),
				SwitchCell => new Handlers.Compatibility.SwitchCellRenderer(),
				Cell => new Handlers.Compatibility.CellRenderer(),
#endif
				// TableView => new Handlers.Compatibility.TableViewRenderer(),
				// Frame => new Handlers.Compatibility.FrameRenderer(),
#endif

#if WINDOWS || MACCATALYST
				MenuFlyout => new MenuFlyoutHandler(),
#endif

#if WINDOWS
				ShellItem => new ShellItemHandler(),
				ShellSection => new ShellSectionHandler(),
				ShellContent => new ShellContentHandler(),
				Shell => new ShellHandler(),
#endif
				IContentView => new ContentViewHandler(),
				_ => null
			};
		}
#pragma warning restore RS0016

		static MauiAppBuilder SetupDefaults(this MauiAppBuilder builder)
		{
#if WINDOWS || ANDROID || IOS || MACCATALYST || TIZEN
			// initialize compatibility DependencyService
			DependencyService.SetToInitialized();
			DependencyService.Register<Xaml.ResourcesLoader>();
			DependencyService.Register<Xaml.ValueConverterProvider>();
			DependencyService.Register<PlatformSizeService>();

#pragma warning disable CS0612, CA1416 // Type or member is obsolete, 'ResourcesProvider' is unsupported on: 'iOS' 14.0 and later
			DependencyService.Register<ResourcesProvider>();
			DependencyService.Register<FontNamedSizeService>();
#pragma warning restore CS0612, CA1416 // Type or member is obsolete
#endif
			builder.Services.AddScoped(_ => new HideSoftInputOnTappedChangedManager());
			builder.ConfigureImageSourceHandlers();
			builder
				.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddMauiControlsHandlers();
				}, GetBarebonesHandlers);

#if WINDOWS
			builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, MauiControlsInitializer>());
#endif
			builder.RemapForControls();

			return builder;
		}

		class MauiControlsInitializer : IMauiInitializeService
		{
			public void Initialize(IServiceProvider services)
			{
#if WINDOWS
				var dispatcher =
					services.GetService<IDispatcher>() ??
					IPlatformApplication.Current?.Services.GetRequiredService<IDispatcher>();

				dispatcher
					.DispatchIfRequired(() =>
					{
						var dictionaries = UI.Xaml.Application.Current?.Resources?.MergedDictionaries;
						if (dictionaries != null)
						{
							// Microsoft.Maui.Controls
							UI.Xaml.Application.Current?.Resources?.AddLibraryResources("MicrosoftMauiControlsIncluded", "ms-appx:///Microsoft.Maui.Controls/Platform/Windows/Styles/Resources.xbf");
						}
					});
#endif
			}
		}


		internal static MauiAppBuilder ConfigureImageSourceHandlers(this MauiAppBuilder builder)
		{
			builder.ConfigureImageSources(services =>
			{
				services.AddService<FileImageSource>(svcs => new FileImageSourceService(svcs.CreateLogger<FileImageSourceService>()));
				services.AddService<FontImageSource>(svcs => new FontImageSourceService(svcs.GetRequiredService<IFontManager>(), svcs.CreateLogger<FontImageSourceService>()));
				services.AddService<StreamImageSource>(svcs => new StreamImageSourceService(svcs.CreateLogger<StreamImageSourceService>()));
				services.AddService<UriImageSource>(svcs => new UriImageSourceService(svcs.CreateLogger<UriImageSourceService>()));
			});

			return builder;
		}

		internal static MauiAppBuilder RemapForControls(this MauiAppBuilder builder)
		{
			// Update the mappings for IView/View to work specifically for Controls
			Element.RemapForControls();
			Application.RemapForControls();
			VisualElement.RemapForControls();
			Label.RemapForControls();
			Button.RemapForControls();
			CheckBox.RemapForControls();
			DatePicker.RemapForControls();
			RadioButton.RemapForControls();
			FlyoutPage.RemapForControls();
			Toolbar.RemapForControls();
			Window.RemapForControls();
			Editor.RemapForControls();
			Entry.RemapForControls();
			Picker.RemapForControls();
			SearchBar.RemapForControls();
			TabbedPage.RemapForControls();
			TimePicker.RemapForControls();
			Layout.RemapForControls();
			ScrollView.RemapForControls();
			RefreshView.RemapForControls();
			Shape.RemapForControls();
			WebView.RemapForControls();
			ContentPage.RemapForControls();

			return builder;
		}
	}
}
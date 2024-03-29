﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml;
using CommandLine;
using ControlzEx.Theming;
using devTools.WiXComponents.Core;
using devTools.WiXComponents.Core.Services;
using devTools.WiXComponents.Core.ViewModels;
using devTools.WiXComponents.Properties;
using essentialMix.Data.Helpers;
using essentialMix.Extensions;
using essentialMix.Helpers;
using essentialMix.Logging;
using essentialMix.Newtonsoft.Helpers;
using JetBrains.Annotations;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Theme = MaterialDesignThemes.Wpf.Theme;

namespace devTools.WiXComponents
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, IApp
	{
		private const string MAHAPPS_THEME = "Steel";

		private static readonly IReadOnlySet<string> __supportedProjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".WiXProj",
			".CSProj",
			".VBProj",
		};

		private static readonly IReadOnlySet<string> __supportedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".wxs",
			".wsi",
			".xml",
		};

		private readonly Lazy<PaletteHelper> _paletteHelper = new Lazy<PaletteHelper>(() => new PaletteHelper(), LazyThreadSafetyMode.PublicationOnly);

		private bool? _darkTheme;

		/// <inheritdoc />
		public App()
		{
			Serilog.ILogger serilogLogger = new LoggerConfiguration()
											.MinimumLevel.Is(LogEventLevel.Verbose)
											.WriteTo.Console(outputTemplate: "{Level:u3} {Message:lj}{NewLine}{Exception}", theme: SystemConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
											.CreateLogger();

			ILoggerFactory factory = LoggerFactory.Create(builder =>
			{
				builder.ClearProviders();
				builder.AddSerilog(serilogLogger, true);
			});
			Logger = new CombinedLogger<App>(factory.CreateLogger(nameof(App)));
		}

		public IServiceProvider ServiceProvider { get; private set; }

		public bool DarkTheme
		{
			get => _darkTheme ??= _paletteHelper.Value.GetTheme().GetBaseTheme() == BaseTheme.Dark;
			set
			{
				if (_darkTheme == value) return;
				_darkTheme = value;
				ITheme theme = _paletteHelper.Value.GetTheme();

				if (value)
				{
					ThemeManager.Current.ChangeTheme(this, $"Dark.{MAHAPPS_THEME}");
					theme.SetBaseTheme(Theme.Dark);
				}
				else
				{
					ThemeManager.Current.ChangeTheme(this, $"Light.{MAHAPPS_THEME}");
					theme.SetBaseTheme(Theme.Light);
				}

				_paletteHelper.Value.SetTheme(theme);
			}
		}

		[NotNull]
		public CombinedLogger<App> Logger { get; }

		/// <inheritdoc />
		protected override void OnStartup([NotNull] StartupEventArgs e)
		{
			// Setup
			JsonConvert.DefaultSettings = () => JsonHelper.CreateSettings();
			string basePath = Assembly.GetExecutingAssembly().GetDirectoryPath();
			bool consoleCreated = false;

			if (e.Args.Length > 0)
			{
				ConsoleHelper.AttachConsole(out consoleCreated);

				if (ConsoleHelper.HasConsole)
				{
					ConsoleHelper.Show();
				}
				else
				{
					Logger.LogError("Could not create Writer window.");
					Shutdown();
					return;
				}
			}

			// Configuration
			Logger.LogInformation("Loading configuration.");
			IConfiguration configuration = IConfigurationBuilderHelper.CreateConfiguration(basePath)
																	.AddConfigurationFiles(basePath, EnvironmentHelper.GetEnvironmentName())
																	.AddEnvironmentVariables()
																	.AddUserSecrets()
																	.AddArguments(e.Args)
																	.Build();

			Parser parser = null;
			StartupArguments args;

			try
			{
				// Command line
				Logger.LogInformation("Processing command line.");
				parser = new Parser(settings =>
				{
					settings.CaseSensitive = false;
					settings.CaseInsensitiveEnumValues = true;
				});

				ParserResult<StartupArguments> result = parser.ParseArguments<StartupArguments>(e.Args);

				if (result.Tag == ParserResultType.NotParsed)
				{
					Logger.LogError(StartupArguments.GetUsage(result));
					Shutdown();
					return;
				}

				args = ((Parsed<StartupArguments>)result).Value;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.CollectMessages());
				Shutdown();
				return;
			}
			finally
			{
				// in case an error occurred and the parser was not disposed
				ObjectHelper.Dispose(ref parser);
			}

			// Logging
			Logger.LogInformation("Configuring logging.");

			if (configuration.GetValue<bool>("Logging:Enabled") && args.LogLevel < LogLevel.None)
			{
				LoggerConfiguration loggerConfiguration = new();
				loggerConfiguration.ReadFrom.Configuration(configuration);
				loggerConfiguration.MinimumLevel.Is(args.LogLevel switch
				{
					LogLevel.Trace => LogEventLevel.Verbose,
					LogLevel.Debug => LogEventLevel.Debug,
					LogLevel.Information => LogEventLevel.Information,
					LogLevel.Warning => LogEventLevel.Warning,
					LogLevel.Error => LogEventLevel.Error,
					_ => LogEventLevel.Fatal
				});
				Log.Logger = loggerConfiguration.CreateLogger();
			}

			// Services
			Logger.LogInformation("Configuring services.");
			ConfigureServices(args, configuration);
			Logger.Combine(ServiceProvider.GetService<ILogger<App>>());

			if (args.Files.Count > 0 || !string.IsNullOrEmpty(args.Directory))
			{
				Logger.LogInformation("Starting command line.");
				ProcessCommandLine(args);
				if (consoleCreated) ConsoleHelper.FreeConsole();
				Shutdown();
				return;
			}

			Logger.LogInformation("Starting main window.");
			ConsoleHelper.Hide();
			if (consoleCreated) ConsoleHelper.FreeConsole();

			Resources.MergedDictionaries.Add(new ResourceDictionary
			{
				Source = new Uri("pack://application:,,,/Themes/Default.xaml")
			});
			Resources.MergedDictionaries.Add(new ResourceDictionary
			{
				Source = new Uri("pack://application:,,,/Resources/DataTemplates.xaml")
			});
			if (_paletteHelper.Value.GetThemeManager() is { } themeManager) themeManager.ThemeChanged += (_, arg) => _darkTheme = arg.NewTheme.GetBaseTheme() == BaseTheme.Dark;
			FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
			{
				DefaultValue = Resources["MaterialDesignWindow"]
			});

			ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
			ThemeManager.Current.SyncTheme();
			DarkTheme = false;

			base.OnStartup(e);
			Start();
		}

		/// <inheritdoc />
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Logger.LogInformation("Exiting.");
			Log.CloseAndFlush();
		}

		private void ConfigureServices([NotNull] StartupArguments startupArguments, [NotNull] IConfiguration configuration)
		{
			ServiceCollection services = new();
			services.AddSingleton(startupArguments);
			services.AddSingleton(configuration);
			services.AddLogging(builder =>
			{
				// clear the default providers
				builder.ClearProviders();
				builder.AddConfiguration(configuration.GetSection("logging"));
				builder.AddDebug();
				builder.AddEventSourceLogger();
				builder.AddSerilog(null, true);
			});
			services.AddSingleton<GeneratorService>();
			services.AddSingleton<ComponentsService>();

			Assembly asm = typeof(CommandViewModelBase).Assembly;
			Type[] viewModelTypes = asm.GetTypes()
										.Where(type => !type.IsAbstract && typeof(CommandViewModelBase).IsAssignableFrom(type))
										.OrderBy(type => type.GetCustomAttribute<DisplayAttribute>()?.Order ?? short.MaxValue)
										.ToArray();

			foreach (Type type in viewModelTypes)
				services.AddSingleton(type);

			services.AddSingleton(svc =>
			{
				ILogger<MainViewModel> lg = (ILogger<MainViewModel>)svc.GetService(typeof(ILogger<MainViewModel>));
				MainViewModel vm = new MainViewModel(this, lg);
				ObservableCollection<CommandViewModelBase> viewModels = vm.ViewModels;

				foreach (Type type in viewModelTypes)
				{
					CommandViewModelBase vmc = (CommandViewModelBase)ServiceProvider.GetRequiredService(type);
					viewModels.Add(vmc);
				}
				return vm;
			});

			services.AddSingleton<MainWindow>();
			ServiceProvider = services.BuildServiceProvider();
		}

		private void Start()
		{
			MainWindow window = ServiceProvider.GetRequiredService<MainWindow>();
			window.Show();
		}

		private void ProcessCommandLine([NotNull] StartupArguments args)
		{
			// When we are here, args either has files or a directory to be processed.
			if (!string.IsNullOrEmpty(args.Directory))
			{
				Logger.LogInformation($"Processing directory '{args.Directory}'.");

				if (!Directory.Exists(args.Directory))
				{
					Logger.LogError(Errors.DirectoryNotFound, args.Directory);
					return;
				}

				IEnumerable<string> files = DirectoryHelper.EnumerateFiles(args.Directory, "*.wxs;*.wsi;*.xml", args.IncludeSubDirectory
																													? SearchOption.AllDirectories
																													: SearchOption.TopDirectoryOnly);
				foreach (string fileName in files)
				{
					ProcessWiXFile(fileName);
				}

				return;
			}

			Logger.LogInformation("Processing files.");

			foreach (string fileName in args.Files)
			{
				string ext = PathHelper.Extension(fileName);
				if (ext == null) continue;
				if (__supportedProjects.Contains(ext)) ProcessProjectFile(fileName);
				if (__supportedFiles.Contains(ext)) ProcessWiXFile(fileName);
			}
		}

		private void ProcessProjectFile([NotNull] string fileName)
		{
			// wixproj, csproj, vbproj
			Logger.LogInformation($"Processing project file '{fileName}'.");

			if (!File.Exists(fileName))
			{
				Logger.LogError(Errors.FileNotFound, fileName);
				return;
			}

			IList<string> files = new List<string>();

			try
			{
				Logger.LogInformation("Loading project file.");
				XmlDocument doc = XmlDocumentHelper.LoadFile(fileName);
				XmlNode root = doc.DocumentElement;

				if (root == null)
				{
					Logger.LogInformation("Skipping empty project.");
					return;
				}

				XmlNamespaceManager manager = doc.GetNamespaceManager();
				string prefix = manager.GetDefaultPrefix();

				Logger.LogInformation("Project loaded, looking for files.");
				// Finds all of the files included in the project.
				XmlNodeList nodes = doc.SelectNodes($"//{prefix}Compile", manager);
				ProcessNodes(nodes, files, Logger);
				nodes = doc.SelectNodes($"//{prefix}Content", manager);
				ProcessNodes(nodes, files, Logger);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.CollectMessages());
				return;
			}

			if (files.Count == 0)
			{
				Logger.LogInformation("No suitable components found.");
				return;
			}

			Logger.LogInformation($"Found {files.Count} files.");

			string path = Path.GetDirectoryName(fileName) ?? Directory.GetCurrentDirectory();
			Logger.LogInformation($"Current path: '{path}'.");

			foreach (string file in files)
			{
				ProcessWiXFile(Path.Combine(path, file));
			}

			static void ProcessNodes(XmlNodeList nodes, IList<string> files, ILogger logger)
			{
				if (nodes == null || nodes.Count == 0) return;

				foreach (XmlNode node in nodes)
				{
					string itemName = node.Attributes?["Include"]?.Value;
					if (string.IsNullOrEmpty(itemName)) continue;

					if (node.HasAttributeOrChild("Link"))
					{
						logger.LogInformation($"Skipping '{itemName}' link.");
						continue;
					}

					string ext = PathHelper.Extension(itemName);

					if (ext == null || !__supportedFiles.Contains(ext))
					{
						logger.LogInformation($"Skipping '{itemName}'.");
						continue;
					}

					files.Add(itemName);
				}
			}
		}

		private void ProcessWiXFile([NotNull] string fileName)
		{
			// wxs, wsi, xml
			Logger.LogInformation($"Processing file '{fileName}'.");

			if (!File.Exists(fileName))
			{
				Logger.LogError(Errors.FileNotFound, fileName);
				return;
			}

			try
			{
				// This will only update files that aren't readonly
				if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					Logger.LogInformation("Skipping read only file.");
					return;
				}

				bool modified = false;
				XmlDocument doc = new()
				{
					PreserveWhitespace = true
				};
				doc.Load(fileName);

				foreach (XmlElement element in doc.GetElementsByTagName("Component").Cast<XmlElement>())
				{
					element.SetAttribute("Guid", Guid.NewGuid().ToString("B").ToUpperInvariant());
					modified = true;
				}

				if (!modified) return;
				Logger.LogInformation("Saving file...");
				doc.Save(fileName);
				Logger.LogInformation("File saved.");
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.CollectMessages());
			}
		}
	}
}

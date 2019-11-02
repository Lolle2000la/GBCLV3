using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GBCLV3.Models;
using GBCLV3.Services;
using GBCLV3.Services.Installation;
using GBCLV3.Services.Launcher;
using GBCLV3.Utils;
using GBCLV3.ViewModels.Windows;
using GBCLV3.Views.Windows;
using Stylet;
using StyletIoC;

namespace GBCLV3
{
    class Bootstrapper : Bootstrapper<MainViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            builder.Bind<ConfigService>().ToSelf().InSingletonScope();
            builder.Bind<LanguageService>().ToSelf().InSingletonScope();
            builder.Bind<UpdateService>().ToSelf().InSingletonScope();
            builder.Bind<ThemeService>().ToSelf().InSingletonScope();

            builder.Bind<GamePathService>().ToSelf().InSingletonScope();
            builder.Bind<UrlService>().ToSelf().InSingletonScope();
            builder.Bind<VersionService>().ToSelf().InSingletonScope();
            builder.Bind<LibraryService>().ToSelf().InSingletonScope();
            builder.Bind<AssetService>().ToSelf().InSingletonScope();
            builder.Bind<LaunchService>().ToSelf().InSingletonScope();
            builder.Bind<ForgeInstallService>().ToSelf().InSingletonScope();
            builder.Bind<FabricInstallService>().ToSelf().InSingletonScope();
            builder.Bind<ModService>().ToSelf().InSingletonScope();
            builder.Bind<ResourcePackService>().ToSelf().InSingletonScope();

            //Custom MessageBox
            builder.Bind<IMessageBoxViewModel>().To<MessageViewModel>();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void Configure()
        {
            // Apply settings before start

            var configService = this.Container.Get<ConfigService>();
            configService.Load();

            var langService = this.Container.Get<LanguageService>();
            langService.Change(configService.Entries.Language);

            // Unable to inject the service using IoC, have to make it a static property
            LocalizedDescriptionAttribute.LanguageService = langService;

            // Override AccentColors
            var accentColor = NativeUtil.GetSystemColorByName("ImmersiveStartSelectionBackground");
            Application.Current.Resources[AdonisUI.Colors.AccentColor] = accentColor;

            Application.Current.Resources[AdonisUI.Colors.AccentInteractionColor]
                = NativeUtil.GetSystemColorByName("ImmersiveStartBackground");

            Application.Current.Resources[AdonisUI.Colors.AccentIntenseHighlightColor]
                = NativeUtil.GetSystemColorByName("ImmersiveStartFolderBackground");

            Application.Current.Resources[AdonisUI.Colors.DisabledAccentForegroundColor]
                = NativeUtil.GetSystemColorByName("ImmersiveStartDisabledText");

            Application.Current.Resources[AdonisUI.Colors.AccentHighlightColor]
                = NativeUtil.GetSystemColorByName("ImmersiveStartSecondaryText");

            // Update background image
            var themeService = this.Container.Get<ThemeService>();
            themeService.UpdateBackgroundImage();

            // Why load the background icon needs accent color?
            // Well...you'll see
            themeService.LoadBackgroundIcon(accentColor);

        }

        protected override void OnLaunch()
        {
            if (this.Args.Any() && this.Args[0] == "updated")
            {
                var windowManager = this.Container.Get<IWindowManager>();
                windowManager.ShowMessageBox("${SuccessfullyUpdatedTo} v" + AssemblyUtil.Version, "${UpdateSuccessful}",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                File.Delete("GBCL.old");
            }
            else
            {
                var configService = this.Container.Get<ConfigService>();
                if (configService.Entries.AutoCheckUpdate)
                {
                    CheckUpdateAsync();
                }
            }

            base.OnLaunch();
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            var windowManager = this.Container.Get<IWindowManager>();
            var errorReportVM = this.Container.Get<ErrorReportViewModel>();

            errorReportVM.ErrorMessage = e.Exception.ToString();
            errorReportVM.Type = ErrorReportType.UnhandledException;
            windowManager.ShowDialog(errorReportVM);

            this.Application.Shutdown(e.Exception.HResult);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var configService = this.Container.Get<ConfigService>();
            configService.Save();
            base.OnExit(e);
        }

        private async void CheckUpdateAsync()
        {
            var updateService = this.Container.Get<UpdateService>();
            var updateVM = this.Container.Get<UpdateViewModel>();
            var windowManager = this.Container.Get<IWindowManager>();

            var info = await updateService.Check();

            if (info != null)
            {
                updateVM.Setup(info);
                windowManager.ShowWindow(updateVM);
            }
        }
    }
}

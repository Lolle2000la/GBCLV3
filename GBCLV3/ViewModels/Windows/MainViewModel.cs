using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using GBCLV3.Models;
using GBCLV3.Services;
using GBCLV3.Utils;
using GBCLV3.ViewModels.Pages;
using Stylet;

namespace GBCLV3.ViewModels.Windows
{
    class MainViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<UsernameChangedEvent>
    {
        #region Private Members

        private readonly Screen[] _pages;

        // IoC
        private readonly IWindowManager _windowManager;
        private readonly Config _config;

        #endregion

        #region Constructor

        public MainViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator,

            ConfigService configService,
            ThemeService themeService,

            LaunchViewModel launchVM,
            SettingsRootViewModel settingsVM,
            VersionsRootViewModel versionsVM,
            AccessoriesViewModel modVM,
            AboutViewModel aboutVM)
        {
            _windowManager = windowManager;
            _config = configService.Entries;

            _pages = new Screen[]
            {
                launchVM, settingsVM, versionsVM, modVM, aboutVM,
            };

            // Start up with LaunchView
            ActivePageIndex = 0;

            // Bind background image service
            ThemeService = themeService;

            //Set Title
            this.DisplayName = "GBCL v" + AssemblyUtil.Version;

            //Subscribe Events
            eventAggregator.Subscribe(this);
        }

        #endregion

        #region Bindings

        public ThemeService ThemeService { get; private set; }

        public string Username => _config.Username;

        public int ActivePageIndex { get; set; }

        public void ChangeActivePage()
        {
            this.ActivateItem(_pages[ActivePageIndex]);
        }

        #endregion

        #region Override Methods

        public void Handle(UsernameChangedEvent message)
        {
            NotifyOfPropertyChange(nameof(Username));
        }

        #endregion

    }
}

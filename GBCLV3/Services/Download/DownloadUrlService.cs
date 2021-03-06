﻿using GBCLV3.Models;
using GBCLV3.Models.Download;
using StyletIoC;

namespace GBCLV3.Services.Download
{
    public class DownloadUrlService
    {
        #region Properties

        public IDownloadUrlBase Base => _config.DownloadSource switch
        {
            DownloadSource.Official => _officialUrlBase,
            DownloadSource.BMCLAPI => _bmclapiUrlBase,
            DownloadSource.MCBBS => _mcbbsUrlBase,
            _ => _officialUrlBase,
        };

        #endregion

        #region Private Fields

        private readonly Config _config;

        private static readonly OfficialUrlBase _officialUrlBase = new OfficialUrlBase();
        private static readonly BMCLAPIUrlBase _bmclapiUrlBase = new BMCLAPIUrlBase();
        private static readonly MCBBSUrlBase _mcbbsUrlBase = new MCBBSUrlBase();

        #endregion

        #region Constructor

        [Inject]
        public DownloadUrlService(ConfigService configService)
        {
            _config = configService.Entries;
        }

        #endregion
    }
}

﻿using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using System.Collections.Generic;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Infrastructure;
using Nop.Services.Events;
using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using System.Net.Http;
using Task = System.Threading.Tasks.Task;
using Nop.Plugin.Widgets.Bronto.Services;

namespace Nop.Plugin.Widgets.Bronto
{
    public class BrontoPlugin : BasePlugin, IWidgetPlugin, IConsumer<EmailSubscribedEvent>,
                                                           IConsumer<EmailUnsubscribedEvent>
    {
        private readonly IBrontoEmailListService _brontoEmailListService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly BrontoSettings _brontoSettings;

        public BrontoPlugin(
            IBrontoEmailListService brontoEmailListService,
            IWebHelper webHelper,
            ISettingService settingService,
            ILocalizationService localizationService,
            ILogger logger,
            BrontoSettings brontoSettings
        )
        {
            _brontoEmailListService = brontoEmailListService;
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _logger = logger;
            _brontoSettings = brontoSettings;
        }

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Bronto/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "Bronto";
        }

        public System.Threading.Tasks.Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.Footer });
        }

        public async Task HandleEventAsync(EmailSubscribedEvent eventMessage)
        {
            await _brontoEmailListService.SubscribeEmailAsync(eventMessage.Subscription.Email);
        }

        public async Task HandleEventAsync(EmailUnsubscribedEvent eventMessage)
        {
            await _brontoEmailListService.UnsubscribeEmailAsync(eventMessage.Subscription.Email);
        }

        public override async Task InstallAsync()
        {
            await AddLocalesAsync();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<BrontoSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync(BrontoPluginLocaleKeys.Base);

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await AddLocalesAsync();

            await base.UpdateAsync(currentVersion, targetVersion);
        }

        private async Task AddLocalesAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                [BrontoPluginLocaleKeys.ScriptManagerCode] = "Script Manager Code",
                [BrontoPluginLocaleKeys.ScriptManagerCodeHint] = "The Script Manager code provided by Bronto.",
                [BrontoPluginLocaleKeys.DirectAddListId] = "Direct Add List ID",
                [BrontoPluginLocaleKeys.DirectAddListIdHint] = "The List ID of the users that opt-in to marketing.",
            });
        }
    }
}

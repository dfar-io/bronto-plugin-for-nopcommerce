using System.Threading.Tasks;
using Nop.Plugin.Widgets.Bronto.Domain;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using System.Net.Http;
using Nop.Core.Http;

namespace Nop.Plugin.Widgets.Bronto.Services
{
    public class BrontoEmailListService : IBrontoEmailListService
    {
        private readonly BrontoSettings _brontoSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public BrontoEmailListService(
            BrontoSettings brontoSettings,
            IHttpClientFactory httpClientFactory,
            ILogger logger
        )
        {
            _brontoSettings = brontoSettings;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task SubscribeEmailAsync(string email)
        {
            await ValidateSettingsAsync();
            await GetRequestAsync("direct_add", "Public_DirectAddForm", email);
        }

        public async Task UnsubscribeEmailAsync(string email)
        {
            await ValidateSettingsAsync();
            await GetRequestAsync("direct_unsub", "Public_DirectUnsubForm", email);
        }

        private async Task ValidateSettingsAsync()
        {
            if (string.IsNullOrWhiteSpace(_brontoSettings.DirectAddListId))
            {
                await _logger.ErrorAsync($"Unable to update email in Bronto - " +
                               "please set the Direct Add List ID in Settings.");
                return;
            }
        }

        private async Task GetRequestAsync(string q, string fn, string email)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response =
                await client.GetAsync(
                    $"https://app.bronto.com/public/?q={q}&fn={fn}&id={_brontoSettings.DirectAddListId}&email={email}");
            response.EnsureSuccessStatusCode();
        }
    }
}

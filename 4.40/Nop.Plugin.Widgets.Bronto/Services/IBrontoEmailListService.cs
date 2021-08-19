using System.Threading.Tasks;
using Nop.Plugin.Widgets.Bronto.Domain;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Widgets.Bronto.Services
{
    public interface IBrontoEmailListService
    {
        Task SubscribeEmailAsync(string email);
        Task UnsubscribeEmailAsync(string email);
    }
}

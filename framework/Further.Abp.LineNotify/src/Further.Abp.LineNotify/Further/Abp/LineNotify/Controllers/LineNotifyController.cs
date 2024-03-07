using Further.Abp.LineNotify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.EventBus.Distributed;

namespace Further.Abp.LineNotify.Controllers
{
    [Area("LineNotify")]
    [RemoteService(Name = "LineNotify")]
    [Route("api/line-notify")]
    public class LineNotifyController : AbpControllerBase, IRemoteService
    {
        private readonly LineNotifyOptions options;
        private readonly ILineNotifyHttpClient lineNotifyHttpClient;
        private readonly IDistributedEventBus distributedEventBus;
        private readonly IAccessTokenProvider accessTokenProvider;

        public LineNotifyController(
            IOptions<LineNotifyOptions> options,
            ILineNotifyHttpClient lineNotifyHttpClient,
            IDistributedEventBus distributedEventBus,
            IAccessTokenProvider accessTokenProvider)
        {
            this.options = options.Value;
            this.lineNotifyHttpClient = lineNotifyHttpClient;
            this.distributedEventBus = distributedEventBus;
            this.accessTokenProvider = accessTokenProvider;
        }

        [HttpGet("authorize-url")]
        public async Task<string> AuthorizeUrlAsync(string returnUrl, string subject = LineNotifyConsts.DefaultSubject, string configuratorName = LineNotifyConsts.DefaultConfiguratorName)
        {
            var url = await lineNotifyHttpClient.AuthorizeAsync(
                returnUrl: returnUrl,
                configuratorName: configuratorName,
                subject: subject);

            return url;
        }

        [HttpGet("token")]
        public async Task<string> TokenAsync(string code, string configuratorName = LineNotifyConsts.DefaultConfiguratorName)
        {
            var token = await lineNotifyHttpClient.TokenAsync(code, configuratorName);

            return token;
        }

        [HttpGet("notify")]
        public async Task NotifyAsync(string message, string subject = LineNotifyConsts.DefaultSubject, string configuratorName = LineNotifyConsts.DefaultConfiguratorName)
        {
            await lineNotifyHttpClient.NotifyAsync(
                message: message,
                configuratorName: configuratorName,
                subject: subject);
        }

        [HttpGet("redirect")]
        public async Task<RedirectResult> RedirectAsync(string code, string state)
        {
            var stateParts = LineNotifyConsts.DecodeState(state);

            var token = await lineNotifyHttpClient.TokenAsync(code, stateParts.ConfiguratorName);

            await accessTokenProvider.SetAccessTokenAsync(
                configuratorName: stateParts.ConfiguratorName,
                subject: stateParts.Subject,
                token: token);

            await distributedEventBus.PublishAsync(new LoginReturnEto
            {
                State = state,
                ConfiguratorsName = stateParts.ConfiguratorName,
                subject = stateParts.Subject,
                Code = code,
                Token = token,
                ResultUrl = stateParts.ReturnUrl
            });

            var result = $"{stateParts.ReturnUrl}?subject={stateParts.Subject}&configuratorName={stateParts.ConfiguratorName}";

            return Redirect(new Uri(result).ToString());

        }
    }
}

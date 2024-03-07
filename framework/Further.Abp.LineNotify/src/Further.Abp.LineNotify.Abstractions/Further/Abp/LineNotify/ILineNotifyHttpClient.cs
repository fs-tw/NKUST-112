using System.Threading.Tasks;

namespace Further.Abp.LineNotify
{
    public interface ILineNotifyHttpClient
    {
        Task<string> AuthorizeAsync(string returnUrl, string configuratorName = LineNotifyConsts.DefaultConfiguratorName, string subject = LineNotifyConsts.DefaultSubject);
        Task NotifyAsync(string message, string configuratorName = LineNotifyConsts.DefaultConfiguratorName, string subject = LineNotifyConsts.DefaultSubject);
        Task<string> TokenAsync(string code, string configuratorName = LineNotifyConsts.DefaultConfiguratorName);
    }
}
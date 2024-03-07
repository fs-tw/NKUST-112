using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Further.Abp.LineNotify
{
    public class LineNotifyConsts
    {
        public const string DefaultConfiguratorName = "Default";
        public const string DefaultSubject = "Default";
        public const string HttpClientName = "AbpLineNotify";
        public static string AccessTokenCacheName(string configuratorName, string subject) => $"AbpLineNotify.AccessToken:{configuratorName}:{subject}";
        public static string EncodeState(string configuratorName, string subject, string returnUrl) => $"{configuratorName};{subject};{returnUrl}";

        public static (string ConfiguratorName, string Subject, string ReturnUrl) DecodeState(string state)
        {
            var parts = state.Split(';');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid state");
            }
            return (parts[0], parts[1], parts[2]);
        }
    }
}

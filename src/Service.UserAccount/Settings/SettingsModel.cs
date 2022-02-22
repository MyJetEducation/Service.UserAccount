using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.UserAccount.Settings
{
    public class SettingsModel
    {
        [YamlProperty("UserAccount.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("UserAccount.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("UserAccount.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("UserAccount.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("UserAccount.ServiceBusWriter")]
        public string ServiceBusWriter { get; set; }
    }
}

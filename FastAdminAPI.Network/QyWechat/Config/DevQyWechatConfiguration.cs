namespace FastAdminAPI.Network.QyWechat.Config
{
    internal class DevQyWechatConfiguration : BaseQyWechatConfiguration
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        internal override string Corpid { get; set; } = "testcopid";
        /// <summary>
        /// 应用的凭证密钥
        /// </summary>
        internal override string CorpSecret { get; set; } = "testcorpsecret";
        /// <summary>
        /// 企业应用的id
        /// </summary>
        internal override int Agentid { get; set; } = 1000000;
    }
}

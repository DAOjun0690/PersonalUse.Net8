namespace BCVP.Net8.Common.Option
{
    /// <summary>
    /// Redis記憶體設定選項
    /// </summary>
    public sealed class RedisOptions : IConfigurableOptions
    {
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Redis連接
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Key前綴
        /// </summary>
        public string InstanceName { get; set; }
    }

}

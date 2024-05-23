namespace BCVP.Net8.Common
{
    public class AOPLogInfo
    {
        /// <summary>
        /// 請求時間
        /// </summary>
        public string RequestTime { get; set; } = string.Empty;
        /// <summary>
        /// 操作人員
        /// </summary>
        public string OpUserName { get; set; } = string.Empty;
        /// <summary>
        /// 請求函式名
        /// </summary>
        public string RequestMethodName { get; set; } = string.Empty;
        /// <summary>
        /// 請求參數名
        /// </summary>
        public string RequestParamsName { get; set; } = string.Empty;
        /// <summary>
        /// 請求參數資料JSON
        /// </summary>
        public string RequestParamsData { get; set; } = string.Empty;
        /// <summary>
        /// 請求響應間隔時間
        /// </summary>
        public string ResponseIntervalTime { get; set; } = string.Empty;
        /// <summary>
        /// 響應時間
        /// </summary>
        public string ResponseTime { get; set; } = string.Empty;
        /// <summary>
        /// 響應結果
        /// </summary>
        public string ResponseJsonData { get; set; } = string.Empty;
    }
}

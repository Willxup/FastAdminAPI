using System;

namespace FastAdminAPI.Common.BASE
{
    /// <summary>
    /// 通用返回信息
    /// </summary>
    public class ResponseModel
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 响应码
        /// </summary>
        public ResponseCode Code { get; set; } = ResponseCode.Success;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = MessageModel.Success;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; } = new object();

        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 分页总数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        public string Mac { get; set; } = string.Empty;

        /// <summary>
        ///  时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 构造
        /// </summary>
        public ResponseModel() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="code">响应码</param>
        public ResponseModel(ResponseCode code)
        {
            Code = code;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data">数据</param>
        public ResponseModel(object data)
        {
            Data = data;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="code">响应码</param>
        /// <param name="msg">消息</param>
        public ResponseModel(ResponseCode code, string msg) : this(code)
        {
            Message = msg;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="code">响应码</param>
        /// <param name="msg">消息</param>
        /// <param name="data">数据</param>
        public ResponseModel(ResponseCode code, string msg, object data) : this(code, msg)
        {
            Data = data;
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ResponseModel Success(object data = null) => new(data);
        /// <summary>
        /// 成功(仅消息)
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ResponseModel SuccessMessage(string message) => new(ResponseCode.Success, message);
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="code">响应码</param>
        /// <returns></returns>
        public static ResponseModel Error(string msg, ResponseCode code = ResponseCode.Error) => new(code, msg);
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="code">响应码</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ResponseModel Warn(string msg, ResponseCode code = ResponseCode.Warn, object data = null) => new(code, msg, data);
    }

    public static class ResponseModelExtension
    {
        /// <summary>
        /// 类型转为通用返回信息体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="totalPage">总页数</param>
        /// <returns></returns>
        public static ResponseModel ToResponseModel<T>(this T data, int totalCount = 0, int totalPage = 0) where T : class, new()
        {
            return new ResponseModel(data)
            {
                DataCount = totalCount,
                PageCount = totalPage
            };
        }
        /// <summary>
        /// 通用返回信息体转换data数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">返回结果</param>
        /// <returns></returns>
        public static T ToConvertData<T>(this ResponseModel result) where T : class, new()
        {
            return result.Data is T data ? data : default;
        }
    }

}

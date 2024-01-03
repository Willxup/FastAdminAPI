using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.TokenPass
{
    public class MarketingActivityInfoModel
    {
        /// <summary>
        /// 营销活动推广Id
        /// </summary>
        public long ActivityId { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 活动地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 报名页面链接
        /// </summary>
        public string RegisterLink { get; set; }
        /// <summary>
        /// 活动形式
        /// </summary>
        public string Form { get; set; }
        /// <summary>
        /// 活动目标
        /// </summary>
        public string Goal { get; set; }
        /// <summary>
        /// 活动方案
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// 活动结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 优化方向
        /// </summary>
        public string Optimization { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 营销模板Id
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 营销模板链接
        /// </summary>
        public string TemplateLink { get; set; }
        /// <summary>
        /// 营销模板背景图
        /// </summary>
        public string Background { get; set; }
    }

}


using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///员工
    ///</summary>
    [SugarTable("S07_Employee")]
    [Serializable]
    public partial class S07_Employee : BaseEntity
    {
        public S07_Employee()
        {
            this.S07_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:员工Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S07_EmployeeId { get; set; }
        /// <summary>
        /// Desc:用户Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S01_UserId { get; set; }
        /// <summary>
        /// Desc:企业Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S10_CompanyId { get; set; }
        /// <summary>
        /// Desc:企业微信UserId
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_QyUserId { get; set; }
        /// <summary>
        /// Desc:姓名
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S07_Name { get; set; }
        /// <summary>
        /// Desc:手机
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S07_Phone { get; set; }
        /// <summary>
        /// Desc:性别 0男 1女
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? S07_Gender { get; set; }
        /// <summary>
        /// Desc:头像
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_Avatar { get; set; }
        /// <summary>
        /// Desc:邮箱
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_Email { get; set; }
        /// <summary>
        /// Desc:类别 0全职 1兼职
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? S07_Kind { get; set; }
        /// <summary>
        /// Desc:状态 0正式 1实习 2离职
        /// Default:
        /// Nullable:True
        /// </summary>

        public byte? S07_Status { get; set; }
        /// <summary>
        /// Desc:个人简介
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_Bio { get; set; }
        /// <summary>
        /// Desc:入职日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_HireDate { get; set; }
        /// <summary>
        /// Desc:试用期日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_TrialPeriodDate { get; set; }
        /// <summary>
        /// Desc:转正日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_ConfirmationDate { get; set; }
        /// <summary>
        /// Desc:离职日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_TerminationDate { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S07_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S07_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S07_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S07_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S07_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S07_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S07_DeleteTime { get; set; }
    }
}

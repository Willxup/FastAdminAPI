
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///员工岗位表
    ///</summary>
    [SugarTable("S08_EmployeePost")]
    [Serializable]
    public partial class S08_EmployeePost : BaseEntity
    {
        public S08_EmployeePost()
        {
            this.S08_IsMainPost = Convert.ToByte("0");
            this.S08_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:员工岗位Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S08_EmployeePostId { get; set; }
        /// <summary>
        /// Desc:员工Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S07_EmployeeId { get; set; }
        /// <summary>
        /// Desc:岗位Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S06_PostId { get; set; }
        /// <summary>
        /// Desc:是否为主岗位 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S08_IsMainPost { get; set; }
        /// <summary>
        /// Desc:部门Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S05_DepartId { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S08_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S08_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S08_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S08_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S08_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S08_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S08_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S08_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S08_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S08_DeleteTime { get; set; }
    }
}

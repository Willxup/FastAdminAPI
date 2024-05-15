
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///登录记录
    ///</summary>
    [SugarTable("S14_LoginRecords")]
    [Serializable]
    public partial class S14_LoginRecords:BaseEntity
    {
        public S14_LoginRecords()
        {

        }
           /// <summary>
           /// Desc:登录记录Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long S14_LoginRecordId{get;set;}
           /// <summary>
           /// Desc:用户Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long S01_UserId{get;set;}
           /// <summary>
           /// Desc:员工Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long S07_EmployeeId{get;set;}
           /// <summary>
           /// Desc:终端 0PC
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? S14_Device{get;set;}
           /// <summary>
           /// Desc:时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime S14_Time{get;set;}
    }
}

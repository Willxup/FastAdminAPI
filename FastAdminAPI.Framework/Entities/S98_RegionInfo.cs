
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    /// 区域
    ///</summary>
    [SugarTable("S98_RegionInfo")]
    [Serializable]
    public partial class S98_RegionInfo:BaseEntity
    {
        public S98_RegionInfo()
        {

        }
           /// <summary>
           /// Desc:区域Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long S98_REGION_ID{get;set;}
           /// <summary>
           /// Desc:区域Code
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string S98_REGION_CODE{get;set;}
           /// <summary>
           /// Desc:区域名称
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string S98_REGION_NAME{get;set;}
           /// <summary>
           /// Desc:父级Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long S98_PARENT_ID{get;set;}
           /// <summary>
           /// Desc:区域等级
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long S98_REGION_LEVEL{get;set;}
           /// <summary>
           /// Desc:区域排序
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long S98_REGION_ORDER{get;set;}
           /// <summary>
           /// Desc:区域英文名称
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string S98_REGION_NAME_EN{get;set;}
           /// <summary>
           /// Desc:区域英文短名
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string S98_REGION_SHORTNAME_EN{get;set;}
    }
}

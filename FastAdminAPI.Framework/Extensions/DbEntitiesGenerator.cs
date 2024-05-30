using SqlSugar;
using System.IO;

namespace FastAdminAPI.Framework.Extensions
{
    public static class DbEntitiesGenerator
    {
        /// <summary>
        /// 生成数据库实体类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="serviceName">服务名称</param>
        public static void GenerateDbEntities(ISqlSugarClient db, string serviceName = "FastAdminAPI.Core")
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Replace(serviceName, "");
            GenerateDbEntitiesByCustom(db, path + @"FastAdminAPI.Framework\Entities", "FastAdminAPI.Framework.Entities", null, "", true);
        }
        /// <summary>
        /// <summary>
        /// 按指定表生成数据库实体类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tables">需要生成的表</param>
        /// <param name="serviceName">服务名称</param>
        public static void GenerateDbEntitiesByAssignTables(ISqlSugarClient db, string[] tables = null, string serviceName = "FastAdminAPI.Core")
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Replace(serviceName, "");
            GenerateDbEntitiesByCustom(db, path + @"FastAdminAPI.Framework\Entities", "FastAdminAPI.Framework.Entities", tables, "", true);
        }
        /// <summary>
        /// 自定义生成数据库实体类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tables">需要生成的表</param>
        /// <param name="interfaceName">继承的接口/类</param>
        /// <param name="isSerializable">是否序列化(默认否)</param>
        /// <param name="tableNameStartWith">以[当前字符串]开头的表</param>
        public static void GenerateDbEntitiesByCustom(ISqlSugarClient db, string[] tables = null,
            string interfaceName = null, bool isSerializable = false, string tableNameStartWith = null)
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Replace("FastAdminAPI.Core", "");

            GenerateDbEntitiesByCustom(db, path + @"FastAdminAPI.Framework\Entities", "FastAdminAPI.Framework.Entities", tables, interfaceName, isSerializable, tableNameStartWith);
        }
        /// <summary>
        /// 自定义生成数据库实体类
        /// </summary>
        /// <param name="path">生成路径</param>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="tables">指定表名</param>
        /// <param name="interfaceName">继承的接口/类</param>
        /// <param name="isSerializable">是否序列化(默认否)</param>
        /// <param name="tableNameStartWith">以[当前字符串]开头的表</param>
        private static void GenerateDbEntitiesByCustom(ISqlSugarClient db, string path, string nameSpace, string[] tables = null,
            string interfaceName = null, bool isSerializable = false, string tableNameStartWith = null)
        {
            path = !string.IsNullOrEmpty(path) ? path : Directory.GetCurrentDirectory() + @"FastAdminAPI.Framework\Entities";
            nameSpace = !string.IsNullOrEmpty(nameSpace) ? nameSpace : "FastAdminAPI.Framework.Entities";

            if (tables != null && tables.Length > 0)
            {
                db.DbFirst.Where(tables).IsCreateDefaultValue().IsCreateAttribute()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (isSerializable ? "\r\n    [Serializable]" : "") + @"
    public partial class {ClassName}" +
                                                   (string.IsNullOrEmpty(interfaceName)
                                                       ? ":BaseEntity"
                                                       : (" :BaseEntity, " + interfaceName)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
        {SugarColumn}
        public {PropertyType} {PropertyName}{get;set;}")
                    //.SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    //.SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(path, nameSpace);
            }
            else
            {
                var generate = db.DbFirst;
                if (!string.IsNullOrEmpty(tableNameStartWith))
                {
                    generate = generate.Where(t => t.StartsWith(tableNameStartWith));
                }
                generate.IsCreateAttribute().IsCreateDefaultValue()
                    .SettingClassTemplate(p => p = @"
{using}

namespace {Namespace}
{
    {ClassDescription}{SugarTable}" + (isSerializable ? "\r\n    [Serializable]" : "") + @"
    public partial class {ClassName}" +
                                                   (string.IsNullOrEmpty(interfaceName)
                                                       ? ":BaseEntity"
                                                       : (" :BaseEntity, " + interfaceName)) + @"
    {
        public {ClassName}()
        {
{Constructor}
        }
{PropertyName}
    }
}
")
                    .SettingPropertyTemplate(p => p = @"
        {SugarColumn}
        public {PropertyType} {PropertyName}{get;set;}")
                    //.SettingPropertyDescriptionTemplate(p => p = "          private {PropertyType} _{PropertyName};\r\n" + p)
                    //.SettingConstructorTemplate(p => p = "              this._{PropertyName} ={DefaultValue};")
                    .CreateClassFile(path, nameSpace);
            }
        }
    }
}

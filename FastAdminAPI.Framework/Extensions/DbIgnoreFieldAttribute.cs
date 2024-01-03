using System;

namespace FastAdminAPI.Framework.Extensions
{
    /// <summary>
    /// 忽略字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbIgnoreFieldAttribute : Attribute
    {
        public DbIgnoreFieldAttribute() { }
    }
}

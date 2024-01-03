using SqlSugar;
using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库操作符
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbQueryOperatorAttribute : Attribute
    {
        private readonly DbOperator _operateSymbol;
        private readonly ConditionalType _operator;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="operateSymbol">操作符</param>
        public DbQueryOperatorAttribute(DbOperator operateSymbol)
        {
            _operateSymbol = operateSymbol;
            _operator = ConvertDbOperator();
        }

        private ConditionalType ConvertDbOperator()
        {
            return _operateSymbol switch
            {
                DbOperator.Equal => ConditionalType.Equal,
                DbOperator.Like => ConditionalType.Like,
                DbOperator.GreaterThan => ConditionalType.GreaterThan,
                DbOperator.GreaterThanOrEqual => ConditionalType.GreaterThanOrEqual,
                DbOperator.LessThan => ConditionalType.LessThan,
                DbOperator.LessThanOrEqual => ConditionalType.LessThanOrEqual,
                DbOperator.In => ConditionalType.In,
                DbOperator.NotIn => ConditionalType.NotIn,
                DbOperator.LikeLeft => ConditionalType.LikeLeft,
                DbOperator.LikeRight => ConditionalType.LikeRight,
                DbOperator.NoEqual => ConditionalType.NoEqual,
                DbOperator.IsNullOrEmpty => ConditionalType.IsNullOrEmpty,
                DbOperator.IsNot => ConditionalType.IsNot,
                DbOperator.NoLike => ConditionalType.NoLike,
                DbOperator.EqualNull => ConditionalType.EqualNull,
                DbOperator.InLike => ConditionalType.InLike,
                _ => throw new NotImplementedException($"Not Support [{_operateSymbol}] operator!")
            };
        }

        public ConditionalType GetDbOperator()
        {
            return _operator;
        }
    }


}

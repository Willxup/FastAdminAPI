using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Datetime;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Framework.Extensions.Models;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FastAdminAPI.Framework.Extensions
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class DbExtension
    {
        #region 配置
        /// <summary>
        /// 统一配置sqlsugar
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static SqlSugarScope ConfigSqlSugar(string connectionString)
        {
            StaticConfig.EnableAllWhereIF = true;
            return new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true, //自动释放
                LanguageType = LanguageType.English
            }, ConfigSqlSugarSetting);
        }
        /// <summary>
        /// 配置sqlsugar设置
        /// </summary>
        /// <param name="db"></param>
        public static void ConfigSqlSugarSetting(SqlSugarClient db)
        {
            db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings() { IsAutoRemoveDataCache = true };
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                SqlFuncServices = CustomSqlFunc()
            };
            if (EnvTool.IsDevelop)
            {
                db.Aop.OnLogExecuting = (sql, parameters) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(DateTime.Now.ToFormattedString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("【SQL】：");
                    Console.ResetColor();
                    Console.WriteLine(sql);
                    if (parameters != null && parameters?.Length > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(DateTime.Now.ToFormattedString());
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("【Paramters】：");
                        Console.ResetColor();
                        Console.WriteLine(string.Join(",", parameters?.Select(it => "【" + it.ParameterName + "=" + it.Value + "】")));
                    }
                };
            }
        }

        #region 自定义扩展
        /// <summary>
        /// SqlSugar自定义扩展
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<SqlFuncExternal> CustomSqlFunc()
        {
            List<SqlFuncExternal> resultList = new()
            {
                //GROUP_CONCAT
                //用法：.Select(a => DbExtension.GroupConcat(a.Name, ","))
                new SqlFuncExternal
                {
                    UniqueMethodName = "GroupConcat",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                        {
                            return $@"GROUP_CONCAT( {expInfo.Args[0].MemberName} SEPARATOR '{expInfo.Args[1].MemberValue}' ) ";
                        }
                        else
                        {
                            throw new NotSupportedException("Not Support this database");
                        }
                    }
                },
                //FIND_IN_SET
                //用法：.Where(a => DbExtension.FindInSet("1", a.Name))
                new SqlFuncExternal
                {
                    UniqueMethodName = "FindInSet",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if(dbType == DbType.MySql)
                        {
                            return $"FIND_IN_SET('{expInfo.Args[0].MemberValue}', {expInfo.Args[1].MemberName} ) > 0";
                        }
                        else
                        {
                             throw new NotSupportedException("Not Support this database");
                        }
                    }
                }
            };

            return resultList;
        }
        /// <summary>
        /// GROUP_CONCAT
        /// 用法：.Select(a => DbExtension.GroupConcat(a.Name, ","))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">字段</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static string GroupConcat<T>(T column, string separator)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        /// <summary>
        /// FIND_IN_SET
        /// 用法：.Where(a => DbExtension.FindInSe("1", a.Name))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static bool FindInSet<T>(T column, string separator)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        #endregion

        #endregion

        #region 通用方法

        #region 列表查询
        /// <summary>
        /// 自动装箱查询列表(不带分页)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search">查询条件</param>
        /// <param name="result">查询结果</param>
        /// <param name="index">页数</param>
        /// <param name="size">行数</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="totalPage">总页数</param>
        /// <returns></returns>
        public static async Task<List<TResult>> ToListAsync<T, TSearch, TResult>(this ISugarQueryable<T> queryable, TSearch search, TResult result, 
            int? index, int? size, RefAsync<int> totalCount = null, RefAsync<int> totalPage = null)
            where TSearch : DbQueryBaseModel
        {
            var query = queryable.Where(search).Select(result)
                        .GroupBy(result).Having(result)
                        .OrderBy(search);

            //是否需要分页
            if (index != null && size != null)
            {
                //是否需要返回总页数和总行数
                if(totalCount != null && totalPage != null)
                {
                    return await query.ToPageListAsync((int)index, (int)size, totalCount, totalPage);
                }
                else
                {
                    return await query.ToPageListAsync((int)index, (int)size);
                }
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        /// <summary>
        /// 灵活查询 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">SqlSugar查询条件</param>
        /// <param name="index">页数</param>
        /// <param name="size">行数</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ToListResultAsync<T>(this ISugarQueryable<T> queryable, int? index = null, int? size = null)
        {
            try
            {
                if (index != null && size != null)
                {
                    RefAsync<int> totalCount = 0;
                    RefAsync<int> totalPage = 0;
                    var result = await queryable.ToPageListAsync((int)index, (int)size, totalCount, totalPage);
                    ResponseModel response = new(result)
                    {
                        DataCount = totalCount,
                        PageCount = totalPage
                    };
                    return response;
                }
                else
                {
                    var result = await queryable.ToListAsync();
                    return ResponseModel.Success(result);

                }
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(ex.Message);
            }
        }
        /// <summary>
        /// 自动装箱查询 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search">查询条件</param>
        /// <param name="result">查询结果</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ToListResultAsync<T, TSearch, TResult>(this ISugarQueryable<T> queryable, TSearch search, TResult result)
            where TSearch : DbQueryBaseModel
        {
            try
            {
                var query = queryable.Where(search).Select(result).GroupBy(result).Having(result).OrderBy(search);

                if (search.Index != null && search.Size != null)
                {
                    RefAsync<int> totalCount = 0;
                    RefAsync<int> totalPage = 0;
                    var list = await query.ToPageListAsync((int)search.Index, (int)search.Size, totalCount, totalPage);
                    ResponseModel response = new(list)
                    {
                        DataCount = totalCount,
                        PageCount = totalPage
                    };
                    return response;
                }
                else
                {
                    var list = await query.ToListAsync();
                    return ResponseModel.Success(list);
                }
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(ex.Message);
            }
        }
        #endregion

        #region DTO查询
        /// <summary>
        /// 自动装箱查询DTO 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="result">查询结果</param>
        /// <returns></returns>
        public static async Task<TResult> ToFirstAsync<T, TResult>(this ISugarQueryable<T> queryable, TResult result)
        {
            return await queryable.Select(result).FirstAsync();
        }
        /// <summary>
        /// 自动装箱条件查询DTO 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search">查询条件</param>
        /// <param name="result">查询结果</param>
        /// <returns></returns>
        public static async Task<TResult> ToFirstAsync<T, TSearch, TResult>(this ISugarQueryable<T> queryable, TSearch search, TResult result)
            where TSearch : class, new()
        {
            return await queryable.Where(search).Select(result).FirstAsync();
        }
        /// <summary>
        /// 自动装箱查询DTO 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="result">查询结果</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ToFirstResultAsync<T, TResult>(this ISugarQueryable<T> queryable, TResult result)
        {
            try
            {
                var query = await queryable.Select(result).FirstAsync();

                return ResponseModel.Success(query);
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(ex.Message);
            }
        }
        /// <summary>
        /// 自动装箱条件查询DTO 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search">查询条件</param>
        /// <param name="result">查询结果</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ToFirstResultAsync<T, TSearch, TResult>(this ISugarQueryable<T> queryable, TSearch search, TResult result)
            where TSearch : class, new()
        {
            try
            {
                var query = await queryable.Where(search).Select(result).FirstAsync();

                return ResponseModel.Success(query);
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(ex.Message);
            }
        }
        #endregion

        #region 插入
        /// <summary>
        /// DTO新增 直接返回通用消息类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="db"></param>
        /// <param name="dto">dto模型</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> InsertResultAsync<TDto, TEntity>(this SqlSugarScope db, TDto dto, string errorMessage = null)
            where TDto : DbOperationBaseModel, new()
            where TEntity : class, new()
        {
            return await db.Insertable<TDto, TEntity>(dto).ExecuteAsync(errorMessage);
        }
        /// <summary>
        /// 自定义新增 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugar插入条件</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IInsertable<T> db, string errorMessage = null)
            where T : class, new()
        {
            try
            {
                var provider = db as InsertableProvider<T>;
                if (provider.InsertObjs?.Length > 1)
                {
                    var res = await db.ExecuteCommandAsync();
                    if (res > 0)
                        return ResponseModel.Success(res);
                    else
                        throw new UserOperationException("未插入任何数据!");
                }
                else
                {
                    //查看是否有自增主键
                    if (provider.EntityInfo.Columns?.Any(c => c.IsIdentity == true) ?? false)
                    {
                        //有自增 返回主键
                        var res = await db.ExecuteReturnBigIdentityAsync();
                        if (res > 0)
                            return ResponseModel.Success(res);
                        else
                            throw new UserOperationException("未插入任何数据!");
                    }
                    else
                    {
                        //无自增 不返回主键
                        var res = await db.ExecuteCommandAsync();
                        if (res > 0)
                            return ResponseModel.Success(res);
                        else
                            throw new UserOperationException("未插入任何数据!");
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        /// <summary>
        /// 大数据写入 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">大数据写入</param>
        /// <param name="data">要插入的数据</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IFastest<T> db, List<T> data, string errorMessage = null)
            where T : class, new()
        {
            try
            {
                var res = await db.BulkCopyAsync(data);
                if (res > 0)
                    return ResponseModel.Success(res);
                else
                    throw new UserOperationException("未插入任何数据!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        #endregion

        #region 更新
        /// <summary>
        /// DTO更新 直接返回通用消息类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="db"></param>
        /// <param name="dto">dto模型</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> UpdateResultAsync<TDto, TEntity>(this SqlSugarScope db, TDto dto, bool isCheckAffectedRows = true, string errorMessage = null)
            where TDto : DbOperationBaseModel, new()
            where TEntity : class, new()
        {
            return await db.Updateable<TDto, TEntity>(dto).ExecuteAsync(isCheckAffectedRows, errorMessage);
        }
        /// <summary>
        /// 自定义更新 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugar更新条件</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IUpdateable<T> db, bool isCheckAffectedRows = true, string errorMessage = null)
            where T : class, new()
        {
            try
            {
                var res = await db.ExecuteCommandAsync();
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("更新失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// DTO软删除 直接返回通用消息类
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="db"></param>
        /// <param name="dto">dto模型</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> SoftDeleteAsync<TDto, TEntity>(this SqlSugarScope db, TDto dto, bool isCheckAffectedRows = true, string errorMessage = null)
            where TDto : DbOperationBaseModel, new()
            where TEntity : class, new()
        {
            return await db.Updateable<TDto, TEntity>(dto).ExecuteAsync(isCheckAffectedRows, errorMessage);
        }
        /// <summary>
        /// 软删除 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> SoftDeleteAsync<T>(this IDeleteable<T> db, Expression<Func<T, T>> updateColumns, bool isCheckAffectedRows = true, string errorMessage = null)
            where T : class, new()
        {
            //获取provider，provider用于获取update更新
            var provider = db.IsLogic();

            //获取where条件和参数
            var where = provider.DeleteBuilder.GetWhereString[5..]; //去除where
            var pars = provider.DeleteBuilder.Parameters;

            //获取IUpdateable<T>
            IUpdateable<T> updateTable = provider.Deleteable.Context.Updateable<T>();

            //参数赋值
            if (pars != null)
            {
                updateTable.UpdateBuilder.Parameters.AddRange(pars);
            }

            //校验更新列
            if (updateColumns == null)
            {
                throw new Exception("Update columns can not be null!");
            }

            //设置更新列
            updateTable.SetColumns(updateColumns);

            try
            {
                var res = await updateTable.Where(where).ExecuteCommandAsync();
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("删除失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        /// <summary>
        /// 删除 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="isCheckAffectedRows"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IDeleteable<T> db, bool isCheckAffectedRows = true, string errorMessage = null)
            where T : class, new()
        {
            try
            {
                var res = await db.ExecuteCommandAsync();
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("删除失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        #endregion

        #region Ado
        /// <summary>
        /// Ado 直接返回通用消息类
        /// </summary>
        /// <param name="ado">SqlSugar ado</param>
        /// <param name="sql">sql</param>
        /// <param name="parameters">参数</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync(this IAdo ado, string sql, List<SugarParameter> parameters,
            bool isCheckAffectedRows = true, string errorMessage = null)
        {
            try
            {
                var res = await ado.ExecuteCommandAsync(sql, parameters);
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("更新失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        #endregion

        #region 事务
        /// <summary>
        /// 事务 直接返回通用消息类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorCallBack"></param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync(this SqlSugarScope db, Func<Task<ResponseModel>> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            ResponseModel result = ResponseModel.Error(errorMessage);
            try
            {
                db.BeginTran();

                if (func != null)
                {
                    result = await func();
                }
                if (result?.Code == ResponseCode.Success)
                {
                    db.CommitTran();
                }
                else
                {
                    db.RollbackTran();
                }
                return result;
            }
            catch (UserOperationException ex)
            {
                result.Message = !string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message;
                db.RollbackTran();
                errorCallBack?.Invoke(ex);
                return result;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    result.Message = errorMessage;
                }
                else
                {
                    if (EnvTool.IsProduction)
                    {
                        result.Message = "操作失败!";
                        NLogHelper.Error($"执行事务出错，{ex.Message}", ex);
                    }
                    else
                    {
                        result.Message = ex.Message;
                    }
                }

                db.RollbackTran();

                errorCallBack?.Invoke(ex);
                return result;
            }
        }
        /// <summary>
        /// 事务(无返回值) 直接返回通用消息类
        /// </summary>
        /// <param name="db">SqlSugarScope</param>
        /// <param name="func">委托方法</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCallBack">错误回调</param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync(this SqlSugarScope db, Func<Task> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            var result = await db.UseTranAsync(func, errorCallBack);
            if (result.IsSuccess)
                return ResponseModel.Success();
            else
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : result.ErrorMessage);
        }
        /// <summary>
        /// 事务(有返回值) 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugarScope</param>
        /// <param name="func">委托方法</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCallBack">错误回调</param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync<T>(this SqlSugarScope db, Func<Task<T>> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            var result = await db.UseTranAsync(func, errorCallBack);
            if (result.IsSuccess)
                return ResponseModel.Success(result.Data);
            else
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : result.ErrorMessage);
        } 
        #endregion

        #endregion

    }
}

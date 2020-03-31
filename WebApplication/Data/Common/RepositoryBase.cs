using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace WebApplication.Data.Common
{
    /// <summary>
    /// 基类数据仓储(Dapper.Helper)
    /// </summary>
    public class RepositoryBase : IRepositoryBase
    {
        private readonly string _connectionstr;

        protected RepositoryBase(string connectionstr)
        {
            _connectionstr = connectionstr;
        }

        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="pms">动态参数</param>
        /// <returns>返回实体</returns>
        public async Task<T> GetExecuteScalarAsync<T>(string sql, DynamicParameters pms = null)
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.ExecuteScalarAsync<T>(sql, pms);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="pms">动态参数</param>
        /// <returns>返回实体</returns>
        public async Task<T> GetEntityAsync<T>(string sql, DynamicParameters pms = null) where T : class, new()
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.QueryFirstOrDefaultAsync<T>(sql, pms);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="pms">动态参数</param>
        /// <returns>返回实体集合</returns>
        public async Task<IEnumerable<T>> GetAllEntityAsync<T>(string sql, DynamicParameters pms = null) where T : class, new()
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.QueryAsync<T>(sql, pms);
        }

        /// <summary>
        /// 根据多条语句返回多个结果集
        /// </summary>
        /// <typeparam name="T">返回实体</typeparam>
        /// <param name="sql">自定义拼接sql多个sql用;隔开</param>
        /// <param name="pms">动态参数</param>
        /// <param name="isnull">是否去除空实体(某一条语句没有查到数据)默认去掉</param>
        /// <returns></returns>
        public async Task<List<T>> GetMultipleEntityAsync<T>(string sql, DynamicParameters pms = null, bool isnull = false) where T : class, new()
        {
            var list = new List<T>();
            await using (SqlConnection conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr))
            {
                var multi = await conn.QueryMultipleAsync(sql, pms); // 执行查询，获取结果集集合

                while (!multi.IsConsumed)
                {
                    var data = await multi.ReadFirstOrDefaultAsync<T>();
                    if (isnull)
                    {
                        list.Add(data ?? new T());
                    }
                    else
                    {
                        if (data != null) list.Add(data);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取dynamic(动态)类型的集合
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="pms">动态参数</param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> GetDynamicAsync(string sql, DynamicParameters pms = null)
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.QueryAsync(sql, pms);
        }

        /// <summary>
        /// 增删改,实体方法
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="t">实体</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> ExecuteEntityAsync<T>(string sql, T t) where T : class, new()
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.ExecuteAsync(sql, t);
        }

        /// <summary>
        /// 增删改,参数方法
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="pms">动态参数</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> ExecuteEntityAsync(string sql, DynamicParameters pms = null)
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            return await conn.ExecuteAsync(sql, pms);
        }

        /// <summary>
        /// 批量增删改
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="t">实体集合</param>
        /// <returns>true成功,false失败</returns>
        public async Task<int> ExecuteMultipleEntityAsync<T>(string sql, IEnumerable<T> t) where T : class, new()
        {
            int i;
            await using (var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr))
            {
                await conn.OpenAsync();
                await using var trans = conn.BeginTransaction();
                try
                {
                    i = await conn.ExecuteAsync(sql, t, trans, 30, CommandType.Text);
                }
                catch (Exception)
                {
                    trans.Rollback();
                    trans.Dispose();
                    conn.Close();
                    throw;
                }
                trans.Commit();
            }

            return i;
        }

        /// <summary>
        /// 批量更新 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<int> ExecuteMultipleEntityAsync(string sql, object obj)
        {
            int i;
            await using (var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr))
            {
                await conn.OpenAsync();
                await using var trans = conn.BeginTransaction();
                try
                {
                    i = await conn.ExecuteAsync(sql, obj, trans, 30, CommandType.Text);
                }
                catch (Exception)
                {
                    trans.Rollback();
                    trans.Dispose();
                    conn.Close();
                    throw;
                }

                trans.Commit();
            }

            return i;
        }

        /// <summary>
        /// 新增实体返回对应主键Id
        /// </summary>
        /// <param name="sql">自定义拼接sql</param>
        /// <param name="t">实体</param>
        /// <returns>实体主键</returns>
        public async Task<int> GetAddEntityIdAsync<T>(string sql, T t) where T : class, new()
        {
            await using var conn = await DataBaseConfig.GetSqlConnectionAsync(_connectionstr);
            sql += ";SELECT SCOPE_IDENTITY()";
            return await conn.ExecuteScalarAsync<int>(sql, t);
        }

    }
}
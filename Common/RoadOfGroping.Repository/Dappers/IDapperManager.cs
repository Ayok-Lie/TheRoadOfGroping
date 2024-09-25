using System.Data;

namespace RoadOfGroping.Repository.Dappers
{
    public interface IDapperManager<T>
    {
        /// <summary>
        /// 获取指定表所有数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 获取指定表指定条件的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(string id);

        /// <summary>
        /// 指定表添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// 指定表更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// 指定表删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// 异步执行指定的 SQL 命令，并可选取特定的列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandTimeout">命令超时</param>
        /// <param name="transaction">事务</param>
        /// <param name="buffered">是否开启缓存</param>
        /// <param name="columns">是否选取特定的列，查询使用</param>
        /// <returns></returns>
        Task ExecuteAsync(string sql, object param = null, CommandType commandType = CommandType.Text, int? commandTimeout = null, IDbTransaction transaction = null, bool buffered = true, IEnumerable<string> columns = null);

        /// <summary>
        /// 执行查询SQL语句并返回结果
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandTimeout">命令超时</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// 执行查询SQL语句并返回单个结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<T> FindAsync(string sql, object param = null);

        /// <summary>
        /// 根据条件获取数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetByConditionAsync(string condition, object parameters = null);
    }
}
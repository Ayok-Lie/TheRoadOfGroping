using System.Data;
using Dapper;

namespace RoadOfGroping.Repository.Dappers
{
    public class DapperManager<T> : IDapperManager<T>
    {
        private readonly IDbConnection _dbConnection;

        // 构造函数，注入数据库连接
        public DapperManager(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // 异步获取所有记录
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string tableName = typeof(T).Name; // 获取表名
            return await _dbConnection.QueryAsync<T>($"SELECT * FROM [{tableName}]");
        }

        // 异步根据 ID 获取单个记录
        public async Task<T> GetByIdAsync(string id)
        {
            string tableName = typeof(T).Name; // 获取表名
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(
                $"SELECT * FROM [{tableName}] WHERE Id = @id", new { id });
        }

        // 异步根据条件获取记录
        public async Task<IEnumerable<T>> GetByConditionAsync(string condition, object parameters = null)
        {
            string tableName = typeof(T).Name; // 获取表名
            string sql = $"SELECT * FROM [{tableName}] WHERE {condition}";

            return await _dbConnection.QueryAsync<T>(sql, parameters); // 执行查询
        }

        // 异步添加新记录
        public async Task AddAsync(T entity)
        {
            string tableName = typeof(T).Name; // 获取表名
            var properties = typeof(T).GetProperties(); // 获取所有公共属性

            // 构建 SQL 语句的列名和参数名
            var columnNames = properties.Select(p => p.Name).ToList();
            var parameterNames = columnNames.Select(name => "@" + name).ToList();

            string sql = $"INSERT INTO [{tableName}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", parameterNames)})";

            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                parameters.Add("@" + property.Name, property.GetValue(entity)); // 添加参数及其值
            }

            await _dbConnection.ExecuteAsync(sql, parameters); // 执行 SQL 插入命令
        }

        // 异步更新记录
        public async Task UpdateAsync(T entity)
        {
            string tableName = typeof(T).Name; // 获取表名
            var properties = typeof(T).GetProperties(); // 获取所有公共属性

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();

            foreach (var property in properties)
            {
                // 排除不需要更新的字段
                if (!new[] { "Id", "DeleterUserName", "DeletionTime", "DeleterUserId", "IsDelete", "CreatorUserId", "CreatorUserName", "CreationTime" }.Contains(property.Name))
                {
                    setClauses.Add($"{property.Name} = @{property.Name}");
                    parameters.Add("@" + property.Name, property.GetValue(entity)); // 添加参数
                }
            }

            // 添加主键到参数
            var keyProperty = properties.FirstOrDefault(p => p.Name == "Id");
            if (keyProperty == null)
            {
                throw new Exception("Key property 'Id' not found in " + typeof(T).Name);
            }
            parameters.Add("@Id", keyProperty.GetValue(entity)); // 添加 Id 参数

            string sql = $"UPDATE [{tableName}] SET {string.Join(", ", setClauses)} WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, parameters); // 执行 SQL 更新命令
        }

        // 异步删除记录
        public async Task DeleteAsync(string id)
        {
            string tableName = typeof(T).Name; // 获取表名
            await _dbConnection.ExecuteAsync($"DELETE FROM [{tableName}] WHERE Id = @id", new { id }); // 执行 SQL 删除命令
        }

        // 执行任意 SQL 语句
        public async Task ExecuteAsync(string sql,
                                        object param = null,
                                        CommandType commandType = CommandType.Text,
                                        int? commandTimeout = null,
                                        IDbTransaction transaction = null,
                                        bool buffered = true,
                                        IEnumerable<string> columns = null)
        {
            // 处理列选择
            if (columns != null && columns.Any())
            {
                string columnString = string.Join(", ", columns);
                sql = $"SELECT {columnString} FROM ({sql}) AS SubQuery"; // 添加列过滤
            }

            // 执行 SQL 命令
            await _dbConnection.ExecuteAsync(new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: transaction,
                commandTimeout: commandTimeout,
                commandType: commandType,
                flags: buffered ? CommandFlags.Buffered : CommandFlags.None
            ));
        }

        // 异步查询并返回结果集
        public async Task<IEnumerable<T>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var result = await _dbConnection.QueryAsync<T>(
                sql: sql,
                param: param,
                commandTimeout: commandTimeout,
                commandType: commandType ?? CommandType.Text); // 默认为 Text

            return result; // 返回查询结果
        }

        // 异步查找记录（根据特定条件）
        public async Task<T> FindAsync(string sql, object param = null)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, param); // 查找一条记录
        }
    }
}
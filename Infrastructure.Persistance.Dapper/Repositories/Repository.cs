using Core.Contracts.Persistence;
using Dapper;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Template.Infrastructure.Persistance.Dapper.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        //private readonly string _connectionString;
        private readonly NpgsqlConnection _connection;
        private readonly string _tableName;

        public Repository(NpgsqlConnection connection)
        {
            _connection = connection;
            _tableName = ToSnakeCase(typeof(T).Name);
        }

        public virtual async Task<int> AddAsync(T entity)
        {
            try
            {
                var columns = string.Join(',', GetColumnNames());
                var values = string.Join(',', GetColumnValues().Select(c => $"@{c}"));
                return await _connection.ExecuteScalarAsync<int>(
                    $"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING id",
                    entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _connection.QueryFirstOrDefaultAsync<T>(
                    $"SELECT * FROM {_tableName} WHERE id = @id",
                    new { id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            try
            {
                var skip = (page - 1) * size;
                return (await _connection.QueryAsync<T>($"SELECT * FROM {_tableName} LIMIT @Size OFFSET @Skip", new { Skip = skip, Size = size })).ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsynch()
        {
            try
            {
                var result = await _connection.QueryAsync<T>(
                $"SELECT * FROM {_tableName}");
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                var updates = string.Join(',', GetColumnNames().Select((c, i) => $"{c} = @{GetColumnValues().ElementAt(i)}"));
                var sql = $"UPDATE {_tableName} SET {updates} WHERE id = @id";
                return await _connection.ExecuteAsync(sql, entity) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _connection.ExecuteAsync(
                $"DELETE FROM {_tableName} WHERE id = @id",
                new { id }) > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<string> GetColumnNames()
        {
            return typeof(T)
                .GetProperties().Where(p => p.Name != "Id" && !p.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute)))
                .Select(p => ToSnakeCase(p.Name));
        }

        private IEnumerable<string> GetColumnValues()
        {
            return typeof(T)
                .GetProperties().Where(p => p.Name != "Id" && !p.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute)))
                .Select(p => p.Name);
        }

        private static string ToSnakeCase(string input)
        {
            return string.Concat(input.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString())).ToLower();
        }
    }
}


//var GetAttributes = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any())
//        .Select(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).FirstOrDefault()).ToArray();

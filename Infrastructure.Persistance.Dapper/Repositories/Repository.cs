﻿using Core.Contracts.Persistence;
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
                var sql = ConvertSql($"SELECT * FROM {_tableName} WHERE id = @id");
                return await _connection.QueryFirstOrDefaultAsync<T>(
                    sql,
                    new { id });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<IReadOnlyList<T>> GetFilteredPagedAsync(int page, int size)
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

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
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

        public string ConvertSql(string sql)
        {
            // Get the properties of the class
            var properties = typeof(T).GetProperties().Where(p => !p.CustomAttributes.Any(a => a.AttributeType == typeof(NotMappedAttribute)));

            // Build the list of columns
            var columns = new List<string>();
            foreach (var property in properties)
            {
                // Get the column name for the property
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute != null ? columnAttribute.Name : ToSnakeCase(property.Name);

                // Add the column to the list, with an optional alias
                columns.Add(columnName == property.Name ? columnName : $"{columnName} as {property.Name}");
            }

            // Replace the SELECT * with the list of columns
            return sql.Replace("*", string.Join(", ", columns));
        }

        public async Task<IReadOnlyList<T>> GetFilteredAsync(QueryFilter filter)
        {
            // Build the SQL query
            var sql = ConvertSql($"SELECT * FROM {_tableName}");
            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();
            if (filter.Conditions != null)
            {
                // Add WHERE clauses for each filter condition
                var i = 0;
                foreach (var condition in filter.Conditions)
                {
                    whereClauses.Add($"{ToSnakeCase(condition.Column)} {condition.Operator} @p{i}");
                    parameters.Add($"p{i}", condition.Value);
                    i++;
                }
            }
            if (whereClauses.Any())
            {
                sql += $" WHERE {string.Join(" AND ", whereClauses)}";
            }
            if (filter.OrderByColumns != null)
            {
                // Add ORDER BY clause for each OrderByColumn
                var orderByClauses = filter.OrderByColumns.Select(x => $"{x.Column} {x.Direction}");
                sql += $" ORDER BY {string.Join(", ", orderByClauses)}";
            }
            if (filter.Limit.HasValue && filter.Offset.HasValue)
            {
                // Add LIMIT and OFFSET clauses for paging
                sql += $" LIMIT {filter.Limit} OFFSET {filter.Offset}";
            }

            // Execute the query and return the results
            var result = await _connection.QueryAsync<T>(sql, parameters);
            return result.ToList();
        }

    }
}


//var GetAttributes = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any())
//        .Select(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).FirstOrDefault()).ToArray();

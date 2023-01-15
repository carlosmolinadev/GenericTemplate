﻿using Core.Contracts.Persistence;
using Dapper;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Template.Infrastructure.Persistance.Dapper.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbConnection _connection;
        private readonly string _tableName;

        public Repository(DbConnection connection)
        {
            var tableAttr = typeof(T).GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                _tableName = tableAttr.Name;
            }
            else
            {
                _tableName = ToSnakeCase(typeof(T).Name);
            }

            _connection = connection;
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

        public virtual async Task<IList<T>> GetAllAsync()
        {
            try
            {
                var sql = ConvertSql($"SELECT * FROM {_tableName}");
                var result = await _connection.QueryAsync<T>(sql);
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<T>> GetFilteredAsync(QueryFilter filter)
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

        public virtual async Task<int> AddAsync(T entity)
        {
            try
            {
                var columns = string.Join(',', GetColumnNames());
                var values = string.Join(',', GetColumnValues().Select(c => $"@{c}"));
                var primaryKey = GetPrimaryKeyType().FirstOrDefault();

                if (primaryKey != null && primaryKey.Name != "Int32")
                {
                    return await _connection.ExecuteAsync($"INSERT INTO {_tableName} ({columns}) VALUES ({values})", entity);
                }
                else
                {
                    return await _connection.ExecuteScalarAsync<int>($"INSERT INTO {_tableName} ({columns}) VALUES ({values}) RETURNING id", entity);
                }
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

        private IEnumerable<Type> GetPrimaryKeyType()
        {
            return typeof(T)
                .GetProperties().Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
                .Select(p => p.PropertyType);
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
    }
}

//};


//public void BulkInsert<TEntity>(IEnumerable<TEntity> entities)
//{
//    using (var connection = new SqlConnection(_connectionString))
//    {
//        connection.Open();

//        var tableName = typeof(TEntity).Name;
//        var columns = GetColumnNames(typeof(TEntity));

//        var values = new StringBuilder();
//        foreach (var entity in entities)
//        {
//            values.Append("(");
//            values.Append(string.Join(", ", GetPropertyValues(entity)));
//            values.Append("), ");
//        }
//        values.Remove(values.Length - 2, 2);

//        var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES {values}";
//        connection.Execute(sql);
//    }
//}


//public void BulkUpdate<TEntity>(IEnumerable<TEntity> entities)
//{
//    using (var connection = new SqlConnection(_connectionString))
//    {
//        connection.Open();

//        var tableName = typeof(TEntity).Name;
//        var columns = GetColumnNames(typeof(TEntity));

//        var values = new StringBuilder();
//        foreach (var entity in entities)
//        {
//            values.Append("(");
//            values.Append(string.Join(", ", GetPropertyValues(entity)));
//            values.Append("), ");
//        }
//        values.Remove(values.Length - 2, 2);

//        var sql = $"UPDATE {tableName} SET ({string.Join(", ", columns)}) = ({values}) WHERE Id IN ({string.Join(", ", GetIdValues(entities))})";
//        connection.Execute(sql);
//    }
//}

//private IEnumerable<string> GetColumnNames(Type type)
//{
//    return type.GetProperties().Select(p => p.Name);
//}

//private IEnumerable<object> GetPropertyValues(object obj)
//{
//    return obj.GetType().GetProperties().Select(p => p.GetValue(obj));
//}

//private IEnumerable<object> GetIdValues(IEnumerable<TEntity> entities)
//{
//    return entities.Select(e => e.Id);
//}


//var GetAttributes = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).Any())
//        .Select(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).FirstOrDefault()).ToArray();

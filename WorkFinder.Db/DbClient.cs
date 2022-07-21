using Dapper;
using Dwh.Models;
using Dwh.Models.BulkCopyMappers;
using Dwh.Models.Queries;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Dwh
{
    public class DbClient
    {
        private string ConnectionString { get; }

        public DbClient(string userId, string password, string host, short port, string dbName, DbVendor dbType)
        {
            switch (dbType)
            {
                case DbVendor.Postgres:
                    ConnectionString = $"User ID={userId};Password={password};Host={host};Port={port};Database={dbName};Pooling=true;";
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                    break;

                default:
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                    break;
            }            
        }

        private IDbConnection OpenConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public IEnumerable<T> SelectAll<T>(string fullTableName)
        {
            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");

            var query = $"SELECT * FROM {fullTableName}";
            using (var connection = OpenConnection())
                return connection.Query<T>(query);
        }

        public IEnumerable<T> SelectAllByTargetEq<T>(string fullTableName, string targetColumnName, object targetColumnValue)
        {
            if (!Regex.IsMatch(targetColumnName.ToLower(), "[a-z0-9_]+"))
                throw new ArgumentException($"Wrong target column name - {targetColumnName}.");
            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");

            var query = $"SELECT * FROM {fullTableName} WHERE {targetColumnName} = @value";
            using (var connection = OpenConnection())
                return connection.Query<T>(query, new { value = targetColumnValue });
        }

        public IEnumerable<T> SelectAllByTargetIn<T>(string fullTableName, string targetColumnName, IEnumerable<string> targetColumnValues)
        {
            if (!Regex.IsMatch(targetColumnName.ToLower(), "[a-z0-9_]+"))
                throw new ArgumentException($"Wrong target column name - {targetColumnName}.");
            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");
            string valuesStr = string.Join(',', targetColumnValues.Select(v => $"'{v}'"));

            var query = $"SELECT * FROM {fullTableName} WHERE {targetColumnName} IN ( {valuesStr} )";
            using (var connection = OpenConnection())
                return connection.Query<T>(query);
        }

        public IEnumerable<T> SelectNotAll<T>(string fullTableName, IEnumerable<string> columns)
        {
            foreach (var column in columns)
                if (!Regex.IsMatch(column.ToLower(), "[a-z0-9_]+"))
                    throw new ArgumentException($"Wrong columns name - {column}.");

            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");

            string columnsStr = string.Join(',', columns);
            var query = $"SELECT {columnsStr} FROM {fullTableName}";

            using (var connection = OpenConnection())
                return connection.Query<T>(query);
        }

        public IEnumerable<T> SelectNotAllByTargetEq<T>(string fullTableName, IEnumerable<string> columns, 
            string targetColumnName, object targetColumnValue)
        {
            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");

            foreach (var column in columns)
                if (!Regex.IsMatch(column.ToLower(), "[a-z0-9_]+"))
                    throw new ArgumentException($"Wrong columns name - {column}.");

            if (!Regex.IsMatch(targetColumnName.ToLower(), "[a-z0-9_]+"))
                throw new ArgumentException($"Wrong target column name - {targetColumnName}.");


            string columnsStr = string.Join(',', columns);
            var query = $"SELECT {columnsStr} FROM {fullTableName} WHERE {targetColumnName} = @value";

            using (var connection = OpenConnection())
                return connection.Query<T>(query, new { value = targetColumnValue });
        }

        public IEnumerable<T> SelectNotAllByTargetIn<T>(string fullTableName, IEnumerable<string> columns, 
            string targetColumnName, IEnumerable<string> targetColumnValues)
        {
            if (!DbEntities.Tables.Contains(fullTableName.ToLower()))
                throw new ArgumentException("Wrong table name.");

            foreach (var column in columns)
                if (!Regex.IsMatch(column.ToLower(), "[a-z0-9_]+"))
                    throw new ArgumentException($"Wrong columns name - {column}.");

            if (!Regex.IsMatch(targetColumnName.ToLower(), "[a-z0-9_]+"))
                throw new ArgumentException($"Wrong target column name - {targetColumnName}.");

            string columnsStr = string.Join(',', columns);
            string valuesStr = string.Join(',', targetColumnValues.Select(v => $"'{v}'"));

            var query = $"SELECT {columnsStr} FROM {fullTableName} WHERE {targetColumnName} IN ( {valuesStr} )";
            using (var connection = OpenConnection())
                return connection.Query<T>(query);
        }

        public Guid Insert<T>(string schema, string table, T entity, bool isReturning = true) 
        {   
            var query = new InsertQueryBuilder<T>(schema, table, entity).Build(isReturning);
            using (var connection = OpenConnection())
                return connection.Query<Guid>(query, entity).Single();
        }

        public IEnumerable<Guid> Update<T>(string schema, string table, T entity, Guid id, bool isReturning = false)
        {
            var query = new UpdateQueryBuilder<T>(schema, table, entity, id).Build(isReturning);
            using (var connection = OpenConnection())
                return connection.Query<Guid>(query, entity);
        }

        public IEnumerable<Guid> Delete(string schema, string table, IEnumerable<Guid> ids, bool isReturning = false)
        {
            var query = new DeleteQueryBuilder(schema, table, ids).Build(isReturning);
            using (var connection = OpenConnection())
                return connection.Query<Guid>(query);
        }

        public DbClient(string connectionString)
        {
            ConnectionString = connectionString;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public DbClient(string userId, string password, string host, string port, string dbName, DbVendor dbType)
        {
            switch (dbType)
            {
                case DbVendor.Postgres:
                    ConnectionString = $"User ID={userId};Password={password};Host={host};Port={port};Database={dbName};Pooling=true;";
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                    break;

                default:
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                    break;
            }
        }

        public void ExecSp(string spName, params object[] values)
        {
            if (!GetAllStoredProcedures().Contains(spName.ToLower()))
                throw new ArgumentException("SP with that name does not exist.");

            using (var connection = OpenConnection())
                connection.Query(spName, values, commandType: CommandType.StoredProcedure);
        }

        public void BulkCopyUserCategory(string fullTableName, IEnumerable<UserCategory> entities)
        {
            var mapper = BulkCopyMappers.GetMapper(fullTableName);
            using (var connection = OpenConnection())
                mapper.SaveAll((NpgsqlConnection)connection, entities);
        }

        public enum DbVendor
        {
            Postgres
        }

        private IEnumerable<string> GetAllStoredProcedures()
        {
            var query = @"SELECT CONCAT(n.nspname, '.', p.proname) AS sp_name
                          FROM pg_proc p
                          JOIN pg_namespace n ON p.pronamespace = n.oid
                          WHERE n.nspname not in ('pg_catalog', 'information_schema')
	                          AND p.prokind = 'p'";
            using (var connection = OpenConnection())
                return connection.Query<string>(query);
        }

        private Dictionary<Type, DbType> TypeMap { get; } = new Dictionary<Type, DbType>
        {
            { typeof(string),   DbType.String },
            { typeof(Guid),     DbType.Guid },
            { typeof(bool),     DbType.Boolean },
            { typeof(int),      DbType.Int32 },
            { typeof(byte[]),   DbType.Binary },
            { typeof(DateTime), DbType.DateTime },
        };
    }

    public enum DbVendor
    {
        Postgres
    }
}

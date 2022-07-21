using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dapper;

namespace Dwh.Models.Queries
{
    public class SelectQueryBuilder<T> : QueryBuilder
    {
        private HashSet<string> Columns { get; set; }
        private Guid? SelectId { get; set; }

        public SelectQueryBuilder(string schema, string table)
        {
            Schema = schema;
            TableName = table;
            Columns = new HashSet<string>() { "*" };
        }

        public SelectQueryBuilder(string schema, string table, IEnumerable<string> columns)
        {
            Schema = schema;
            TableName = table;
            Columns = new HashSet<string>();
            foreach (var column in columns)
                Columns.Add(column);
        }

        public SelectQueryBuilder(string schema, string table, Guid id)
        {
            Schema = schema;
            TableName = table;
            Columns = new HashSet<string>() { "*" };
            SelectId = id;
        }

        public SelectQueryBuilder(string schema, string table, IEnumerable<string> columns, Guid id)
        {
            Schema = schema;
            TableName = table;
            Columns = new HashSet<string>();
            foreach (var column in columns)
                Columns.Add(column);
            SelectId = id;
        }

        public override string Build(bool isReturning = true)
        {
            Columns.RemoveWhere(item => !Regex.IsMatch(item, "([a-z0-9_]+|\\*)"));
            string columnsStr = string.Join(',', Columns);
            if (SelectId == null)
                return $"SELECT {string.Join(',', Columns)} FROM {Schema}.{TableName}";
            else
                return $"SELECT {string.Join(',', Columns)} FROM {Schema}.{TableName} WHERE id = '{SelectId.ToString()}'";
        }
    }
}

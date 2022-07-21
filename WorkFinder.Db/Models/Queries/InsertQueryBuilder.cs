using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dwh.Models.Queries
{
    class InsertQueryBuilder<T> : QueryBuilder
    {
        private T Entity { get; set; }
        private Dictionary<string, string> NewValues { get; set; }

        public InsertQueryBuilder(string schema, string table, T entity)
        {
            Schema = schema;
            TableName = table;
            Entity = entity;
            NewValues = new Dictionary<string, string>();
        }

        public override string Build(bool isReturning = true)
        {
            foreach (var prop in Entity.GetType().GetProperties())
                if (prop.GetValue(Entity) != null)
                    if (Regex.IsMatch(prop.Name, "[a-zA-Z0-9]+") && prop.Name.ToLower() != "id")
                        NewValues.Add(ToSnakeCase(prop.Name), "@" + prop.Name);
            return $"INSERT INTO {Schema}.{TableName} ({string.Join(',', NewValues.Keys)}) VALUES ({string.Join(',', NewValues.Values)}){(isReturning ? " RETURNING id" : "")}";
        }
    }
}

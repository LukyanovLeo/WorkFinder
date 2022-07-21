using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dapper;

namespace Dwh.Models.Queries
{
    class UpdateQueryBuilder<T> : QueryBuilder
    {
        private Guid Id { get; set; }

        private T Entity { get; set; }
        private Dictionary<string, string> NewValues { get; set; }

        public UpdateQueryBuilder(string schema, string table, T entity, Guid id)
        {
            Schema = schema;
            TableName = table;
            Id = id;
            Entity = entity;
            NewValues = new Dictionary<string, string>();
        }

        public override string Build(bool isReturning = true)
        {
            foreach (var prop in Entity.GetType().GetProperties())
                if (prop.GetValue(Entity) != null)
                    if (Regex.IsMatch(prop.Name, "[a-zA-Z0-9]+"))
                        NewValues.Add(ToSnakeCase(prop.Name), "@" + prop.GetValue(Entity));

            var newValuesList = new List<string>();
            foreach (var item in NewValues)
                newValuesList.Add($"{item.Key}={item.Key}");

            return $@"UPDATE {Schema}.{TableName} SET {string.Join(',', newValuesList)} WHERE id = ""{Id}""{(isReturning ? " RETURNING id" : "")}";
        }
    }
}

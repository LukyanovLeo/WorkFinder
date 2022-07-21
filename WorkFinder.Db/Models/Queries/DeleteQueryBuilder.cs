using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dwh.Models.Queries
{
    class DeleteQueryBuilder : QueryBuilder
    {
        private HashSet<Guid> Ids { get; set; }

        public DeleteQueryBuilder(string schema, string table, IEnumerable<Guid> ids)
        {
            Schema = schema;
            TableName = table;
            Ids = new HashSet<Guid>();
            foreach (var id in ids)
                Ids.Add(id);
        }

        public override string Build(bool isReturning = true)
        {
            return $"DELETE FROM {Schema}.{TableName} WHERE id IN ({string.Join(',', Ids.Select(id => $@"'{id}'"))}){(isReturning ? " RETURNING id" : "")}";
        }
    }
}

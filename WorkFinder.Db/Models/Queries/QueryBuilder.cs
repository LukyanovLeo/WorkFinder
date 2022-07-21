using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dwh.Models.Queries
{
    public abstract class QueryBuilder
    {
        private string _schema;
        private string _tableName;
        protected internal string Schema 
        { 
            get
            {
                return _schema;
            }
            set
            {
                if (!Regex.IsMatch(value, "[a-z]+"))
                    _schema = "";
                else
                    _schema = value;
            }
        }
        protected internal string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                if (!Regex.IsMatch(value, "[a-z0-9_]+"))
                    _tableName = "";
                else
                    _tableName = value;
            }
        }

        public abstract string Build(bool isReturning = true);

        public static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}

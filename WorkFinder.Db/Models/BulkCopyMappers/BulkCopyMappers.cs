using PostgreSQLCopyHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dwh.Models.BulkCopyMappers
{
    class BulkCopyMappers
    {
        private static Dictionary<string, PostgreSQLCopyHelper<UserCategory>> Mappers { get; } = new
            Dictionary<string, PostgreSQLCopyHelper<UserCategory>>
        {
            { 
                "quota.user_category",

                new PostgreSQLCopyHelper<UserCategory>("quota", "user_category")
                    .MapUUID("user_id", x => x.UserId)
                    .MapUUID("category_id", x => x.CategoryId)
                    .MapUUID("tariff_id", x => x.TariffId)
                    .MapTimeStampTz("expired_date", x => x.ExpiredDate)
            },
        };

        public static PostgreSQLCopyHelper<UserCategory> GetMapper(string fulltableTable) => Mappers[fulltableTable];
    }    
}

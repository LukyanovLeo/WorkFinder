using System;
using System.Collections.Generic;
using System.Text;

namespace Dwh.Models
{
    public static class DbEntities
    {
        public static HashSet<string> Tables { get; } = new HashSet<string>
        {
            "public.posts",
            "public.categories",
            "public.user",
            "public.tariffs",

            "social.vk_user",
            "social.google_user",
            "social.yandex_user",

            "quota.user_category",
        };
    }
}

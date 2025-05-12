using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoNETWeb.Migrations
{
    using System.Data.Entity.Migrations;

    public class Configuration : DbMigrationsConfiguration<DemoNETWeb.Data_Context.BlogContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.EntityFramework.MySqlMigrationSqlGenerator());
        }
    }
}
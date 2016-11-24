namespace Unitoys.Model.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Unitoys.Model;

    internal sealed class Configuration : DbMigrationsConfiguration<UnitoysEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(UnitoysEntities context)
        {
            base.Seed(context);

            //var category = new List<UT_ManageUsers> { 
            //  new UT_ManageUsers{LoginName="valen",PassWord="698d51a19d8a121ce581499d7b701668",TrueName="Ò¶²¨",Lock4=0,CreateDate=DateTime.Now}
            //};
            //category.ForEach(c => context.UT_ManageUsers.Add(c));
            //context.SaveChanges();
           

        }
    }
}

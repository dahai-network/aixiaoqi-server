using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Core;
using Unitoys.IServices;
using Unitoys.Model;
using Unitoys.Services;

namespace ConsoleApplication1
{
    
    class Program
    {

        static void Main(string[] args)
        {
            UnitoysEntities db = new UnitoysEntities();

            UT_ManageUsers manageUser = new UT_ManageUsers();
            manageUser.LoginName = "vale2n22";
            manageUser.PassWord = SecureHelper.MD5("33123");
            manageUser.TrueName = "李四1";
            manageUser.Lock4 = 0;
            manageUser.CreateDate = DateTime.Now;

            UT_Country m = new UT_Country();
            m.CountryName = "美国";
            m.CreateDate = DateTime.Now;
            m.Pic = "pic";
            m.Rate = 1;


            db.UT_ManageUsers.Add(manageUser);
            db.UT_Country.Add(m);
            var rs = db.SaveChanges();
            
            Console.WriteLine("结果：" + rs);
            Console.ReadLine();

        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class MySqlDBHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private static string connectionStr = 
            System.Configuration.ConfigurationManager.ConnectionStrings["VosBbConnection"].ConnectionString;
        public MySqlDBHelper() { }
        public MySqlDBHelper(string connectionStr)
        {
            MySqlDBHelper.connectionStr = connectionStr;
        }

        /// <summary>
        /// 得到连接对象
        /// </summary>
        /// <returns></returns>
        private static MySqlConnection GetConn()
        {
            MySqlConnection sqlconn = null;
            sqlconn = new MySqlConnection(connectionStr);
            //if (string.IsNullOrEmpty(connectionStr))
            //{
            //    sqlconn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["web"].ConnectionString);
            //}
            //else
            //{
            //    sqlconn = new MySqlConnection(connectionStr);
            //}
            return sqlconn;
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, params MySqlParameter[] sp)
        {
            //return new DataTable();
            using (MySqlConnection conn = GetConn())
            {
                conn.Open();
                using (MySqlDataAdapter sda = new MySqlDataAdapter(sql, conn))
                {
                    sda.SelectCommand.Parameters.AddRange(sp);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 增删改操作
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>执行后的条数</returns>
        public static int ExecuteNonQuery(string sql, params MySqlParameter[] sp)
        {

            using (MySqlConnection conn = GetConn())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(sp);
                    int i = cmd.ExecuteNonQuery();
                    return i;
                }
            }

        }

        /// <summary>
        /// 执行一条SQL语句,返回首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>首行首列</returns>
        public static object ExecuteScalar(string sql, params MySqlParameter[] sp)
        {
            using (MySqlConnection conn = GetConn())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(sp);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}

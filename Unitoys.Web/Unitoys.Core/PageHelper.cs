using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public class PageHelper
    {
        #region - 属性 -

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 连接语句
        /// </summary>
        public string InnerJoin { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public string SelectFields { get; set; }

        /// <summary>
        /// 排序字段名
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页号
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 排序类别
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public string WhereCondition { get; set; }

        #endregion

        #region - 构造函数 -

        /// <summary>
        /// 构造函数(需要对where参数进行安全性过滤,最好配合可变参数执行)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="orderName">排序字段名称</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageNumer">页号</param>
        /// <param name="orderType">排序方式</param>
        /// <param name="where">查询条件</param>
        public PageHelper(string tableName, string orderName, int pageSize, int pageNumer, string orderType, string where)
            : this(tableName, "", "*", orderName, pageSize, pageNumer, orderType, where)
        {


        }

        /// <summary>
        /// 构造函数(需要对where参数进行安全性过滤,最好配合可变参数执行)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="innerJoin">连接语句</param>
        /// <param name="selectFields">查询字段,默认为*</param>
        /// <param name="orderName">排序字段名称</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageNumer">页号</param>
        /// <param name="orderType">排序方式</param>
        /// <param name="where">查询条件</param>
        public PageHelper(string tableName, string innerJoin, string selectFields, string orderName, int pageSize, int pageNumber, string orderType, string where)
        {
            this.TableName = tableName
                ;
            this.InnerJoin = innerJoin;
            this.SelectFields = selectFields; ;
            this.OrderName = orderName;
            this.PageSize = pageSize;
            this.PageNumber = pageNumber <= 0 ? 1 : pageNumber;//如果PageNumber<0则赋值为1
            this.OrderType = orderType.ToLower() == "desc" ? "desc" : "asc";
            this.WhereCondition = where;
        }

        #endregion

        #region - 方法 -

        /// <summary>
        /// 获得记录总数的sql语句
        /// </summary>
        /// <returns></returns>
        public string GetCount()
        {
            string CommandText = "select count(*) as Total from [ " + TableName + " ] " + InnerJoin;
            if (!String.IsNullOrEmpty(WhereCondition))
            {
                CommandText += " where " + WhereCondition;
            }
            return CommandText;
        }

        #endregion


        #region 获取Select Top配合 Max,Min方式的查询分页sql语句(千万级别分页)
        /// <summary>
        /// 获取Select Top配合 Max,Min方式的查询分页sql语句(千万级别分页)
        /// </summary>
        /// <returns></returns>
        public string GetSelectTopByMaxOrMinPagination()
        {

            //主语句
            StringBuilder sbSql = new StringBuilder(300);
            //临时变量
            string strTemp = "";
            //排序类型
            string strOrder = "";
            if (OrderType == "desc")
            {
                strTemp = " <( select min ";
            }
            else
                strTemp = " > ( select max ";
            //排序类型生成
            strOrder = " order by [" + OrderName + "]  " + OrderType + " ";
            //如果页码为1就做优化查询用Select Top的方式
            if (PageNumber == 1)
            {
                //查询条件判断
                if (!String.IsNullOrEmpty(WhereCondition))
                    sbSql.AppendFormat("select top {0} {1} from [{2}] {3} where {4} {5}", PageSize, SelectFields, TableName, InnerJoin, WhereCondition, strOrder);
                else
                    sbSql.AppendFormat("select top {0} {1} from [{2}] {3}  {4}", PageSize, SelectFields, TableName, InnerJoin, strOrder);
            }
            else
            {

                if (!String.IsNullOrEmpty(WhereCondition))
                {
                    sbSql.AppendFormat("select top {0} {1} from {2} {3} where [{4}] {5} ([{4}]) from ( select top {6} [{4}] from [{2}]  {3} {7} ) as tblTmp)  and {8} {7} ", PageSize, SelectFields, TableName, InnerJoin, OrderName, strTemp, (PageNumber - 1) * PageSize, strOrder, WhereCondition);
                }
                else
                    sbSql.AppendFormat("select top {0} {1} from {2} {3} where [{4}] {5} ([{4}]) from (select top {6} [{4}] from [{2}]  {3} {7} ) as tblTmp) {7}", PageSize, SelectFields, TableName, InnerJoin, OrderName, strTemp, (PageNumber - 1) * PageSize, strOrder);
            }
            return sbSql.ToString();
        }
        #endregion
        #region 通过RowNumber的方式分页(百万级别左右)
        /// <summary>
        /// 通过RowNumber的方式分页(百万级别左右)
        /// </summary>
        /// <returns></returns>
        public string GetRowNumberPagination(int totalCount)
        {
            //开始记录和结束记录,总页数,总记录数
            int startRecord = 0, endRecord = 0, totalPage = 0;

            //计算开始页码
            startRecord = (PageNumber - 1) * PageSize + 1;

            //计算结束页码
            endRecord = startRecord + PageSize - 1;
            //获取总记录数的sql和查询的sql
            string totalCountSql, sqlString;
            totalCountSql = "select @TotalRecord = count(*) from " + TableName + " ";//总记录数sql语句
            sqlString = string.Format("(select row_number() over (order by {0} {1}) as rowId,{2} from {3} ", OrderName, OrderType, SelectFields, TableName);
            //添加查询条件
            if (!String.IsNullOrEmpty(WhereCondition))
            {
                totalCountSql += " where " + WhereCondition;
                sqlString += " where " + WhereCondition;
            }
            //计算总页数
            totalPage = (int)Math.Ceiling(totalCount * 1.0 / (double)PageSize);
            sqlString = string.Format("  select * from {0}) as t where rowId between {1}  and {2} ", sqlString, startRecord, endRecord);

            return sqlString;
        }
        #endregion
    }
}

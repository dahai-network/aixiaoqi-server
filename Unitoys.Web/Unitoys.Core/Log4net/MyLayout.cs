using log4net.Appender;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unitoys.Model;

namespace Unitoys.Core.Log4net
{
    public class MyMessagePatternConverter : PatternLayoutConverter
    {

        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }

        }

        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }

    }
    class MyLayout : log4net.Layout.PatternLayout
    {
        public MyLayout()
        {
            this.AddConverter("property", typeof(MyMessagePatternConverter));
        }
    }
    public class CustAdoNetAppender : AdoNetAppender
    {
        string _connectiongStr = null;
        public new string ConnectionString
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    using (UnitoysEntities db = new UnitoysEntities())
                    {
                        value = db.Database.Connection.ConnectionString;
                    }
                }
                _connectiongStr = value;
                base.ConnectionString = _connectiongStr;
            }
            get
            {
                return base.ConnectionString;
            }
        }
    }
}

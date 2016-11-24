using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    public static class EnumHelper
    {
        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string GetDescription(this Enum value, bool isTop = false)
        {
            Type enumType = value.GetType();
            DescriptionAttribute attr = null;
            if (isTop)
            {
                attr = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
            }
            else
            {
                // 获取枚举常数名称。
                string name = Enum.GetName(enumType, value);
                if (name != null)
                {
                    // 获取枚举字段。
                    FieldInfo fieldInfo = enumType.GetField(name);
                    if (fieldInfo != null)
                    {
                        // 获取描述的属性。
                        attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    }
                }
            }

            if (attr != null && !string.IsNullOrEmpty(attr.Description))
                return attr.Description;
            else
                return string.Empty;

        }
    }
}

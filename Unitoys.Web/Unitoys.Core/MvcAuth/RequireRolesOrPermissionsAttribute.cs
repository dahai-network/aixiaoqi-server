using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitoys.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRolesOrPermissionsAttribute : Attribute
    {
        private string[] _rolesOrPermissionsName;

        public RequireRolesOrPermissionsAttribute(params string[] rolesOrPermissionsName)
        {
            this._rolesOrPermissionsName = rolesOrPermissionsName;
        }

        public string[] RolesOrPermissionsName
        {
            get
            {
                return this._rolesOrPermissionsName;
            }
        }
    }
}

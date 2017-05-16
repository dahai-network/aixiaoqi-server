using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitoys.IServices;
using Unitoys.Core;
using Unitoys.Model;
using System.Data.Entity;

namespace Unitoys.Services
{
    public class ProductAttributeService : BaseService<UT_ProductAttribute>, IProductAttributeService
    {
        public async Task<KeyValuePair<int, List<UT_ProductAttribute>>> SearchAsync(int page, int rows, string attributeName, string attributeValue)
        {
            using (UnitoysEntities db = new UnitoysEntities())
            {
                var query = db.UT_ProductAttribute.Include(x => x.UT_Attribute).Include(x => x.UT_AttributeValue).Where(x => true);
                if (!string.IsNullOrEmpty(attributeName))
                {
                    query = query.Where(x => x.UT_Attribute.Name.Contains(attributeName));
                }
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    query = query.Where(x => x.UT_AttributeValue.Value.Contains(attributeValue));
                }

                var result = await query.OrderByDescending(x => x.CreateDate).Skip((page - 1) * rows).Take(rows).ToListAsync();

                var count = await query.CountAsync();

                return new KeyValuePair<int, List<UT_ProductAttribute>>(count, result);
            }
        }
    }
}

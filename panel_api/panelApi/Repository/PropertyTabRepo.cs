using panelApi.Models;
using panelApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace panelApi.Repository
{
    public class PropertyTabRepo : IPropertyTabRepo
    {
        public bool Create(PropertyTab entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(PropertyTab entity)
        {
            throw new NotImplementedException();
        }

        public PropertyTab Get(Expression<Func<PropertyTab, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public ICollection<PropertyTab> GetList(Expression<Func<PropertyTab, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public bool Update(PropertyTab entity)
        {
            throw new NotImplementedException();
        }
    }
}

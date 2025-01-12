﻿using panelApi.Models;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface ISubCategoryRepo : IBaseRepo<SubCategory>
    {
        Task<int> GetCategoryId(Expression<Func<SubCategory, bool>> filter = null);
    }
}

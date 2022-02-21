﻿using panelApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace panelApi.Repository.IRepository
{
    public interface IFe_SubCat_RelRepo : IBaseRepo<Fe_SubCat_Relational>
    {
        Task<bool> UpdateCreate(List<Fe_SubCat_Relational> entity);
        Task<bool> IsPairExist(int featureId, int subCategoryId);
    }
}
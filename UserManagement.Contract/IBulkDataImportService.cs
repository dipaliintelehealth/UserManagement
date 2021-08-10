// ***********************************************************************
// Assembly         : DataUnification.Repository.Contract
// Author           : Manoj Kumar Behera
// Created          : 31-JUL-2019
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="IRepository.cs" company="CSM technologies">
//     Copyright ©  2019
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UserManagement.Domain;

namespace UserManagement.Contract
{
    public interface IBulkDataImportService<T> where T :class
    {
        Task<IEnumerable<ResultModel<T>>> ImportData(Stream stream);
    }
}

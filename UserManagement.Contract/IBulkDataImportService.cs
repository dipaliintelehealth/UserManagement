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
using CSharpFunctionalExtensions;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Contract
{
    public interface IBulkDataImportService<T> where T :class
    {
        Task<Result<IEnumerable<T>>> ImportData(IEnumerable<MemberBulkImportVM> models,string pathForCsvLog);
        Task<IEnumerable<T>> CreateModels(Stream stream);
        Task<IEnumerable<KeyValue<string,string>>> GetStates();
        Task<IEnumerable<KeyValue<string, string>>> GetDistrict(string stateId);

        Task<IEnumerable<KeyValue<string, string>>> GetCities(string stateId,string districtId);


    }
}

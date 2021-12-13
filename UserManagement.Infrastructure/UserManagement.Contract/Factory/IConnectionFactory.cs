// ***********************************************************************
// Assembly         : DataUnification.Repository.Contract
// Author           : Manoj Kumar Behera
// Created          : 31-JUL-2019
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="IConnectionFactory.cs" company="CSM technologies">
//     Copyright ©  2019
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Data;

namespace UserManagement.Contract.Factory
{
    /// <summary>
    /// Interface IConnectionFactory
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Gets the get connection.
        /// </summary>
        /// <value>The get connection.</value>
        IDbConnection GetConnection { get; }
    }
    ///// <summary>
    ///// Interface IOraConnectionFactory
    ///// </summary>
    //public interface IOraConnectionFactory
    //{
    //    /// <summary>
    //    /// Gets the get connection.
    //    /// </summary>
    //    /// <value>The get connection.</value>
    //    IDbConnection GetConnection { get; }
    //}
}

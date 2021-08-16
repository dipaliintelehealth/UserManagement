// ***********************************************************************
// Assembly         : DataUnification.Repository
// Author           : Manoj Kumar Behera
// Created          : 29-Jul-2019
//
// Last Modified By : Manoj Kumar Behera
// Last Modified On : 29-SEP-2018
// ***********************************************************************
// <copyright file="RepositoryBase.cs" company=" ">
//     Copyright ©  2019
// </copyright>
// <summary></summary>
// ***********************************************************************

using Dapper;
using UserManagement.Contract;
using UserManagement.Contract.Factory;
using System;
using System.Data;

namespace UserManagement.Infrastructure.Repository
{
    /// <summary>
    /// Class RepositoryBase.
    /// </summary>    
    /// <seealso cref="DataUnification.Repository.Contract.IRepository" />
    public abstract class RepositoryBase : IRepository
    {        
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        protected IDbConnection Connection;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        protected RepositoryBase(IConnectionFactory connectionFactory)
        {
           
                Connection = connectionFactory.GetConnection;
                //Not required to open the connection, it will automatically managed by DAPPER
                //Connection.Open();
           
        }        

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {  
                    Connection?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        ~RepositoryBase()
        {
            Dispose(false);
        }
    }

    
}

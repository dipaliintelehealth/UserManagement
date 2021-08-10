// ***********************************************************************
// Assembly         : DataUnification.Repository
// Author           : Manoj Kumar Behera
// Created          : 29-SEP-2018
//
// Last Modified By : Manoj Kumar Behera
// Last Modified On : 29-SEP-2018
// ***********************************************************************
// <copyright file="ConnectionFactory.cs" company=" ">
//     Copyright ©  2018
// </copyright>
// <summary></summary>
// ***********************************************************************
 
using System.Data;
using System.Data.SqlClient;
using UserManagement.Contract.Factory;

namespace UserManagement.Infrastructure.Factory
{
    /// <summary>
    /// Class ConnectionFactory.
    /// </summary>
    /// <seealso cref="UserManagement.Contract.Factory.IConnectionFactory" />
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets the get connection.
        /// </summary>
        /// <value>The get connection.</value>
        //public IDbConnection GetOraConnection => new OracleConnection(_connectionString);
        public IDbConnection GetConnection => new SqlConnection(_connectionString);
    }

}

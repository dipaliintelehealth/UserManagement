using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace UserManagement.Infrastructure.Extension
{
    public class SqlDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<SqlParameter> sqlParameters = new List<SqlParameter>();

        public void Add(string name, SqlDbType sqlDbType, ParameterDirection direction, object value = null, int? size = null)
        {
            var sqlParameter = new SqlParameter();
            if (size.HasValue)
            {
                sqlParameter.ParameterName = name;
                sqlParameter.SqlDbType = sqlDbType;
                sqlParameter.Direction = direction;
                sqlParameter.Value = value;
                sqlParameter.Size = size.Value;
            }
            else
            {
                sqlParameter.ParameterName = name;
                sqlParameter.SqlDbType = sqlDbType;
                sqlParameter.Direction = direction;
                sqlParameter.Value = value;
            }

            sqlParameters.Add(sqlParameter);
        }

        public void Add(string name, SqlDbType sqlDbType, ParameterDirection direction)
        {
            var sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = name;
            sqlParameter.SqlDbType = sqlDbType;
            sqlParameter.Direction = direction;
            sqlParameters.Add(sqlParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

            var sqlCommand = command as SqlCommand;

            if (sqlCommand != null)
            {
                sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
            }
        }
        public object GetOutParamValue(string param)
        {
            object res = 0;
            SqlCommand cmd = new SqlCommand();
            res = Convert.ToString(cmd.Parameters[param].Value);
            return res;
        }
    }
}
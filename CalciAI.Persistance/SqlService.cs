using System.Data.SqlClient;
using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using CalciAI.Models;
using System.Linq;

namespace CalciAI.Persistance
{

    public static class SqlService
    {
        //private static readonly string connString = FileConfigProvider.Load<SqlSetting>("sql").ConnectionString;
        private static readonly int defaultCommandTimeout = 300;
        private static readonly int MsgVariableLength = 8000;

        #region For Insert, Update And Delete

        public static async Task<ProcessResult> ExecuteSPAsync(string SPName, object paramModel)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();
                var result = await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (result > 0)
                {
                    return ProcessResult.Success();
                }

                return ProcessResult.Fail("DBError", "DB Error.");
            }
            catch (Exception ex)
            {
                return ProcessResult.Fail("Exception", ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<ProcessResult> ExecuteSPAsync(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                var result = await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (result > 0)
                {
                    return ProcessResult.Success();
                }

                return ProcessResult.Fail("DBError", "DB Error.");
            }
            catch (Exception ex)
            {
                return ProcessResult.Fail("DBError", ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<string> ExecuteSpWithSingleReturnAsync(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);
            
            try
            {
                string defaultOutParam = "@Msg";

                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);

                    SqlParameter outpara = null;
                    outpara = cmd.Parameters.Add(defaultOutParam, SqlDbType.NVarChar, MsgVariableLength);
                    outpara.Direction = ParameterDirection.Output;
                }

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                return cmd.Parameters[defaultOutParam].Value.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<string> ExecuteSpWithSingleReturnAsync(string SPName, SqlParameter[] parameters, string outParamName)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);

                    SqlParameter outpara = null;
                    outpara = cmd.Parameters.Add(outParamName, SqlDbType.NVarChar, MsgVariableLength);
                    outpara.Direction = ParameterDirection.Output;
                }

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                return cmd.Parameters[outParamName].Value.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<ProcessResult<SpResponse>> ExecuteSpWithMultiReturnAsync(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                var result = await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();

                SpResponse spResponse = new();

                foreach (var prm in parameters)
                {
                    if (prm.Direction == ParameterDirection.Output)
                    {
                        spResponse.ReturnValue.Add(prm.ParameterName, cmd.Parameters[prm.ParameterName].Value.ToString());
                    }
                }

                return ProcessResult<SpResponse>.Success(spResponse);
            }
            catch (Exception ex)
            {
                return ProcessResult<SpResponse>.Fail("DBError", ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        #endregion


        #region For Fetch One Row With Model Parameters

        public static async Task<T> ExecuteReadersAsync<T>(string SPName, SqlParameter[] parameters = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataTable dt = new();
                da.Fill(dt);
                await con.CloseAsync();

                if (dt.Rows.Count > 0)
                {
                    var retVal = ConvertToModel<T>(dt);
                    dt.Dispose();
                    return retVal;
                }
                else
                {
                    dt.Dispose();
                    return Activator.CreateInstance<T>();
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2>> ExecuteReadersAsync<T1, T2>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);

                    ds.Dispose();
                    return new Tuple<T1, T2>(tableT1, tableT2);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2, T3>> ExecuteReadersAsync<T1, T2, T3>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToModel<T3>(ds.Tables[2]);

                    ds.Dispose();
                    return new Tuple<T1, T2, T3>(tableT1, tableT2, tableT3);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2, T3>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>(), Activator.CreateInstance<T3>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2, T3, T4>> ExecuteReadersAsync<T1, T2, T3, T4>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToModel<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToModel<T4>(ds.Tables[2]);

                    ds.Dispose();
                    return new Tuple<T1, T2, T3, T4>(tableT1, tableT2, tableT3, tableT4);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2, T3, T4>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>(), Activator.CreateInstance<T3>(), Activator.CreateInstance<T4>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        #endregion


        #region For Fetch One Row With Sql Parameters

        public static T ExecuteReader<T>(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                con.Open();
                SqlDataAdapter da = new(cmd);
                DataTable dt = new();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                {
                    var retVal = ConvertToModel<T>(dt);
                    dt.Dispose();
                    return retVal;
                }
                else
                {
                    dt.Dispose();
                    return Activator.CreateInstance<T>();
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    //con.Dispose();
                }
            }
        }

        public static async Task<T> ExecuteReaderAsync<T>(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataTable dt = new();
                da.Fill(dt);
                await con.CloseAsync();

                if (dt.Rows.Count > 0)
                {
                    var retVal = ConvertToModel<T>(dt);
                    dt.Dispose();
                    return retVal;
                }
                else
                {
                    dt.Dispose();
                    return Activator.CreateInstance<T>();
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2>> ExecuteReaderAsync<T1, T2>(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);

                    ds.Dispose();
                    return new Tuple<T1, T2>(tableT1, tableT2);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2, T3>> ExecuteReaderAsync<T1, T2, T3>(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToModel<T3>(ds.Tables[2]);

                    ds.Dispose();
                    return new Tuple<T1, T2, T3>(tableT1, tableT2, tableT3);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2, T3>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>(), Activator.CreateInstance<T3>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<T1, T2, T3, T4>> ExecuteReaderAsync<T1, T2, T3, T4>(string SPName, SqlParameter[] parameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                await con.OpenAsync();
                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToModel<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToModel<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToModel<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToModel<T4>(ds.Tables[3]);

                    ds.Dispose();
                    return new Tuple<T1, T2, T3, T4>(tableT1, tableT2, tableT3, tableT4);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<T1, T2, T3, T4>(Activator.CreateInstance<T1>(), Activator.CreateInstance<T2>(), Activator.CreateInstance<T3>(), Activator.CreateInstance<T4>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        #endregion


        #region For Fetch Multiple Row With Model Parameter

        public static async Task<List<T>> ExecuteReaderListAsync<T>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataTable dt = new();
                da.Fill(dt);
                await con.CloseAsync();

                if (dt.Rows.Count > 0)
                {
                    var retVal = ConvertToList<T>(dt);
                    dt.Dispose();
                    return retVal;
                }
                else
                {
                    dt.Dispose();
                    return new List<T>(new List<T>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>>> ExecuteReaderListAsync<T1, T2>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>>(tableT1, tableT2);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>>(new List<T1>(), new List<T2>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>>> ExecuteReaderListAsync<T1, T2, T3>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>>(tableT1, tableT2, tableT3);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>>(new List<T1>(), new List<T2>(), new List<T3>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> ExecuteReaderListAsync<T1, T2, T3, T4>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(tableT1, tableT2, tableT3, tableT4);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>> ExecuteReaderListAsync<T1, T2, T3, T4, T5>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                con.Open();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                con.Close();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);
                    var tableT5 = ConvertToList<T5>(ds.Tables[4]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>(tableT1, tableT2, tableT3, tableT4, tableT5);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    //con.Dispose();
                }
            }
        }

        public static Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>> ExecuteReaderListAsync<T1, T2, T3, T4, T5, T6>(string SPName, object paramModel = null)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (paramModel != null)
                {
                    Type type = paramModel.GetType();
                    PropertyInfo[] props = type.GetProperties();

                    foreach (var prop in props)
                    {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(paramModel));
                    }
                }

                con.Open();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                con.Close();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);
                    var tableT5 = ConvertToList<T5>(ds.Tables[4]);
                    var tableT6 = ConvertToList<T6>(ds.Tables[5]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>(tableT1, tableT2, tableT3, tableT4, tableT5, tableT6);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>(), new List<T6>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    //con.Dispose();
                }
            }
        }

        #endregion


        #region For Fetch Multiple Row With Sql Parameter

        public static async Task<List<T>> ExecuteReaderListAsync<T>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataTable dt = new();
                da.Fill(dt);
                await con.CloseAsync();

                var retVal = ConvertToList<T>(dt);
                dt.Dispose();

                return retVal;
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>>> ExecuteReaderListAsync<T1, T2>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>>(tableT1, tableT2);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>>(new List<T1>(), new List<T2>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>>> ExecuteReaderListAsync<T1, T2, T3>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>>(tableT1, tableT2, tableT3);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>>(new List<T1>(), new List<T2>(), new List<T3>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>>> ExecuteReaderListAsync<T1, T2, T3, T4>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(tableT1, tableT2, tableT3, tableT4);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>> ExecuteReaderListAsync<T1, T2, T3, T4, T5>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);
                    var tableT5 = ConvertToList<T5>(ds.Tables[4]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>(tableT1, tableT2, tableT3, tableT4, tableT5);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }
        
        public static async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>> ExecuteReaderListAsync<T1, T2, T3, T4, T5, T6>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);
                    var tableT5 = ConvertToList<T5>(ds.Tables[4]);
                    var tableT6 = ConvertToList<T6>(ds.Tables[5]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>(tableT1, tableT2, tableT3, tableT4, tableT5, tableT6);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>(), new List<T6>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        public static async Task<Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>> ExecuteReaderListAsync<T1, T2, T3, T4, T5, T6, T7>(string SPName, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(SPName, con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataSet ds = new();
                da.Fill(ds);
                await con.CloseAsync();

                if (ds.Tables.Count > 0)
                {
                    var tableT1 = ConvertToList<T1>(ds.Tables[0]);
                    var tableT2 = ConvertToList<T2>(ds.Tables[1]);
                    var tableT3 = ConvertToList<T3>(ds.Tables[2]);
                    var tableT4 = ConvertToList<T4>(ds.Tables[3]);
                    var tableT5 = ConvertToList<T5>(ds.Tables[4]);
                    var tableT6 = ConvertToList<T6>(ds.Tables[5]);
                    var tableT7 = ConvertToList<T7>(ds.Tables[6]);

                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>(tableT1, tableT2, tableT3, tableT4, tableT5, tableT6, tableT7);
                }
                else
                {
                    ds.Dispose();
                    return new Tuple<List<T1>, List<T2>, List<T3>, List<T4>, List<T5>, List<T6>, List<T7>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>(), new List<T6>(), new List<T7>());
                }
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }

        #endregion


        #region Scalar Function
        public static async Task<string> ExecuteScalarFunctionAsync(string query, SqlParameter[] sqlParameters)
        {
            using SqlConnection con = new(ConnStringStore.SqlConnectionString);

            try
            {
                using SqlCommand cmd = new(query, con);

                cmd.CommandTimeout = defaultCommandTimeout;

                if (sqlParameters != null)
                {
                    cmd.Parameters.AddRange(sqlParameters);
                }

                await con.OpenAsync();

                SqlDataAdapter da = new(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                await con.CloseAsync();

                string str = dt.Rows[0][0].ToString();
                return str;
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    await con.CloseAsync();
                    //con.Dispose();
                }
            }
        }
        #endregion Scalar Function


        #region Converter Methods

        public static List<T> ConvertToList<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name))
                        {
                            if (row[pro.Name].GetType() == typeof(DBNull))
                            {
                                pro.SetValue(objT, null, null);
                            }
                            else if (pro.PropertyType.IsEnum)
                            {
                                pro.SetValue(objT, Enum.Parse(pro.PropertyType.GetTypeInfo().UnderlyingSystemType, row[pro.Name].ToString()), null);
                            }
                            else
                            {
                                pro.SetValue(objT, row[pro.Name], null);
                            }
                        }
                    }

                    return objT;
                }).ToList();
            }
            catch(Exception ex)
            {
                return new List<T>();
            }
        }

        public static T ConvertToModel<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();

                var properties = typeof(T).GetProperties();

                var objT = Activator.CreateInstance<T>();

                if (dt.Rows.Count == 0)
                {
                    return objT;
                }

                DataRow row = dt.Rows[0];

                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        if (row[pro.Name].GetType() == typeof(DBNull))
                        {
                            pro.SetValue(objT, null, null);
                        }
                        else if (pro.PropertyType.IsEnum)
                        {
                            pro.SetValue(objT, Enum.Parse(pro.PropertyType.GetTypeInfo().UnderlyingSystemType, row[pro.Name].ToString()), null);
                        }
                        else
                        {
                            pro.SetValue(objT, row[pro.Name], null);
                        }
                    }
                }

                return objT;
            }
            catch
            {
                Exception exception = new();
                throw exception;
            }
        }

        #endregion
    }
}

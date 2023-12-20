using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace SQLiteConnector
{
    class DAL_SQLite : IDisposable
    {
        string _dbFilePath = null;
        SQLiteConnection _connection = null;
        SQLiteDataReader _dataReader = null;

        string _error = null; string _errorSql=null;

        public DAL_SQLite() { }

        public void OpenDatabase(string dbFilePath) { _dbFilePath = dbFilePath; _connection = new SQLiteConnection(GetConnectionString(_dbFilePath)); _connection.Open(); }
        public void CloseDatabase() { if (_connection == null) return; _connection.Close(); _connection.Dispose(); }

        static string GetConnectionString(string dbFilePath) { return string.Format("Data Source={0};Version=3", dbFilePath); }


        public string GetError() { return string.Format("Error: {0}\r\nSQL:{1}", _error, _errorSql); }

        public Exception GetErrorException(string className) { return GetErrorException(className, new System.Diagnostics.StackFrame(1).GetMethod().Name); }

        public Exception GetErrorException(string className, string functionName) { return new Exception(string.Format("{0}/ {1}\r\n{2}",className,functionName ,GetError())); }
        public Exception GetErrorException(System.Reflection.MethodBase method) { return GetErrorException(method.DeclaringType.Name,method.Name); }

        SQLiteTransaction _transaction = null;
        public void TransActionStart() { _transaction = _connection.BeginTransaction(); }

        public void TransActionCommit() { _transaction.Commit(); }
        public void TransActionRollback() { _transaction.Rollback(); }

 
        public int ExecuteCommand(string sql)
        {
            int result = -1;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(_connection))
                {
                    command.CommandText = sql;
                    result=command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { _error = ex.Message; _errorSql = sql;result = -1; }

            return result;
        }
        public int? ExecuteCommandValue(string sql)
        {
            int? result = null;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(_connection))
                {
                    command.CommandText = sql;
                    result=command.ExecuteScalar() as int?;
                    if (result == null)
                    {
                        Int64? result2 = command.ExecuteScalar() as Int64?;
                        if (result2.HasValue)
                            result = (int)result2.Value;
                    }

                }
            }
            catch (Exception ex) { _error = ex.Message; _errorSql = sql; }
            return result;
        }

        public List<string> ExecuteCommandValues(string sql)
        {
            if (!Rdr_Open(sql))
                return null;

            List<string> result = new List<string>();

            try
            {
                while (Rdr_DataRead())
                    result.Add(Rdr_DataFieldStr(0));

                Rdr_Close();
            }

            catch { result = null; Rdr_Close(); }

            return result;
        }
        public List<int> ExecuteCommandValuesInt(string sql)
        {
            if (!Rdr_Open(sql))
                return null;

            List<int> result = new List<int>();

            try
            {
                while (Rdr_DataRead())
                    result.Add(Rdr_DataFieldInt(0));

                Rdr_Close();
            }

            catch { result = null; Rdr_Close(); }

            return result;
        }

        public int GetLastRowID() { int? result= ExecuteCommandValue("SELECT last_insert_rowid()"); return result ?? -1; }

        public void Dispose() { } // CloseDatabase(); }

        #region SQLConvert
        public static string SQLBool(bool value) { return value ? "1" : "0"; }

        public static string SQLDate(DateTime? dDate, bool withTime)
        {
            if (!dDate.HasValue)
                return "Null";

            string formatDate = "yyyy\\-MM\\-dd";
            string formatTime = "yyyy\\-MM\\-dd\\THH\\:mm\\:ss";

            return SQLText(dDate.Value.ToString(withTime ? formatTime : formatDate));
        }

        public static string SQLInt(int value) { return value.ToString(); }
        public static string SQLDouble(double value,string format) { return value.ToString(format); }
        public static string SQLText(string Text)
        {
            if (String.IsNullOrEmpty(Text))
            {
                return SQLNull();
            }
            else
            {
                if (Text.Contains("'"))
                {
                    string Result = null;
                    for (int i = 0; i < Text.Length; i++)
                    {
                        if (Text.Substring(i, 1) == "'")
                        {
                            Result = Result + "''";
                        }
                        else
                        {
                            Result = Result + Text.Substring(i, 1);
                        }
                    }
                    return "'" + Result + "'";
                }
                else
                {
                    return "'" + Text + "'";
                }
            }
        }
        public static string SQLNull() { return "null"; }

        #endregion 

        #region Data Reader



        #region Data Read
        public bool Rdr_Open(string sql)
        {
            bool result = false;
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(_connection))
                {
                    command.CommandText = sql;
                    _dataReader = command.ExecuteReader();
                    result = true;
                }
            }
            catch (Exception ex) { _error = ex.Message; _errorSql = sql; result = false; }

            return result;
        }
        public void Rdr_Close()
        {
            if (_dataReader == null)
                return;
            _dataReader.Close();
        }

        public bool Rdr_DataRead() { return _dataReader.Read(); }
        //public bool Rdr_NextResult() { return _dataReader.NextResult(); }
        //public object Rdr { get { return _dataReader; } }
        #endregion

        #region Fields
        public int Rdr_FieldCount() { return _dataReader.FieldCount; }
        //public string Rdr_GetName(int col) { return _dataReader.GetName(col); }
        public string Rdr_DataFieldName(int col) { return _dataReader.GetName(col); }
        //public Type Rdr_DataFieldType(int col) { return _dataReader.GetFieldType(col); }
        public bool Rdr_DataFieldIsNull(int col) { return _dataReader.IsDBNull(col); }

        public string Rdr_DataFieldStr(int col) { return Rdr_DataFieldIsNull(col)?"":_dataReader.GetString(col); }
        public int Rdr_DataFieldLenght(int col) { return -1; }
        public int Rdr_DataFieldInt(int col) { return Rdr_DataFieldInt(col,0); }
        public int Rdr_DataFieldInt(int col, int iNullValue) { return Rdr_DataFieldIsNull(col) ? iNullValue : (int)_dataReader.GetInt32(col); }
        public int? Rdr_DataFieldIntNull(int col)
        {
            try
            {
                if (Rdr_DataFieldIsNull(col)) // || dataReader.(col) == "")
                    return (int?)null;
                else
                    return (int)_dataReader.GetInt32(col);
            }
            catch
            {
                bool ist = _dataReader.IsDBNull(col);
                string ft = _dataReader.GetDataTypeName(col);
                return null;
            }
        }
        public double? Rdr_DataFieldDoubleNull(int col)
        {
            try
            {
                if (Rdr_DataFieldIsNull(col)) // || dataReader.(col) == "")
                    return null;
                else
                    return _dataReader.GetDouble(col);
            }
            catch
            {
                bool ist = _dataReader.IsDBNull(col);
                string ft = _dataReader.GetDataTypeName(col);
                return null;
            }
        }
        public double Rdr_DataFieldDouble(int col, double rNullValue) { return Rdr_DataFieldIsNull(col) ? rNullValue : _dataReader.GetDouble(col); }
        //public decimal Rdr_DataFieldDecimal(int col, decimal nullValue) { return Rdr_DataFieldIsNull(col) ? nullValue : _dataReader.GetDecimal(col); }
        public byte Rdr_DataFieldByte(int col, byte nullValue) { return Rdr_DataFieldIsNull(col) ? nullValue : _dataReader.GetByte(col); }
        public bool Rdr_DataFieldBool(int col, bool nullValue) { return Rdr_DataFieldIsNull(col) ? nullValue : _dataReader.GetInt32(col)>0; }
        public Guid Rdr_DataFieldGuid(int col, Guid NullValue) { return Rdr_DataFieldIsNull(col) ? NullValue : _dataReader.GetGuid(col); }
        public DateTime? Rdr_DataFieldDateTime(int col)
        {
            if (Rdr_DataFieldIsNull(col)) return null;

            var value = Rdr_DataFieldStr(col);

            //temporary fix for time records containing '.' as separator
            if(DateTime.TryParse(value.Replace('.',':'),out DateTime result))
                return result;

            throw new FormatException($"Unable to read date or time '{value}'");
        }
        #endregion

        #endregion
    }
}

using System;
using System.Data;

namespace NBWare.CMS.Framework.SqlHelper
{
    public class DataValue : IDisposable
    {
        private object _data;
        private ParameterDirection _direction;
        private int _length;
        private string _name;
        private DbType _paramType;

        public void Dispose()
        {
            if (this._data is IDisposable)
            {
                ((IDisposable)this._data).Dispose();
            }
            this._data = null;
        }

        public DataValue(string name, object data, ParameterDirection direction, DbType dbtype)
            : this(name, data, direction, (data == null) ? 500 : data.ToString().Length, dbtype)
        {

        }

        public DataValue(string name, object data, ParameterDirection direction, int length, DbType type)
        {
            this._name = name;
            this._direction = direction;
            this._data = data;
            this._length = length;
            this._paramType = type;
        }
        public string name
        {
            get
            {
                return this._name;
            }
        }

        public DbType ParamType
        {
            get
            {
                return this._paramType;
            }
        }

        public int Length
        {
            get
            {
                return this._length;
            }
        }

        public ParameterDirection direction
        {
            get
            {
                return this._direction;
            }
        }

        public object data
        {
            get
            {
                return this._data;
            }
        }
    }
}

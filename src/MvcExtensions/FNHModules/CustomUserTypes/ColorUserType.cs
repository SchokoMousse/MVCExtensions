﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using System.Drawing;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using FluentNHibernate.Conventions;

namespace MvcExtensions.FNHModules.CustomUserTypes
{
    // source : http://stackoverflow.com/questions/1063933/nhibernate-mapping-to-system-drawing-color
    public class ColorUserType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? typeof(Color).GetHashCode() + 473 : x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var obj = NHibernateUtil.String.NullSafeGet(rs, names[0]);
            if (obj == null) return null;
            var colorString = (string)obj;
            return ColorTranslator.FromHtml(colorString);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                ((IDataParameter)cmd.Parameters[index]).Value = ColorTranslator.ToHtml((Color)value);
            }

        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public SqlType[] SqlTypes
        {
            get { return new[] { new SqlType(DbType.StringFixedLength) }; }
        }

        public Type ReturnedType
        {
            get { return typeof(Color); }
        }

        public bool IsMutable
        {
            get { return true; }
        }
    }

}

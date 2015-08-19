﻿using System;
using UnityEngine;

//反射机制应用到的库
using System.Reflection;

//SQLite应用到的库
using System.Data;
using Mono.Data.Sqlite;

namespace AssemblyCSharp
{

	[AttributeUsage(AttributeTargets.Property)]
	public class SQLFieldAttribute:Attribute
	{
		public string Name{ set; get;}
		public string Type{ set; get;}
		public bool IsNotNull{ set; get;}
		public bool AutoIncrement{set;get;}
		public bool IsPrimaryKey{set;get;}
		public string Default{ set; get;}
		public bool IsUnique{set;get;}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class SQLTableAttribute:Attribute
	{
		public string Name{set;get;}
	}


	public interface SQLInterface{

		int sql_id{ set;get; }

		string GetValues();
		string GetFields();

		string InsertSQL{ get;}
//		string UpdateSQL{ get;}
//		string DeleteSQL{ get;}
	}


	/// <summary>
	/// 测试功能用到的类
	/// </summary>
	public class SQLObject:SQLInterface
	{
		[SQLField(Name="sql_id",Type="integer",AutoIncrement=true,IsPrimaryKey=true)]
		public int sql_id{ set; get;}

		public SQLObject(){}

		public string GetValues()
		{
			PropertyInfo[] p_list = this.GetType().GetProperties ();

			string values = "(";

			foreach (PropertyInfo item in p_list) 
			{
				string name = LOSQLiteTools.GetFieldName (item);
				if (name == null) {
					continue;
				}

				values += "'" + item.GetValue (this, null).ToString () + "',";
			}

			values = values.Substring (0, values.Length - 1);

			values += ")";

			return values;
		}

		public string GetFields()
		{
			PropertyInfo[] p_list = this.GetType().GetProperties ();

			string fields = "(";

			foreach (PropertyInfo item in p_list) 
			{	
				string name = LOSQLiteTools.GetFieldName (item);
				if (name == null) {
					continue;
				}
				fields += name + ",";
			}


			fields = fields.Substring (0, fields.Length - 1);

			fields += ")";

			return fields;
		}


		public string InsertSQL{
			get{ 
				string sql = "insert into ";
				sql += LOSQLiteTools.GetTableName(this.GetType());
				sql += this.GetFields() + " ";
				sql += "values";
				sql += this.GetValues () + ";";
				return sql;
			}
		}
	}

	[SQLTable(Name="test_class")]
	public class TestClass:SQLObject
	{
		[SQLField(Name="test_name",Type="text")]
		public string 	test_name{set;get;}

		[SQLField(Name="test_age",Type="integer")]
		public int		test_age{ set; get;}
	}
	public class LOSQLiteTools
	{
		//静态数据库链接对象
		private static SqliteConnection connection;

		/// <summary>
		/// 打开数据库
		/// </summary>
		public static void OpenDB(string name)
		{
			//构造数据库链接字符串
			string path = "Data Source=";
			path += Application.persistentDataPath + "/";
			path += name + ".sqlite";

			Debug.Log (path);

			//创建数据库链接对象
			connection = new SqliteConnection (path);
			//打开数据库
			connection.Open ();
		}

		/// <summary>
		/// 获取表格的名称
		/// </summary>
		/// <returns>The table name.</returns>
		public static string GetTableName(Type item)
		{
			//获取到特性类型
			Type att_type = typeof(SQLTableAttribute);

			//获取参数type对应的特性对象
			Attribute a = Attribute.GetCustomAttribute(item,att_type);

			if (a == null) {
				return null;
			}

			//因为在Attribute.Get函数里的最后一个参数已经指定了
			//特性的类型是SQLTableAttribute,所以可以显式转换
			SQLTableAttribute sa = (SQLTableAttribute)a;

			//将特性对象的Name属性返回
			return sa.Name;
		}

		/// <summary>
		/// 获取属性在Field中的名字
		/// </summary>
		public static string GetFieldName(PropertyInfo item)
		{
			Type att_type = typeof(SQLFieldAttribute);

			Attribute a = Attribute.GetCustomAttribute (item, att_type);

			if (a == null) {
				return null;
			}

			SQLFieldAttribute sfa = (SQLFieldAttribute)a;

			return sfa.Name;
		}

		/// <summary>
		/// 获取属性在Field中的类型
		/// </summary>
		private static string GetFieldType(PropertyInfo item)
		{
			Type att_type = typeof(SQLFieldAttribute);

			Attribute a = Attribute.GetCustomAttribute (item, att_type);

			if (a == null) {
				return null;
			}

			SQLFieldAttribute sfa = (SQLFieldAttribute)a;

			return sfa.Type;
		}

		/// <summary>
		/// 获取创建表格时的Field字符串
		/// </summary>
		private static string GetFieldString(PropertyInfo item)
		{
			Type att_type = typeof(SQLFieldAttribute);
			Attribute a = Attribute.GetCustomAttribute (item, att_type);

			if (a == null) {
				return null;
			}

			SQLFieldAttribute sfa = (SQLFieldAttribute)a;

			string sql = "";
			sql += sfa.Name + " ";
			sql += sfa.Type + " ";

			if (sfa.IsPrimaryKey) {
				sql += "primary key" + " ";
			}
			if (sfa.AutoIncrement) {
				sql += "autoincrement" + " ";
			}
			if (sfa.IsNotNull) {
				sql += "not null" + " ";
			}
			if (sfa.IsUnique) {
				sql += "unique" + " ";
			}
			if (sfa.Default != null) {
				sql += "default " + sfa.Default;
			}

			return sql;
		}

		/// <summary>
		/// 通过实体类型创建数据库表格
		/// </summary>
		public static void CreateTable(Type type)
		{
			//获取一个类型的所有属性
			PropertyInfo[] p_list = type.GetProperties();

			//获取Table的名字
			string table_name = GetTableName(type);

			//获取Table的列名字符串
			string field_list = "(";

			foreach (PropertyInfo item in p_list) 
			{
				string fieldstring = GetFieldString (item);
				if (fieldstring == null) {
					continue;
				}
				//对应的属性区域
				field_list += GetFieldString(item) + ",";
			}

			//删除最后一个,
			field_list = field_list.Substring (0, field_list.Length - 1);

			field_list += ")";

			//开始构造sql命令
			string sql = "create table if not exists ";
			sql += table_name + field_list + ";";

			SqliteCommand command = new SqliteCommand (connection);

			command.CommandText = sql;

			command.ExecuteNonQuery ();
		}

		/// <summary>
		/// 判断一个类型是否包含Table特性
		/// </summary>
		private static bool HasAttribute(Type item)
		{
			Type att_type = typeof(SQLTableAttribute);

			Attribute a = Attribute.GetCustomAttribute (item, att_type);

			return a != null;
		}

		/// <summary>
		/// 添加具有SQLTable特性的对象数据
		/// </summary>
		public static bool InsertEntity(SQLObject sender)
		{
			if (!HasAttribute(sender.GetType())) 
			{
				return false;
			}
				
			SqliteCommand command = new SqliteCommand (connection);
			command.CommandText = sender.InsertSQL;
			command.ExecuteNonQuery ();

			return true;
		}
	}
}


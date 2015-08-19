# SQLite数据库-Unity操作

**项目开发的时候，经常会遇到的一种需求，数据存储**

离线缓存的数据类型很多，大致分成两类
- 字符串文本数据
- 多媒体数据

字符串数据的类型只有字符串，但是结构有很多:
-  xml
- json
- md5
- base64
-  普通字符串

多媒体数据的类型:
- 图片(jpg,png,gif...)
- 音频(mp3,aif...)
- 视频(mp4,mpv)


*通常用数据库来存储字符串文本类型的数据，但是需要注意的是数据库同时也能存储多媒体类型的数据*

***
**关系数据库**
在一个给定的应用领域中，所有实体及实体之间联系的集合构成一个关系数据库。
目前主流的关系数据库有oracle、db2、sqlserver、sybase、mysql等。

**在Unity中打开数据库函数**
```
	private string GetDBPath(string name)
	{
		return Application.persistentDataPath + "/" + name + ".sqlite";
	}

	/// <summary>
	/// 就是用来存储程序与数据库链接的对象
	/// </summary>
	private SqliteConnection connection = null;
	private void OpenDataBase()
	{
		//获取一个数据库文件的路径
		string path = GetDBPath ("xiaohao");
		string c = "Data Source=" + path;
		//需要通过数据库文件磁盘路径进行初始化
		connection = new SqliteConnection (c);
		//打开数据库
		connection.Open();
	}
```

## CRUD

***
**创建表格SQL**
```
create table lo_human(human_id integer,human_name text,human_age integer);
```
**C#函数调用创建表格SQL**
```
	private void CreateObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand(connection);

		//给命令对象添加SQL语句
		command.CommandText = "create table if not exists lo_human(human_id integer,human_name text,human_age integer);";

		//执行命令
		command.ExecuteNonQuery();
	}
```
***

**添加数据**
```
insert into lo_human(human_id,human_name,human_age) values(1,'xiaohao',36);
```
**C#函数调用添加数据SQL**
```
	private void InsertObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);

		//给命令对象添加SQL语句
		command.CommandText = "insert into lo_human(human_id,human_name,human_age) values(1,'xiaohao',36);";

		//执行命令
		command.ExecuteNonQuery ();
	}
```
***

**更新数据**
```
update lo_human set human_name='cuiyayun' where human_id=2;
```
**C#函数调用更新数据SQL**
```
	private void UpdateObject()
	{
    	//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);
        
        //给命令对象添加SQL语句
		command.CommandText = "update lo_human set human_name='cuiyayun' where human_id=3;";
        
        //执行命令
		command.ExecuteNonQuery ();
	}
```
***
**删除数据**
```
delete from lo_human where humanid=1;
```
**C#函数调用删除数据SQL**
```
	private void DeleteObject()
	{
    	//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);
		
        //给命令对象添加SQL语句
        command.CommandText = "delete from lo_human where human_id=1;";
		
        //执行命令
        command.ExecuteNonQuery ();
	}
```
***
**查询数据**
```
select * from lo_human where human_id>15 order by human_id desc;
```
**C#函数调用查询数据SQL**
```
	private void SelectObject()
	{
    	//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);
		
        //给命令对象添加SQL语句
        command.CommandText = "select * from lo_human where human_id>15 order by human_id desc;";

		//数据读取器
		SqliteDataReader reader = command.ExecuteReader();

		//判读是否可以读取下一行数据,如果可以的话就获取数据
		while (reader.Read())
		{
			//在循环体里,已经确定是哪行数据.
			Debug.Log(reader ["human_name"]);
		}
	}
```
*** 

## 高级用法
**通过使用C#语言的反射机制实现工具类SQLiteTools**
```
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
    
    /// <summary>
	/// 测试功能用到的类
	/// </summary>
    //创建TestClass附件的特性对象SQLTable,并且将该特性对象的属性Name赋值为"test_class"
	[SQLTable(Name="test_class")]
	public class TestClass
	{

		//创建test_id属性附件的特性对象SQLField,并且将该特性对象的属性Name、Type、AutoIncrement、IsNotNull、IsPrimaryKey进行赋值
        [SQLField(Name="test_id",Type="integer",AutoIncrement=true,IsNotNull=true,IsPrimaryKey=true)]
		public int 		test_id{set;get;}

		[SQLField(Name="test_name",Type="text")]
		public string 	test_name{set;get;}

		[SQLField(Name="test_age",Type="integer")]
		public int		test_age{ set; get;}

		public TestClass(){}
	}
```
**LOSQLiteTools.cs实现具体的功能**
**获取表格名称函数**
```
		/// <summary>
		/// 获取表格的名称
		/// </summary>
		/// <returns>The table name.</returns>
		private static string GetTableName(Type item)
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
```
**获取属性姓名函数**
```
		/// <summary>
		/// 获取属性在Field中的名字
		/// </summary>
		private static string GetFieldName(PropertyInfo item)
		{
			Type att_type = typeof(SQLFieldAttribute);

			Attribute a = Attribute.GetCustomAttribute (item, att_type);

			if (a == null) {
				return null;
			}

			SQLFieldAttribute sfa = (SQLFieldAttribute)a;

			return sfa.Name;
		}
```
**获取属性类型函数**
```
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
```
**获取属性区域字符串函数**
```
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
```
**创建表格函数**
```
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
				//对应的属性区域
				field_list += GetFieldString(item) + ",";
			}

			//删除最后一个,
			field_list = field_list.Substring (0, field_list.Length - 1);

			field_list += ")";

			//开始构造sql命令
			string sql = "create table if not exists ";
			sql += table_name + field_list + ";";

			Debug.Log (sql);

			SqliteCommand command = new SqliteCommand (connection);

			command.CommandText = sql;

			command.ExecuteNonQuery ();
		}
	}
```
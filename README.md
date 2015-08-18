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
using UnityEngine;
using System.Collections;

using Mono.Data.Sqlite;
using System.Data;
using AssemblyCSharp;

public class RX_SqliteScript : MonoBehaviour {


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
		string path = GetDBPath ("good");
		string c = "Data Source=" + path;
		//需要通过数据库文件磁盘路径进行初始化
		connection = new SqliteConnection (c);

		//打开数据库
		connection.Open();
	}

	/// <summary>
	/// 创建表格.
	/// </summary>
	private void CreateObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand(connection);

		//给命令对象添加SQL语句
		command.CommandText = "create table if not exists lo_human(human_id integer,human_name text,human_age integer);";

		//执行命令
		command.ExecuteNonQuery();
	}

	/// <summary>
	/// 插入数据.
	/// </summary>
	private void InsertObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);

		//给命令对象添加SQL语句
		command.CommandText = "insert into lo_human(human_id,human_name,human_age) values(1,'xiaohao',36);";

		//执行命令
		command.ExecuteNonQuery ();
	}

	/// <summary>
	/// 更新数据.
	/// </summary>
	private void UpdateObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);

		//给命令对象添加SQL语句
		command.CommandText = "update lo_human set human_name='cuiyayun' where human_id=3;";

		//执行命令
		command.ExecuteNonQuery ();
	}

	/// <summary>
	/// 删除数据.
	/// </summary>
	private void DeleteObject()
	{
		//在一个数据库链接对象上创建一个命令对象
		SqliteCommand command = new SqliteCommand (connection);

		//给命令对象添加SQL语句
		command.CommandText = "delete from lo_human where human_id=1;";

		//执行命令
		command.ExecuteNonQuery ();
	}

	/// <summary>
	/// 查询数据.
	/// </summary>
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

	// Use this for initialization
	void Start ()
	{
		//打开数据库
		LOSQLiteTools.OpenDB("good");
		//调用函数..
		LOSQLiteTools.CreateTable (typeof(TestClass));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

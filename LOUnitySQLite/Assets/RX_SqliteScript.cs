using UnityEngine;
using System.Collections;

using Mono.Data.Sqlite;
using System.Data;
using AssemblyCSharp;

public class RX_SqliteScript : MonoBehaviour {


	[SQLTable(Name="student")]
	public class Student:SQLObject
	{
		[SQLField(Name="stu_name",Type="text")]
		public string stu_name{ set; get;}
	}
		
	// Use this for initialization
	void Start ()
	{
		//打开数据库
		LOSQLiteTools.OpenDB("yihuiyun");
		//调用函数..
		LOSQLiteTools.CreateTable (typeof(TestClass));
		LOSQLiteTools.CreateTable (typeof(Student));

		Student stu = new Student ();
		stu.sql_id = 0;
		stu.stu_name = "易荟云";

		LOSQLiteTools.UpdateEntity (stu);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

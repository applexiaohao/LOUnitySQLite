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

		public Student(){}

		public override string ToString ()
		{
			return string.Format ("[Student: stu_name={0}]", stu_name);
		}
	}
		
	// Use this for initialization
	void Start ()
	{
		//打开数据库
		LOSQLiteTools.OpenDB("yihuiyun");


		SQLObject[] list = LOSQLiteTools.SelectEntity (typeof(Student));

		foreach (SQLObject item in list) 
		{
			Debug.Log (item.ToString());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

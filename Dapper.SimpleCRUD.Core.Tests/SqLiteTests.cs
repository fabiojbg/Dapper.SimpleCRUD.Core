using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.IO;
using SimpleCRUD.Core.Tests.Models;

namespace SimpleCRUD.Core.Tests
{
    /*
    [TestClass]
    [Ignore]
    public class SqLiteTests : SimpleCRUDCoreBaseTests
    {
        protected TestContext _textContext;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            try
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\MyDatabase.sqlite");
            }
            catch { }
            //SqliteConnection.CreateFile("MyDatabase.sqlite");
            //_connection = new SqliteConnection($"Data Source={Directory.GetCurrentDirectory()}\\MyDatabase.sqlite;");
            _connection = new SqliteConnection($"Data Source={Directory.GetCurrentDirectory()}\\MyDatabase.sqlite;");

            using (_connection)
            {
                _connection.Open();

                _connection.Execute(@" create table Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime default current_timestamp ) ");
                _connection.Execute(@" create table Car (CarId INTEGER PRIMARY KEY AUTOINCREMENT, Id INTEGER null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                _connection.Execute(@" create table BigCar (CarId INTEGER PRIMARY KEY AUTOINCREMENT, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                _connection.Execute(@" insert into BigCar (CarId,Make,Model) Values (2147483649,'car','car') ");
                _connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                _connection.Execute(@" CREATE TABLE GUIDTest([Id] [uniqueidentifier] NOT NULL,[name] [varchar](50) NOT NULL, CONSTRAINT [PK_GUIDTest] PRIMARY KEY  ([Id] ASC))");
                _connection.Execute(@" create table StrangeColumnNames (ItemId INTEGER PRIMARY KEY AUTOINCREMENT, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null) ");
                _connection.Execute(@" create table UserWithoutAutoIdentity (Id INTEGER PRIMARY KEY, Name nvarchar(100) not null, Age int not null) ");
                _connection.Execute(@" create table IgnoreColumns (Id INTEGER PRIMARY KEY AUTOINCREMENT, IgnoreInsert nvarchar(100) null, IgnoreUpdate nvarchar(100) null, IgnoreSelect nvarchar(100)  null, IgnoreAll nvarchar(100) null) ");
                _connection.Execute(@" CREATE TABLE KeyMaster (Key1 INTEGER NOT NULL, Key2 INTEGER NOT NULL, PRIMARY KEY ([Key1], [Key2]))");
            }
           Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.SQLite);
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _connection.Close();
            File.Delete(Directory.GetCurrentDirectory() + "\\MyDatabase.sqlite");
        }
    }
    */
}

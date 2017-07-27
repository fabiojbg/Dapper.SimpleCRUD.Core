using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;

namespace SimpleCRUD.Core.Tests
{
    [TestClass]
    public class SqlServerTests : SimpleCRUDCoreBaseTests
    {
        protected TestContext _textContext;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            using (var masterConnection = new SqlConnection(@"Data Source=(LocalDb)\mssqllocaldb;Initial Catalog=Master;Integrated Security=True"))
            {
                masterConnection.Open();
                try
                {
                    masterConnection.Execute(@" DROP DATABASE DapperSimpleCrudTestDb; ");
                }
                catch (Exception)
                { }

                masterConnection.Execute(@" CREATE DATABASE DapperSimpleCrudTestDb; ");
            }

            _connection = new SqlConnection(@"Data Source = (LocalDb)\mssqllocaldb;Initial Catalog=DapperSimpleCrudTestDb;Integrated Security=True;MultipleActiveResultSets=True");

            _connection.Open();
            _connection.Execute(@" create table Users (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime DEFAULT(getdate())) ");
            _connection.Execute(@" create table Car (CarId int IDENTITY(1,1) not null, Id int null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
            _connection.Execute(@" create table BigCar (CarId bigint IDENTITY(2147483650,1) not null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
            _connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
            _connection.Execute(@" CREATE SCHEMA Log; ");
            _connection.Execute(@" create table Log.CarLog (Id int IDENTITY(1,1) not null, LogNotes nvarchar(100) NOT NULL) ");
            _connection.Execute(@" CREATE TABLE [dbo].[GUIDTest]([Id] [uniqueidentifier] NOT NULL,[name] [varchar](50) NOT NULL, CONSTRAINT [PK_GUIDTest] PRIMARY KEY CLUSTERED ([Id] ASC))");
            _connection.Execute(@" create table StrangeColumnNames (ItemId int IDENTITY(1,1) not null Primary Key, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null)");
            _connection.Execute(@" create table UserWithoutAutoIdentity (Id int not null Primary Key, Name nvarchar(100) not null, Age int not null) ");
            _connection.Execute(@" create table IgnoreColumns (Id int IDENTITY(1,1) not null Primary Key, IgnoreInsert nvarchar(100) null, IgnoreUpdate nvarchar(100) null, IgnoreSelect nvarchar(100)  null, IgnoreAll nvarchar(100) null) ");
            _connection.Execute(@" CREATE TABLE GradingScale ([ScaleID] [int] IDENTITY(1,1) NOT NULL, [AppID] [int] NULL, [ScaleName] [nvarchar](50) NOT NULL, [IsDefault] [bit] NOT NULL)");
            _connection.Execute(@" CREATE TABLE KeyMaster ([Key1] [int] NOT NULL, [Key2] [int] NOT NULL, CONSTRAINT [PK_KeyMaster] PRIMARY KEY CLUSTERED ([Key1] ASC, [Key2] ASC))");
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            using (var masterConnection = new SqlConnection(@"Data Source=(LocalDb)\mssqllocaldb;Initial Catalog=Master;Integrated Security=True"))
            {
                masterConnection.Open();
                try
                {
                    masterConnection.Execute(@" DROP DATABASE DapperSimpleCrudTestDb; ");
                }
                catch
                { }
            }
            _connection.Close();
        }
    }
}

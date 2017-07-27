using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SimpleCRUD.Core.Tests.Models;
using Dapper;

namespace SimpleCRUD.Core.Tests
{
    public abstract class SimpleCRUDCoreBaseTests
    {
        protected static DbConnection _connection;

        [TestMethod]
        public void TestInsertWithSpecifiedTableName()
        {
            var id = _connection.Insert(new User { Name = "TestInsertWithSpecifiedTableName", Age = 10 });
            var user = _connection.Get<User>(id);

            _connection.Delete<User>(id);

            Assert.AreEqual("TestInsertWithSpecifiedTableName", user.Name);
        }

        [TestMethod]
        public void TestInsertUsingBigIntPrimaryKey()
        {
            var id = _connection.Insert<long, BigCar>(new BigCar { Make = "Big", Model = "Car" });
            var BigCar = _connection.Get<BigCar>(id);
            Assert.IsNotNull(BigCar);

            _connection.Delete<BigCar>(id);
            BigCar = _connection.Get<BigCar>(id);

            Assert.IsNull(BigCar);
        }

        [TestMethod]
        public void TestInsertUsingGenericLimitedFields()
        {
            //arrange
            var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };

            //act
            var id = _connection.Insert<int?, UserEditableSettings>(user);

            //assert
            var insertedUser = _connection.Get<User>(id);

            _connection.Delete<User>(id);

            Assert.IsNull(insertedUser.ScheduledDayOff);
        }

        [TestMethod]
        public void TestInsertUsingGenericLimitedFieldsAsync()
        {
            //arrange
            var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };

            //act
            var idTask = _connection.InsertAsync<int?, UserEditableSettings>(user);
            idTask.Wait();
            var id = idTask.Result;

            //assert
            var insertedUser = _connection.Get<User>(id);
            _connection.Delete<User>(id);

            Assert.IsNull(insertedUser.ScheduledDayOff);
        }

        [TestMethod]
        public void TestSimpleGet()
        {
            var id = _connection.Insert(new User { Name = "UserTestSimpleGet", Age = 10 });
            var user = _connection.Get<User>(id);
            _connection.Delete<User>(id);
            Assert.AreEqual("UserTestSimpleGet", user.Name);
        }

        [TestMethod]
        public void TestDeleteById()
        {
            var id = _connection.Insert(new User { Name = "UserTestDeleteById", Age = 10 });
            _connection.Delete<User>(id);
            Assert.IsNull(_connection.Get<User>(id));
        }

        [TestMethod]
        public void TestDeleteByObject()
        {
            var id = _connection.Insert(new User { Name = "TestDeleteByObject", Age = 10 });
            var user = _connection.Get<User>(id);
            _connection.Delete(user);
            Assert.IsNull(_connection.Get<User>(id));
        }

        [TestMethod]
        public void TestSimpleGetList()
        {
            _connection.Insert(new User { Name = "TestSimpleGetList1", Age = 10 });
            _connection.Insert(new User { Name = "TestSimpleGetList2", Age = 10 });
            var user = _connection.GetList<User>(new { });
            var userCount = user.Count();
            _connection.Execute("Delete from Users");

            Assert.AreEqual(2, userCount);
        }

        [TestMethod]
        public void TestFilteredGetList()
        {
            _connection.Insert(new User { Name = "TestFilteredGetList1", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetList2", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetList3", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetList4", Age = 11 });

            var user = _connection.GetList<User>(new { Age = 10 });

            var userCount = user.Count();
            _connection.Execute("Delete from Users");

            Assert.AreEqual(3, userCount);
        }
        [TestMethod]
        public void TestFilteredGetListWithMultipleKeys()
        {
            _connection.Insert(new KeyMaster { Key1 = 1, Key2 = 1 });
            _connection.Insert(new KeyMaster { Key1 = 1, Key2 = 2 });
            _connection.Insert(new KeyMaster { Key1 = 1, Key2 = 3 });
            _connection.Insert(new KeyMaster { Key1 = 2, Key2 = 4 });

            var keyMasters = _connection.GetList<KeyMaster>(new { Key1 = 1 });
            var keycount = keyMasters.Count();
            _connection.Execute("Delete from KeyMaster");
            Assert.AreEqual(3, keycount);
        }

        [TestMethod]
        public void TestFilteredWithSQLGetList()
        {
            _connection.Insert(new User { Name = "TestFilteredWithSQLGetList1", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredWithSQLGetList2", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredWithSQLGetList3", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredWithSQLGetList4", Age = 11 });

            var user = _connection.GetList<User>("where Name like 'TestFilteredWithSQLGetList%' and Age = 10");
            var userCount = user.Count();
            _connection.Execute("Delete from Users");

            Assert.AreEqual(3, userCount);
        }

        [TestMethod]
        public void TestGetListWithNullWhere()
        {
            _connection.Insert(new User { Name = "TestGetListWithNullWhere", Age = 10 });
            var user = _connection.GetList<User>(null);
            var userCount = user.Count();
            _connection.Execute("Delete from Users");

            Assert.AreEqual(1, userCount);
        }

        [TestMethod]
        public void TestGetListWithoutWhere()
        {
            _connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10 });
            var user = _connection.GetList<User>(null);
            var userCount = user.Count();
            _connection.Execute("Delete from Users");

            Assert.AreEqual(1, userCount);
        }

        [TestMethod]
        public void TestGetListWithParameters()
        {
            _connection.Insert(new User { Name = "TestsGetListWithParameters1", Age = 10 });
            _connection.Insert(new User { Name = "TestsGetListWithParameters2", Age = 10 });
            _connection.Insert(new User { Name = "TestsGetListWithParameters3", Age = 10 });
            _connection.Insert(new User { Name = "TestsGetListWithParameters4", Age = 11 });

            var user = _connection.GetList<User>("where Age > @Age", new { Age = 10 });
            var userCount = user.Count();

            _connection.Execute("Delete from Users");

            Assert.AreEqual(1, userCount);
        }

        [TestMethod]
        public void TestGetWithReadonlyProperty()
        {
            var id = _connection.Insert(new User { Name = "TestGetWithReadonlyProperty", Age = 10 });
            var user = _connection.Get<User>(id);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(DateTime.Now.Year, user.CreatedDate.Year);
        }

        [TestMethod]
        public void TestInsertWithReadonlyProperty()
        {
            var id = _connection.Insert(new User { Name = "TestInsertWithReadonlyProperty", Age = 10, CreatedDate = new DateTime(2001, 1, 1) });
            var user = _connection.Get<User>(id);
            _connection.Execute("Delete from Users");
            //the date can't be 2001 - it should be the autogenerated date from the database
            Assert.AreEqual(DateTime.Now.Year, user.CreatedDate.Year);
        }

        [TestMethod]
        public void TestUpdateWithReadonlyProperty()
        {
            var id = _connection.Insert(new User { Name = "TestUpdateWithReadonlyProperty", Age = 10 });
            var user = _connection.Get<User>(id);
            user.Age = 11;
            user.CreatedDate = new DateTime(2001, 1, 1);
            _connection.Update(user);
            user = _connection.Get<User>(id);
            //don't allow changing created date since it has a readonly attribute
            _connection.Execute("Delete from Users");
            Assert.AreEqual(DateTime.Now.Year, user.CreatedDate.Year);
        }

        [TestMethod]
        public void TestGetWithNotMappedProperty()
        {
            var id = _connection.Insert(new User { Name = "TestGetWithNotMappedProperty", Age = 10, NotMappedInt = 1000 });
            var user = _connection.Get<User>(id);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(0, user.NotMappedInt);
        }

        [TestMethod]
        public void TestInsertWithNotMappedProperty()
        {
            var id = _connection.Insert(new User { Name = "TestInsertWithNotMappedProperty", Age = 10, CreatedDate = new DateTime(2001, 1, 1), NotMappedInt = 1000 });
            var user = _connection.Get<User>(id);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(0, user.NotMappedInt);
        }

        [TestMethod]
        public void TestUpdateWithNotMappedProperty()
        {
            var id = _connection.Insert(new User { Name = "TestUpdateWithNotMappedProperty", Age = 10 });
            var user = _connection.Get<User>(id);
            user.Age = 11;
            user.CreatedDate = new DateTime(2001, 1, 1);
            user.NotMappedInt = 1234;
            _connection.Update(user);
            user = _connection.Get<User>(id);

            _connection.Execute("Delete from Users");

            Assert.AreEqual(0, user.NotMappedInt);
        }

        [TestMethod]
        public void TestInsertWithSpecifiedKey()
        {
            var id = _connection.Insert(new Car { Make = "Honda", Model = "Civic" });
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var newid = (int)_connection.Insert(new Car { Make = "Honda", Model = "Civic" });
            var newitem = _connection.Get<Car>(newid);
            newitem.Make = "Toyota";
            _connection.Update(newitem);
            var updateditem = _connection.Get<Car>(newid);
            _connection.Delete<Car>(newid);

            Assert.AreEqual("Toyota", updateditem.Make);
        }

        /// <summary>
        /// We expect scheduled day off to NOT be updated, since it's not a property of UserEditableSettings
        /// </summary>
        [TestMethod]
        public void TestUpdateUsingGenericLimitedFields()
        {
            //arrange
            var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };
            user.Id = _connection.Insert(user) ?? 0;

            user.ScheduledDayOff = DayOfWeek.Thursday;
            var userAsEditableSettings = (UserEditableSettings)user;
            userAsEditableSettings.Name = "User++";

            _connection.Update(userAsEditableSettings);

            //act
            var insertedUser = _connection.Get<User>(user.Id);

            _connection.Delete<User>(user.Id);

            //assert
            Assert.AreEqual("User++", insertedUser.Name);
            Assert.AreEqual(DayOfWeek.Friday, insertedUser.ScheduledDayOff);
        }

        /// <summary>
        /// We expect scheduled day off to NOT be updated, since it's not a property of UserEditableSettings
        /// </summary>
        [TestMethod]
        public void TestUpdateUsingGenericLimitedFieldsAsync()
        {
            //arrange
            var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };
            user.Id = _connection.Insert(user) ?? 0;

            user.ScheduledDayOff = DayOfWeek.Thursday;
            var userAsEditableSettings = (UserEditableSettings)user;
            userAsEditableSettings.Name = "User++";

            _connection.UpdateAsync(userAsEditableSettings).Wait();

            //act
            var insertedUser = _connection.Get<User>(user.Id);

            _connection.Delete<User>(user.Id);

            //assert
            Assert.AreEqual("User++", insertedUser.Name);
            Assert.AreEqual(DayOfWeek.Friday, insertedUser.ScheduledDayOff);
        }

        [TestMethod]
        public void TestDeleteByObjectWithAttributes()
        {
            var id = _connection.Insert(new Car { Make = "Honda", Model = "Civic" });
            var car = _connection.Get<Car>(id);
            _connection.Delete(car);
            Assert.IsNull(_connection.Get<Car>(id));
        }

        [TestMethod]
        public void TestDeleteByMultipleKeyObjectWithAttributes()
        {
            var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
            _connection.Insert(keyMaster);
            var car = _connection.Get<KeyMaster>(new { Key1 = 1, Key2 = 2 });
            _connection.Delete(car);
            Assert.IsNull(_connection.Get<KeyMaster>(keyMaster));
        }

        [TestMethod]
        public void TestComplexTypesMarkedEditableAreSaved()
        {
            var id = (int)_connection.Insert(new User { Name = "User", Age = 11, ScheduledDayOff = DayOfWeek.Friday });
            var user1 = _connection.Get<User>(id);
            _connection.Delete(user1);
            Assert.AreEqual(DayOfWeek.Friday, user1.ScheduledDayOff);
        }

        [TestMethod]
        public void TestNullableSimpleTypesAreSaved()
        {
            var id = (int)_connection.Insert(new User1 { Name = "User", Age = 11, ScheduledDayOff = 2 });
            var user1 = _connection.Get<User1>(id);
            _connection.Delete(user1);
            Assert.AreEqual(2, user1.ScheduledDayOff);
        }

        [TestMethod]
        public void TestInsertIntoDifferentSchema()
        {
            if (Dapper.SimpleCRUD.GetDialect() != Dapper.SimpleCRUD.Dialect.SQLServer.ToString())
                return;

            var id = _connection.Insert(new CarLog { LogNotes = "blah blah blah" });
            _connection.Delete<CarLog>(id);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestGetFromDifferentSchema()
        {
            if (Dapper.SimpleCRUD.GetDialect() != Dapper.SimpleCRUD.Dialect.SQLServer.ToString())
                return;

            var id = _connection.Insert(new CarLog { LogNotes = "TestGetFromDifferentSchema" });
            var carlog = _connection.Get<CarLog>(id);
            _connection.Delete<CarLog>(id);
            Assert.AreEqual("TestGetFromDifferentSchema", carlog.LogNotes);
        }

        [TestMethod]
        public void TestTryingToGetFromTableInSchemaWithoutDataAnnotationShouldFail()
        {
            try
            {
                _connection.Get<SchemalessCarLog>(1);
            }
            catch (Exception)
            {
                //we expect to get an exception, so return
                return;
            }

            //if we get here without throwing an exception, the test failed.
            Assert.Fail("Expected exception not triggered");
        }
        [TestMethod]
        public void TestGetFromTableWithNonIntPrimaryKey()
        {
            //note - there's not support for inserts without a non-int id, so drop down to a normal execute
            _connection.Execute("INSERT INTO CITY (NAME, POPULATION) VALUES ('Morgantown', 31000)");
            var city = _connection.Get<City>("Morgantown");
            Assert.AreEqual(31000, city.Population);
        }
        [TestMethod]
        public void TestDeleteFromTableWithNonIntPrimaryKey()
        {
            //note - there's not support for inserts without a non-int id, so drop down to a normal execute
            _connection.Execute("INSERT INTO CITY (NAME, POPULATION) VALUES ('Fairmont', 18737)");
            Assert.AreEqual(1, _connection.Delete<City>("Fairmont"));
        }

        [TestMethod]
        public void TestNullableEnumInsert()
        {
            _connection.Insert(new User { Name = "Enum-y", Age = 10, ScheduledDayOff = DayOfWeek.Thursday });
            var user = _connection.GetList<User>(new { Name = "Enum-y" }).FirstOrDefault() ?? new User();
            _connection.Delete<User>(user.Id);
            Assert.AreEqual(DayOfWeek.Thursday, user.ScheduledDayOff);
        }

        //dialect test 

        //[TestMethod]
        //public void TestChangeDialect()
        //{
        //    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
        //    SimpleCRUD.GetDialect().IsEqualTo(SimpleCRUD.Dialect.SQLServer.ToString());
        //    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
        //    SimpleCRUD.GetDialect().IsEqualTo(SimpleCRUD.Dialect.PostgreSQL.ToString());
        //}


        //  A GUID is being created and returned on insert but never actually
        //applied to the insert query.

        //This can be seen on a table where the key
        //is a GUID and defaults to (newid()) and no GUID is provided on the
        //insert. Dapper will generate a GUID but it is not applied so the GUID is
        //generated by newid() but the Dapper GUID is returned instead which is
        //incorrect.


        //GUID primary key tests
        [TestMethod]
        public void TestInsertIntoTableWithUnspecifiedGuidKey()
        {
            var id = _connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "GuidUser" });

            var record = _connection.Get<GUIDTest>(id);

            _connection.Delete<GUIDTest>(id);

            Assert.AreEqual("Guid", id.GetType().Name);
            Assert.AreEqual("GuidUser", record.Name);
        }

        [TestMethod]
        public void TestInsertIntoTableWithGuidKey()
        {
            var guid = new Guid("1a6fb33d-7141-47a0-b9fa-86a1a1945da9");
            var id = _connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "InsertIntoTableWithGuidKey", Id = guid });
            _connection.Delete<GUIDTest>(id);

            Assert.AreEqual(guid, id);
        }

        [TestMethod]
        public void TestGetRecordWithGuidKey()
        {
            var guid = new Guid("2a6fb33d-7141-47a0-b9fa-86a1a1945da9");
            _connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "GetRecordWithGuidKey", Id = guid });
            var id = _connection.GetList<GUIDTest>(null).First().Id;
            var record = _connection.Get<GUIDTest>(id);
            _connection.Delete<GUIDTest>(id);

            Assert.AreEqual("GetRecordWithGuidKey", record.Name);
        }

        [TestMethod]
        public void TestDeleteRecordWithGuidKey()
        {
            var guid = new Guid("3a6fb33d-7141-47a0-b9fa-86a1a1945da9");
            _connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "DeleteRecordWithGuidKey", Id = guid });
            var id = _connection.GetList<GUIDTest>(null).First().Id;
            _connection.Delete<GUIDTest>(id);
            Assert.IsNull(_connection.Get<GUIDTest>(id));
        }

        //async  tests
        [TestMethod]
        public void TestMultiInsertASync()
        {
            var tasks = new List<Task>();
            tasks.Add(_connection.InsertAsync(new User { Name = "TestMultiInsertASync1", Age = 10 }));
            tasks.Add(_connection.InsertAsync(new User { Name = "TestMultiInsertASync2", Age = 10 }));
            tasks.Add(_connection.InsertAsync(new User { Name = "TestMultiInsertASync3", Age = 10 }));
            tasks.Add(_connection.InsertAsync(new User { Name = "TestMultiInsertASync4", Age = 11 }));

            Task.WaitAll(tasks.ToArray());

            var list = _connection.GetList<User>(new { Age = 10 });
            _connection.Execute("Delete from Users");

            Assert.AreEqual(3, list.Count());
        }

        [TestMethod]
        public void MultiInsertWithGuidAsync()
        {
            var tasks = new List<Task>();
            tasks.Add(_connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" }));
            tasks.Add(_connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" }));
            tasks.Add(_connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" }));
            tasks.Add(_connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" }));

            Task.WaitAll(tasks.ToArray());

            var list = _connection.GetList<GUIDTest>(new { Name = "MultiInsertWithGuidAsync" });
            _connection.Execute("Delete from GUIDTest");
            Assert.AreEqual(4, list.Count());
        }

        [TestMethod]
        public void TestSimpleGetAsync()
        {
            var id = _connection.Insert(new User { Name = "TestSimpleGetAsync", Age = 10 });
            var user = _connection.GetAsync<User>(id);
            _connection.Delete<User>(id);
            Assert.AreEqual("TestSimpleGetAsync", user.Result.Name);
        }

        [TestMethod]
        public void TestMultipleKeyGetAsync()
        {
            var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
            _connection.Insert(keyMaster);
            var result = _connection.GetAsync<KeyMaster>(new { Key1 = 1, Key2 = 2 });
            _connection.Delete(keyMaster);

            Assert.AreEqual(1, result.Result.Key1);
            Assert.AreEqual(2, result.Result.Key2);
        }

        [TestMethod]
        public void TestDeleteByIdAsync()
        {
            var id = _connection.Insert(new User { Name = "UserAsyncDelete", Age = 10 });
            _connection.DeleteAsync<User>(id);
            //tiny wait to let the delete happen
            System.Threading.Thread.Sleep(300);
            Assert.IsNull(_connection.Get<User>(id));
        }

        [TestMethod]
        public void TestDeleteByObjectAsync()
        {
            var id = _connection.Insert(new User { Name = "TestDeleteByObjectAsync", Age = 10 });
            var user = _connection.Get<User>(id);
            _connection.DeleteAsync(user);
            _connection.Delete<User>(id);
            Assert.IsNull(_connection.Get<User>(id));
        }

        [TestMethod]
        public void TestDeleteByMultipleKeyObject()
        {
            var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
            _connection.Insert(keyMaster);
            _connection.Get<KeyMaster>(keyMaster);
            _connection.Delete<KeyMaster>(new { Key1 = 1, Key2 = 2 });
            _connection.Delete(keyMaster);
            Assert.IsNull(_connection.Get<KeyMaster>(keyMaster));
        }

        [TestMethod]
        public void TestSimpleGetListAsync()
        {
            _connection.Insert(new User { Name = "TestSimpleGetListAsync1", Age = 10 });
            _connection.Insert(new User { Name = "TestSimpleGetListAsync2", Age = 10 });
            var user = _connection.GetListAsync<User>(new { });
            _connection.Execute("Delete from Users");
            Assert.AreEqual(2, user.Result.Count());
        }

        [TestMethod]
        public void TestFilteredGetListAsync()
        {
            _connection.Insert(new User { Name = "TestFilteredGetListAsync1", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListAsync2", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListAsync3", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListAsync4", Age = 11 });

            var user = _connection.GetListAsync<User>(new { Age = 10 });
            _connection.Execute("Delete from Users");
            Assert.AreEqual(3, user.Result.Count());
        }

        [TestMethod]
        public void TestFilteredGetListParametersAsync()
        {
            _connection.Insert(new User { Name = "TestFilteredGetListParametersAsync1", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListParametersAsync2", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListParametersAsync3", Age = 10 });
            _connection.Insert(new User { Name = "TestFilteredGetListParametersAsync4", Age = 11 });

            var user = _connection.GetListAsync<User>("where Age = @Age", new { Age = 10 });
            _connection.Execute("Delete from Users");
            Assert.AreEqual(3, user.Result.Count());
        }

        [TestMethod]
        public void TestRecordCountAsync()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetList<User>(null);

            var resultListCount = resultlist.Count();
            var recordCountUser = _connection.RecordCountAsync<User>().Result;
            var resultQueryCount = _connection.RecordCountAsync<User>("where age = 10 or age = 11").Result;

            _connection.Execute("Delete from Users");

            Assert.AreEqual(30, resultListCount);
            Assert.AreEqual(30, recordCountUser);
            Assert.AreEqual(2, resultQueryCount);
        }

        [TestMethod]
        public void TestRecordCountByObjectAsync()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetList<User>(null);

            var resultListCount = resultlist.Count();
            var recordCountUser = _connection.RecordCountAsync<User>().Result;
            var resultQueryCount = _connection.RecordCountAsync<User>(new { age = 10 }).Result;

            _connection.Execute("Delete from Users");

            Assert.AreEqual(30, resultlist.Count());
            Assert.AreEqual(30, recordCountUser);
            Assert.AreEqual(1, resultQueryCount);
        }

        //column attribute tests

        [TestMethod]
        public void TestInsertWithSpecifiedColumnName()
        {
            var itemId = _connection.Insert(new StrangeColumnNames { Word = "InsertWithSpecifiedColumnName", StrangeWord = "Strange 1" });
            _connection.Delete<StrangeColumnNames>(itemId);
            Assert.AreEqual(1, itemId);
        }

        [TestMethod]
        public void TestDeleteByObjectWithSpecifiedColumnName()
        {
            var itemId = _connection.Insert(new StrangeColumnNames { Word = "TestDeleteByObjectWithSpecifiedColumnName", StrangeWord = "Strange 1" });
            var strange = _connection.Get<StrangeColumnNames>(itemId);
            _connection.Delete(strange);
            Assert.IsNull(_connection.Get<StrangeColumnNames>(itemId));
        }

        [TestMethod]
        public void TestSimpleGetListWithSpecifiedColumnName()
        {
            var id1 = _connection.Insert(new StrangeColumnNames { Word = "TestSimpleGetListWithSpecifiedColumnName1", StrangeWord = "Strange 2", });
            var id2 = _connection.Insert(new StrangeColumnNames { Word = "TestSimpleGetListWithSpecifiedColumnName2", StrangeWord = "Strange 3", });
            var strange = _connection.GetList<StrangeColumnNames>(new { });
            _connection.Delete<StrangeColumnNames>(id1);
            _connection.Delete<StrangeColumnNames>(id2);
            Assert.AreEqual("Strange 2", strange.First().StrangeWord);
            Assert.AreEqual(2, strange.Count());
        }

        [TestMethod]
        public void TestUpdateWithSpecifiedColumnName()
        {
            var newid = (int)_connection.Insert(new StrangeColumnNames { Word = "Word Insert", StrangeWord = "Strange Insert" });
            var newitem = _connection.Get<StrangeColumnNames>(newid);
            newitem.Word = "Word Update";
            _connection.Update(newitem);
            var updateditem = _connection.Get<StrangeColumnNames>(newid);
            _connection.Delete<StrangeColumnNames>(newid);
            Assert.AreEqual("Word Update", updateditem.Word);
        }

        [TestMethod]
        public void TestFilteredGetListWithSpecifiedColumnName()
        {
            _connection.Insert(new StrangeColumnNames { Word = "Word 5", StrangeWord = "Strange 1", });
            _connection.Insert(new StrangeColumnNames { Word = "Word 6", StrangeWord = "Strange 2", });
            _connection.Insert(new StrangeColumnNames { Word = "Word 7", StrangeWord = "Strange 2", });
            _connection.Insert(new StrangeColumnNames { Word = "Word 8", StrangeWord = "Strange 2", });

            var strange = _connection.GetList<StrangeColumnNames>(new { StrangeWord = "Strange 2" });
            _connection.Execute("Delete from StrangeColumnNames");
            Assert.AreEqual(3, strange.Count());
        }

        [TestMethod]
        public void TestGetListPaged()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetListPaged<User>(2, 10, null, null);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(10, resultlist.Count());
            Assert.AreEqual("Person 14", resultlist.Skip(4).First().Name);
        }

        [TestMethod]
        public void TestGetListPagedWithParameters()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetListPaged<User>(1, 30, "where Age > @Age", null, new { Age = 14 });
            _connection.Execute("Delete from Users");
            Assert.AreEqual(15, resultlist.Count());
            Assert.AreEqual("Person 15", resultlist.First().Name);
        }


        [TestMethod]
        public void TestGetListPagedWithSpecifiedPrimaryKey()
        {
            int x = 0;
            do
            {
                _connection.Insert(new StrangeColumnNames { Word = "Word " + x, StrangeWord = "Strange " + x });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetListPaged<StrangeColumnNames>(2, 10, null, null);
            _connection.Execute("Delete from StrangeColumnNames");
            Assert.AreEqual(10, resultlist.Count());
            Assert.AreEqual("Word 14", resultlist.Skip(4).First().Word);
        }

        [TestMethod]
        public void TestGetListPagedWithWhereClause()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist1 = _connection.GetListPaged<User>(1, 3, "Where Name LIKE 'Person 2%'", "age desc");
            Assert.AreEqual(3, resultlist1.Count());

            var resultlist = _connection.GetListPaged<User>(2, 3, "Where Name LIKE 'Person 2%'", "age desc");

            _connection.Execute("Delete from Users");

            Assert.AreEqual(3, resultlist.Count());
            Assert.AreEqual("Person 25", resultlist.Skip(1).First().Name);
        }

        [TestMethod]
        public void TestDeleteListWithWhereClause()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            _connection.DeleteList<User>("Where age > 9");
            var resultlist = _connection.GetList<User>(null);
            _connection.Execute("Delete from Users");

            Assert.AreEqual(10, resultlist.Count());
        }

        [TestMethod]
        public void TestDeleteListWithWhereObject()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 10);

            _connection.DeleteList<User>(new { age = 9 });
            var resultlist = _connection.GetList<User>(null);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(9, resultlist.Count());
        }

        [TestMethod]
        public void TestDeleteListWithParameters()
        {
            int x = 1;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 10);

            _connection.DeleteList<User>("where age >= @Age", new { Age = 9 });
            var resultlist = _connection.GetList<User>(null);
            _connection.Execute("Delete from Users");
            Assert.AreEqual(8, resultlist.Count());
        }

        [TestMethod]
        public void TestRecordCountWhereClause()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetList<User>(null);
            var resultListCount = resultlist.Count();
            var recordCount = _connection.RecordCount<User>();
            var queryCount = _connection.RecordCount<User>("where age = 10 or age = 11");

            _connection.Execute("Delete from Users");

            Assert.AreEqual(30, resultListCount);
            Assert.AreEqual(30, recordCount);
            Assert.AreEqual(2, queryCount);
        }

        [TestMethod]
        public void TestRecordCountWhereObject()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetList<User>(null);

            var resultListCount = resultlist.Count();
            var recordCount = _connection.RecordCount<User>();
            var queryCount = _connection.RecordCount<User>(new { age = 10 });

            _connection.Execute("Delete from Users");

            Assert.AreEqual(30, resultListCount);
            Assert.AreEqual(30, recordCount);
            Assert.AreEqual(1, queryCount);
        }

        [TestMethod]
        public void TestRecordCountParameters()
        {
            int x = 0;
            do
            {
                _connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                x++;
            } while (x < 30);

            var resultlist = _connection.GetList<User>(null);
            var recordCount = _connection.RecordCount<User>("where Age > 15");

            _connection.Execute("Delete from Users");

            Assert.AreEqual(30, resultlist.Count());
            Assert.AreEqual(14, recordCount);
        }

        [TestMethod]
        public void TestInsertWithSpecifiedPrimaryKey()
        {
            var id = _connection.Insert(new UserWithoutAutoIdentity() { Id = 999, Name = "User999", Age = 10 });
            Assert.AreEqual(999, id);
            var user = _connection.Get<UserWithoutAutoIdentity>(999);
            _connection.Execute("Delete from UserWithoutAutoIdentity");
            Assert.AreEqual("User999", user.Name);
        }


        [TestMethod]
        public void TestInsertWithSpecifiedPrimaryKeyAsync()
        {
            var id = _connection.InsertAsync(new UserWithoutAutoIdentity() { Id = 999, Name = "User999Async", Age = 10 });
            Assert.AreEqual(999, id.Result);
            var user = _connection.GetAsync<UserWithoutAutoIdentity>(999);
            _connection.Execute("Delete from UserWithoutAutoIdentity");
            Assert.AreEqual("User999Async", user.Result.Name);
        }

        [TestMethod]
        public void TestInsertWithMultiplePrimaryKeysAsync()
        {
            var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
            _connection.InsertAsync(keyMaster).Wait();
            var result = _connection.GetAsync<KeyMaster>(new { Key1 = 1, Key2 = 2 });
            Assert.AreEqual(1, result.Result.Key1);
            Assert.AreEqual(2, result.Result.Key2);
        }

        [TestMethod]
        public void TestGetListNullableWhere()
        {
            _connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10, ScheduledDayOff = DayOfWeek.Friday });
            _connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10 });

            //test with null property
            var list = _connection.GetList<User>(new { ScheduledDayOff = (DayOfWeek?)null });
            Assert.AreEqual(1, list.Count());

            // test with db.null value
            list = _connection.GetList<User>(new { ScheduledDayOff = DBNull.Value });

            _connection.Execute("Delete from Users");

            Assert.AreEqual(1, list.Count());
        }

        //ignore attribute tests
        //i cheated here and stuffed all of these in one test
        //didn't implement in postgres or mysql tests yet
        [TestMethod]
        public void IgnoreProperties()
        {
            var itemId = _connection.Insert(new IgnoreColumns() { IgnoreInsert = "OriginalInsert", IgnoreUpdate = "OriginalUpdate", IgnoreSelect = "OriginalSelect", IgnoreAll = "OriginalAll" });
            try
            {
                var item = _connection.Get<IgnoreColumns>(itemId);
                //verify insert column was ignored
                Assert.IsNull(item.IgnoreInsert);

                //verify select value wasn't selected 
                Assert.IsNull(item.IgnoreSelect);

                //verify the column is really there via straight dapper
                var fromDapper = _connection.Query<IgnoreColumns>("Select * from IgnoreColumns where Id = @Id", new { Id = itemId }).First();
                Assert.AreEqual("OriginalSelect", fromDapper.IgnoreSelect);

                //change value and update
                item.IgnoreUpdate = "ChangedUpdate";
                _connection.Update(item);

                //verify that update didn't take effect
                item = _connection.Get<IgnoreColumns>(itemId);
                Assert.AreEqual("OriginalUpdate", item.IgnoreUpdate);

                var allColumnDapper = _connection.Query<IgnoreColumns>("Select IgnoreAll from IgnoreColumns where Id = @Id", new { Id = itemId }).First();
                Assert.IsNull(allColumnDapper.IgnoreAll);
            }
            finally
            {
                _connection.Delete<IgnoreColumns>(itemId);
            }
        }

    }

}



# Dapper.SimpleCRUD.Core
Simple CRUD helpers for Dapper based on .NET Standard 1.3

## Features

<img  align="right" src="https://raw.githubusercontent.com/ericdc1/Dapper.SimpleCRUD/master/images/SimpleCRUD-200x200.png" alt="SimpleCRUD">

Dapper.SimpleCRUD.Core is a porting for the .NET Standard 1.3  of the excelent extension for Dapper created by Eric Coffman.

See usage instructions in **Eric Coffman's** original project in https://github.com/ericdc1/Dapper.SimpleCRUD.

This version has also the following changes and improvements:

##### 1) Tests were migrated to Unit Tests (only SQL Server for now)

##### 2) Possibility to use strings as primary keys

##### 3) Possibility to not retrieve certain fields on the Get functions
The following functions have a new overload able to receive the **'string[] propertiesToBeIgnored'** parameter to be able to define whick fields you don't want to retrieve:
- Get&lt;Type&gt;(..., string[] propertiesToBeIgnored, ...)
- GetList&lt;Type&gt;(..., string[] propertiesToBeIgnored, ...)
- GetListPaged&lt;Type&gt;(..., string[] propertiesToBeIgnored, ...)

By using this parameter you can specify which properties you don't want to be retrieved from the database.
This is usefull to avoid the read of large and sensitive fields.

##### 4) New Update functions to update only non-null fields of an object
The following functions were added to allow the update of non-nulled fields of an object.

```csharp
public static int UpdateNonNullProps(this IDbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null);
public static Task<int> UpdateNonNullPropsAsync<TEntity>(this IDbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null, System.Threading.CancellationToken? token = null);
```
Example usage:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
}
...
// change only the status of a user
connection.UpdateNonNullProps( new User{ Id=1, Status="inactive" });
```
This is useful when we only want to update certain properties of the object with no need to read the object from database, change the properties and re-save it back. 
This is more performant and also more thread safe as you only need one operation to update the object. 
>**But be aware with non-nullable properties because these will always be updated.**

##### 5) Added function to help in manipulating entries on Many-To-Many tables

```csharp
public static void UpdateManyToManyRelations<ManyToManyTable>(this IDbConnection connection,
                                                              string parentEntityIdFieldName, object parentId,
                                                              string childEntityIdFieldName, IEnumerable<Guid> newChildIds,
                                                              IDbTransaction transaction = null, int? commandTimeout = null)
```
With this function you need only to provide the new list of items(newChildIds) to be linked with a parent object(parentEntityIdFieldName/parentId) in the &lt;ManyToManyTable&gt;
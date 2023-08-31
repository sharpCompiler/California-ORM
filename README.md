# California-ORM
  A very tiny .NET Object-Relational Mapping (ORM) NuGet package that works exclusively with SQL Server and ADO.NET. It extends the functionality of ADO.NET by providing a simple and elegant way to map query results to C# objects.
  
  
## Version
  For the latest version see Nuget web
  https://www.nuget.org/packages/California-ORM/

## Features
``` csharp
// Inserting new entity
Task<int> InsertAsync<T>(T entity, Dictionary<string, object?>? extraFields = null, SqlTransaction? transaction = null)

// Deleting entity
Task<int> DeleteAsync<T>(object entityId, SqlTransaction? transaction = null)

// Updaing entity
Task<int> UpdateAsync<T>(T entity, SqlTransaction? transaction = null)

// Getting entities using manually written SQL Query
Task<IEnumerable<T>> QueryAsync<T>(string sql, Dictionary<string, object?>? parameters = null, SqlTransaction? transaction = null)

// Exeucute quest that return no results 
Task<int> NonQueryAsync(string sql, Dictionary<string, object?>? parameters = null, SqlTransaction? transaction = null)

// Getting entity by Id
Task<T?> GetAsync<T>(object entityId, SqlTransaction? transaction = null)
```
        
Example
``` csharp
var person = new PersonModel
{
  CreatedDateTime = DateTime.Now,
  Id = Guid.NewGuid(),
  Name = "First and Lastname"
};
await using var con = new SqlConnection("Server=.\\SQLEXPRESS;Database=California-ORM;Trusted_Connection=True;TrustServerCertificate=True");
await con.OpenAsync();

await con.InsertAsync(person, extraFields: new Dictionary<string, object?> { {"ExtraFiled", null} });
```

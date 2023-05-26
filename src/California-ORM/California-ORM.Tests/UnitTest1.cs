using System.Data;
using System.Data.SqlClient;
using California_ORM.Sql;

namespace California_ORM.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var person = new Person()
        {
            Birthday = new DateOnly(1980,2,8),
            CreatedAt = DateTime.Now,
            Id = Guid.NewGuid(),
            Name = "Burim 'Hajrizaj"
        };

        var con = new SqlConnection("Persist Security Info=False;Initial Catalog=DapperSampleDb;Data Source=.\\sqlexpress;Trusted_Connection=yes;");
        await con.OpenAsync();

        con.Insert(person);
        
        Assert.IsTrue(con.State == ConnectionState.Open);
    }
}

[Table("Persons")]
public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateOnly Birthday { get; set; }
}
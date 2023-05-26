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
            Birthday = new DateTime(1980,2,8),
            CreatedAt = DateTime.Now,
            Id = Guid.NewGuid(),
            Name = "Burim 'Hajrizaj"
        };

        var con = new SqlConnection("Persist Security Info=False;Initial Catalog=DapperSampleDb;Data Source=.\\sqlexpress;Trusted_Connection=yes;");
        await con.OpenAsync();

        var p = con.Get<Person>(Guid.Parse("67B12E6A-6720-47C2-A6B2-544860AA0ECB"));
        
        Assert.IsTrue(con.State == ConnectionState.Open);
    }
}

[Table("Person")]
public class Person
{
    [PrimaryKey]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    //[IgnoreMember]
    public DateTime CreatedAt { get; set; }
    public DateTime Birthday { get; set; }
}
using California_ORM.Attributes;
using California_ORM.Extensions;
using Microsoft.Data.SqlClient;

namespace Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var p = new Person()
        {
            Name = "Burim Hajrizaj",
            Birthday = new DateOnly(1980, 2 , 8),
            CreatedAt = DateTime.Now,
            Description = "This will be ignored",
            Id = Guid.NewGuid()
        };
        try
        {
            await using var con =
                new SqlConnection(
                    "Server=.\\sqlexpress;Database=DapperSampleDb;Trusted_Connection=True;TrustServerCertificate=true;");
            await con.OpenAsync();

            await con.InsertAsync(p);
        }
        catch (Exception ex)
        {

        }
    }
}

public class Person
{
    [PrimaryKey]
    public Guid Id { get; set; }
    public string Name { get; set; }

    [IgnoreMember]
    public string Description { get; set; }
    public DateOnly Birthday { get; set; }
    public DateTime CreatedAt { get; set; }
}
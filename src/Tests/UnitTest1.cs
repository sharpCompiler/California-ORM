using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using California_ORM.Sql;
using California_ORM.Sql.Builder;

namespace Tests;


[TestClass]
public class MapperTest
{
    [TestMethod]
    public void Test_Person()
    {
        //arrange
        Mapper.MapClass<Person>()
            .ForMember(x => x.Id)
            .ForMember(x => x.Birthday)
            //.ForMember(x => x.Description)
            .ForMember(x => x.CreatedAt)
            .ForMember(x => x.Name);
        var person = new Person()
        {
            Name = "Burim Hajrizaj",
            Birthday = new DateOnly(1980,02,08),
            CreatedAt = DateTime.Now,
            Description = "The owner of California ORM",
            Id = Guid.NewGuid(),
        };
        var con = new SqlConnection("Server=.\\sqlexpress;Database=DapperSampleDb;Trusted_Connection=True;");
        con.Open();


        //act
        con.Insert(person);


        //assert


    }
}


//[TestClass]
//public class UnitTest1
//{
//    [TestMethod]
//    public void TestMethod1()
//    {
//        var person = new Person()
//        {
//            Name = "Burim Hajrizaj",
//            Birthday = new DateOnly(1980,2,8),
//            CreatedAt = DateTime.Now,
//            Description = "Two and a Half",
//            Id = Guid.NewGuid()
//        };
//        var con = new SqlConnection("Server=.\\sqlexpress;Database=DapperSampleDb;Trusted_Connection=True;");
//        con.Open();

//        con.Insert(person);
//    }
//}

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateOnly Birthday { get; set; }
    public DateTime CreatedAt { get; set; }
}
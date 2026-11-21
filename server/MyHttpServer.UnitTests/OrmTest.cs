using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniHttpServer.ORM;
using System.Diagnostics;

namespace OrmTests;

[TestClass]
[DoNotParallelize]
public class OrmContextIntegrationTests
{
    private class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int Age { get; set; }
    }

    private const string Table = "TestUsers";
    private const string MasterConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=master;TrustServerCertificate=True;";

    private static readonly object LocalDbLock = new();
    private static bool LocalDbStarted;

    private string _dbName = null!;
    private string _connectionString = null!;
    private OrmContext _orm = null!;

    [TestInitialize]
    public async Task Init()
    {
        EnsureLocalDbStarted();

        _dbName = "OrmTests_" + Guid.NewGuid().ToString("N");
        _connectionString = $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={_dbName};TrustServerCertificate=True;";

        await ExecOnMaster($@"
            IF DB_ID(N'{_dbName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];
            END;
            CREATE DATABASE [{_dbName}];");

        _orm = new OrmContext(_connectionString);

        await Exec($@"
            IF OBJECT_ID('{Table}') IS NOT NULL DROP TABLE {Table};
            CREATE TABLE {Table} (
                Id int IDENTITY(1,1) PRIMARY KEY,
                Name nvarchar(100) NULL,
                Email nvarchar(100) NULL,
                Age int NOT NULL
            );");
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (string.IsNullOrEmpty(_dbName))
            return;

        try
        {
            await ExecOnMaster($@"
                IF DB_ID(N'{_dbName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END;");
        }
        catch
        {
            
        }
    }

    private static void EnsureLocalDbStarted()
    {
        lock (LocalDbLock)
        {
            if (LocalDbStarted) return;
            try
            {
                using var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "sqllocaldb",
                    Arguments = "start MSSQLLocalDB",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                });
                p?.WaitForExit(15000);
            }
            catch
            {
               
            }
            LocalDbStarted = true;
        }
    }

    private static async Task ExecOnMaster(string sql)
    {
        await using var conn = new SqlConnection(MasterConnectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task Exec(string sql)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<object?> Scalar(string sql)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        return await cmd.ExecuteScalarAsync();
    }

    private static User NewUser(string name = "Anna", string email = "anna@example.com", int age = 22)
        => new User { Name = name, Email = email, Age = age };

    

    [TestMethod]
    public async Task Insert_ValidEntity_ReturnsOneAndRowExists()
    {
        int result = await _orm.InsertAsync(Table, NewUser());

        Assert.AreEqual(1, result);
        Assert.AreEqual(1, await Scalar($"SELECT COUNT(*) FROM {Table}"));
        Assert.AreEqual("Anna", await Scalar($"SELECT Name FROM {Table}"));
    }

    [TestMethod]
    public async Task Insert_IgnoresIdFromEntity_DatabaseGeneratesId()
    {
        var user = NewUser();
        user.Id = 999; 

        await _orm.InsertAsync(Table, user);

        Assert.AreEqual(1, await Scalar($"SELECT Id FROM {Table}"));
    }

    [TestMethod]
    public async Task Insert_TwoRows_IdsAreSequential()
    {
        await _orm.InsertAsync(Table, NewUser("A"));
        await _orm.InsertAsync(Table, NewUser("B"));

        Assert.AreEqual(2, await Scalar($"SELECT MAX(Id) FROM {Table}"));
    }

    [TestMethod]
    public async Task Insert_NullProperty_StoredAsDbNull()
    {
        var user = new User { Name = null!, Email = null!, Age = 5 };
        int result = await _orm.InsertAsync(Table, user);

        Assert.AreEqual(1, result);
        Assert.AreEqual(DBNull.Value, await Scalar($"SELECT Name FROM {Table}"));
        Assert.AreEqual(DBNull.Value, await Scalar($"SELECT Email FROM {Table}"));
    }

    [TestMethod]
    public async Task Insert_SqlInjectionInValue_IsStoredAsPlainText()
    {
        string evil = "x'); DROP TABLE TestUsers; --";

        await _orm.InsertAsync(Table, NewUser(name: evil));

        Assert.AreEqual(evil, await Scalar($"SELECT Name FROM {Table}"));
    }

    [TestMethod]
    public async Task Insert_AfterDelete_DoesNotReuseId()
    {
        await _orm.InsertAsync(Table, NewUser("A"));
        await _orm.DeleteAsync(Table, 1);

        await _orm.InsertAsync(Table, NewUser("B"));

        Assert.AreEqual(2, await Scalar($"SELECT Id FROM {Table} WHERE Name = 'B'"));
    }

 

    [TestMethod]
    public async Task GetAll_EmptyTable_ReturnsEmptyList() 
    {
        var users = await _orm.GetAllAsync<User>(Table);

        Assert.AreEqual(0, users.Count);
    }

    [TestMethod]
    public async Task GetById_ExistingId_ReturnsEntityWithMappedProperties()
    {
        await _orm.InsertAsync(Table, NewUser("A", "a@a.com", 10));
        await _orm.InsertAsync(Table, NewUser("B", "b@b.com", 20));

        var user = await _orm.GetByIdAsync<User>(Table, 2);

        Assert.IsNotNull(user);
        Assert.AreEqual(2, user!.Id);
        Assert.AreEqual("B", user.Name);
        Assert.AreEqual("b@b.com", user.Email);
        Assert.AreEqual(20, user.Age);
    }

    [TestMethod]
    public async Task GetById_NonExistingId_ReturnsNull()
    {
        await _orm.InsertAsync(Table, NewUser("A"));

        var user = await _orm.GetByIdAsync<User>(Table, 999);

        Assert.IsNull(user);
    }

    [TestMethod]
    public async Task GetById_NullInDatabase_KeepsDefaultValues()
    {
        await Exec($"INSERT INTO {Table} (Name, Email, Age) VALUES (NULL, NULL, 7)");

        var user = await _orm.GetByIdAsync<User>(Table, 1);

        Assert.IsNotNull(user);
        Assert.AreEqual("", user!.Name);
        Assert.AreEqual("", user.Email);
        Assert.AreEqual(7, user.Age);
    }

    [TestMethod]
    public async Task GetAll_WithRows_MapsAllProperties()
    {
     
        await _orm.InsertAsync(Table, NewUser("A", "a@a.com", 10));
        await _orm.InsertAsync(Table, NewUser("B", "b@b.com", 20));

        var users = await _orm.GetAllAsync<User>(Table);

        Assert.AreEqual(2, users.Count);
        Assert.AreEqual(1, users[0].Id);
        Assert.AreEqual("A", users[0].Name);
        Assert.AreEqual("a@a.com", users[0].Email);
        Assert.AreEqual(10, users[0].Age);
        Assert.AreEqual("B", users[1].Name);
    }

    [TestMethod]
    public async Task GetAll_NullInDatabase_KeepsDefaultValue() 
    {
        await Exec($"INSERT INTO {Table} (Name, Email, Age) VALUES (NULL, NULL, 7)");

        var users = await _orm.GetAllAsync<User>(Table);

        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("", users[0].Name);  
        Assert.AreEqual("", users[0].Email);
        Assert.AreEqual(7, users[0].Age);
    }

    [TestMethod]
    public async Task GetAll_ColumnWithoutProperty_IsIgnored()
    {
        await Exec($"ALTER TABLE {Table} ADD Extra nvarchar(50) NULL");
        await Exec($"INSERT INTO {Table} (Name, Email, Age, Extra) VALUES ('A', 'a@a.com', 1, 'lol')");

        var users = await _orm.GetAllAsync<User>(Table);

        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("A", users[0].Name);
    }

    [TestMethod]
    public async Task GetAll_ColumnNamesInDifferentCase_AreMapped() 
    {
        await Exec(@"
            CREATE TABLE TestUsersLower (
                id int IDENTITY(1,1) PRIMARY KEY,
                name nvarchar(100), email nvarchar(100), age int);
            INSERT INTO TestUsersLower (name, email, age) VALUES ('low', 'l@l.com', 3);");

        var users = await _orm.GetAllAsync<User>("TestUsersLower");

        Assert.AreEqual("low", users[0].Name);
        Assert.AreEqual(3, users[0].Age);
    }

 

    [TestMethod]
    public async Task Update_ExistingId_ReturnsOneAndChangesAllFields()
    {
        await _orm.InsertAsync(Table, NewUser("Old", "old@a.com", 1));

        int result = await _orm.UpdateAsync(Table, 1, NewUser("New", "new@a.com", 50));

        Assert.AreEqual(1, result);
        var u = (await _orm.GetAllAsync<User>(Table))[0];
        Assert.AreEqual("New", u.Name);
        Assert.AreEqual("new@a.com", u.Email);
        Assert.AreEqual(50, u.Age);
    }

    [TestMethod]
    public async Task Update_NonExistingId_ReturnsZero()
    {
        await _orm.InsertAsync(Table, NewUser());

        int result = await _orm.UpdateAsync(Table, 999, NewUser("X"));

        Assert.AreEqual(0, result);
        Assert.AreEqual("Anna", await Scalar($"SELECT Name FROM {Table}"));
    }

    [TestMethod]
    public async Task Update_OnlyTargetRowChanges()
    {
        await _orm.InsertAsync(Table, NewUser("A"));
        await _orm.InsertAsync(Table, NewUser("B"));

        await _orm.UpdateAsync(Table, 2, NewUser("B2"));

        var users = await _orm.GetAllAsync<User>(Table);
        Assert.AreEqual("A", users[0].Name);
        Assert.AreEqual("B2", users[1].Name);
    }

    [TestMethod]
    public async Task Update_IgnoresIdFromEntity() 
    {
        await _orm.InsertAsync(Table, NewUser());
        var changed = NewUser("Z");
        changed.Id = 500;

        await _orm.UpdateAsync(Table, 1, changed);

        Assert.AreEqual(1, await Scalar($"SELECT Id FROM {Table}"));
    }

    [TestMethod]
    public async Task Update_NullProperty_StoredAsDbNull()
    {
        await _orm.InsertAsync(Table, NewUser());

        await _orm.UpdateAsync(Table, 1, new User { Name = null!, Email = "e", Age = 1 });

        Assert.AreEqual(DBNull.Value, await Scalar($"SELECT Name FROM {Table}"));
    }

    

    [TestMethod]
    public async Task Delete_ExistingId_ReturnsOneAndRemovesRow()
    {
        await _orm.InsertAsync(Table, NewUser());

        int result = await _orm.DeleteAsync(Table, 1);

        Assert.AreEqual(1, result);
        Assert.AreEqual(0, await Scalar($"SELECT COUNT(*) FROM {Table}"));
    }

    [TestMethod]
    public async Task Delete_NonExistingId_ReturnsZero()
    {
        await _orm.InsertAsync(Table, NewUser());

        int result = await _orm.DeleteAsync(Table, 999);

        Assert.AreEqual(0, result);
        Assert.AreEqual(1, await Scalar($"SELECT COUNT(*) FROM {Table}"));
    }

    [TestMethod]
    public async Task Delete_OtherRowsRemain()
    {
        await _orm.InsertAsync(Table, NewUser("A"));
        await _orm.InsertAsync(Table, NewUser("B"));

        await _orm.DeleteAsync(Table, 1);

        var users = await _orm.GetAllAsync<User>(Table);
        Assert.AreEqual(1, users.Count);
        Assert.AreEqual("B", users[0].Name);
    }

    [TestMethod]
    public async Task Delete_FirstRow_DoesNotRenumberRemaining()
    {
        await _orm.InsertAsync(Table, NewUser("A"));
        await _orm.InsertAsync(Table, NewUser("B"));

        await _orm.DeleteAsync(Table, 1);

        var users = await _orm.GetAllAsync<User>(Table);
        Assert.AreEqual(1, users.Count);
        Assert.AreEqual(2, users[0].Id); 
    }


    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(null)]
    [DataRow("Users; DROP TABLE Users")]
    [DataRow("1abc")]
    [DataRow("a-b")]
    [DataRow("a b")]
    [DataRow("[Users]")]
    public async Task InvalidTableName_ThrowsArgumentException(string? name)
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _orm.GetAllAsync<User>(name!));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _orm.GetByIdAsync<User>(name!, 1));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _orm.InsertAsync(name!, NewUser()));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _orm.UpdateAsync(name!, 1, NewUser()));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _orm.DeleteAsync(name!, 1));
    }

    [TestMethod]
    public async Task NonExistingTable_ThrowsSqlException()
    {
        await Assert.ThrowsExceptionAsync<SqlException>(() => _orm.GetAllAsync<User>("NoSuchTable"));
    }

    [TestMethod]
    public async Task CancelledToken_ThrowsOperationCanceled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsExceptionAsync<TaskCanceledException>(
            () => _orm.GetAllAsync<User>(Table, cts.Token));
    }
}

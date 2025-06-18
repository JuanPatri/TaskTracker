using Microsoft.EntityFrameworkCore;
using Repository;

namespace RepositoryTest.Context;

public static class SqlContextFactory
{
    public static SqlContext CreateMemoryContext() 
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return new SqlContext(optionsBuilder.Options);
    }
}
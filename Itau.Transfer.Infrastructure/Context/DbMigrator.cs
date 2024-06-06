using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Itau.Transfer.Infrastructure.Context;

[ExcludeFromCodeCoverage]
public static class DbMigrator
{
    public static void Migrate(IApplicationBuilder app)
    {
        try
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        }
        catch (SqlException sqlException)
        {
            SentrySdk.CaptureException(sqlException);
            Console.WriteLine(sqlException);
            throw;
        }
    }
}
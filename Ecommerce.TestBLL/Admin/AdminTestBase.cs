
using FluentAssertions.Common;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MyDexef.API;
using MyDexef.Core.Entities;
using MyDexef.Core.Infrastructure;

using System;
using System.Data.Entity;

namespace MyDexef.TestBLL
{
    public class AdminTestBase
    {
        private IWebHost webHost;
        private DependencyResolverHelper _serviceProvider;
        private ApplicationDbContext context;
        public AdminTestBase( )
        {

            webHost = WebHost
                .CreateDefaultBuilder(new string [] { "Development" })
                .ConfigureServices(services =>
                {
                    DbContextOptions<MyDexefContext> dbContextOptions = new DbContextOptionsBuilder<MyDexefContext>()
                    .UseInMemoryDatabase(databaseName: "TestDB")
                    .Options;
                    services.AddDbContext<MyDexefContext>(opt => opt.UseInMemoryDatabase("TestDB"));
                    //services.AddScoped<IDbFactory, DbFactory>(s => new DbFactory(dbContextOptions));
                })
                .UseStartup<Startup>().Build();

            _serviceProvider = new DependencyResolverHelper(webHost);
            SetDbContext();
            SeedDbContext();
        }

        private void SetDbContext( )
        {
            context = _serviceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureCreated();

        }
        protected T Resolve<T>( )
        {
            return _serviceProvider.GetService<T>();
        }
        protected void SeedDbContext( )
        {
            if (context != null)
                context.SeedLockUpData();
        }
        protected void ClearDbContext( )
        {
            throw new NotImplementedException();
        }

        protected void DeleteDbContext( )
        {
            if (context != null)
                context.Database.EnsureDeleted();
        }


    }
}

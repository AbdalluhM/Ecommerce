using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ecommerce.Core.Infrastructure
{

    /// <summary>   A database factory. </summary>
    public class DbFactory : IDbFactory
    {
        /// <summary>   Context for the database. </summary>
        private ApplicationDbContext _dbContext { get; set; }
        /// <summary>   The connection. </summary>
        //private readonly DbContextOptions<EcommerceContext> options;
        private DbConnection _connection;
        private bool seeddata;
        /// <summary>   Default constructor. </summary>
        public DbFactory(DbConnection connection)
      {
            _connection = connection;
            seeddata = true;
            // _dbContext.Database.SetDbConnection(_connection);
        }
        public DbFactory( ApplicationDbContext context )
        {
            _dbContext = context;
            // _dbContext.Database.SetDbConnection(_connection);
        }
        //public DbFactory( DbContextOptions<EcommerceContext> options )
        //{
        //    _dbContext = new ApplicationDbContext(options);
        //    //_dbContext.Database.SetDbConnection(_connection);
        //}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="_connection">  The connection. </param>
        ///-------------------------------------------------------------------------------------------------

        //public DbFactory(DbContextOptions<EcommerceContext> _options )
        //{
        //    options = _options;
        //}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Initializes this object./. </summary>
        ///
        /// <returns>   An ApplicationDbContext. </returns>
        ///-------------------------------------------------------------------------------------------------

        public ApplicationDbContext Initialize()
        {
            //return _dbContext ?? (_dbContext = new ApplicationDbContext(options));
            if (_dbContext == null )
            {
                _dbContext = new ApplicationDbContext();
            }
            //if (seeddata)
            //{ 
            //  seeddata = false;
            // _dbContext.SeedLockUpData();    
            //}
            _dbContext.Database.SetDbConnection(_connection);
            return _dbContext;        
        }
    }
}

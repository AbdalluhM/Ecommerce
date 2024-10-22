using Ecommerce.Core.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using Ecommerce.Core.Helpers.Extensions;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Json;

namespace Ecommerce.Repositroy.Base
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A base repository. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    ///-------------------------------------------------------------------------------------------------

    public class BaseRepository<T> : IRepository<T> where T : EntityBase
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="dbFactory">    The database factory. </param>
        ///-------------------------------------------------------------------------------------------------

        public BaseRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            DbSet = DbContext.Set<T>();
        }

        /// <summary>   Context for the database. </summary>
        private ApplicationDbContext _dbContext;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a context for the database. </summary>
        ///
        /// <value> The database context. </value>
        ///-------------------------------------------------------------------------------------------------

        protected ApplicationDbContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Initialize()); }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enables the dynamic filter. </summary>
        ///
        /// <param name="filterName">   Name of the dynamic filter. </param>
        ///-------------------------------------------------------------------------------------------------

        public void EnableFilter(string filterName)
        {
            // _dbContext.EnableDynamicFilter(filterName);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Disables the query filter for specific entity. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------

        public IQueryable<T> DisableFilter()
        {
            //var x = DbContext.Set<CountryCurrency>().IgnoreQueryFilters();
            //var set = DbSet.IgnoreQueryFilters()
            //return set.Aggregate(set,()) ;
            return DbSet.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        }

        //Check is DbSet Has IsActive & IsDeleted Filters Actually
        private bool HasProperty(DbSet<T> dbSet, string propertyName)
        {
            return dbSet.EntityType.FindProperty(propertyName) != null;
        }
        private bool HasProperty(DbSet<T> dbSet, params string[] properties)
        {
            foreach (var prop in properties)
            {
                if (string.IsNullOrWhiteSpace(prop))
                    return false;
                if (dbSet.EntityType.FindProperty(prop) == null)
                    return false;
            }
            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Disables the query filter for specific entity for specific filter for all filters. </summary>
        ///
        ///-------------------------------------------------------------------------------------------------
        public IQueryable<T> DisableFilter(string filterName)
        {
            if (HasProperty(DbSet, nameof(DynamicFilters.IsActive), nameof(DynamicFilters.IsDeleted)))
            {
                if (filterName == nameof(DynamicFilters.IsActive))
                    return DisableFilter().Where(x => !x.IsDeleted);
                else if (filterName == nameof(DynamicFilters.IsDeleted))
                    return DisableFilter().Where(x => !x.IsActive);
                else
                    return DisableFilter();
            }
            else
                return DbSet.AsQueryable<T>();


        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the set the database belongs to. </summary>
        ///
        /// <value> The database set. </value>
        ///-------------------------------------------------------------------------------------------------

        public DbSet<T> DbSet { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the database factory. </summary>
        ///
        /// <value> The database factory. </value>
        ///-------------------------------------------------------------------------------------------------

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds entity. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ///
        /// <returns>   A T. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual T Add(T entity)
        {
            DbSet.Add(entity);
            return entity;
        }

        public virtual List<T> Add(List<T> entities)
        {
            if (entities != null && entities.Count() > 0)
                DbSet.AddRange(entities);
            return entities;
        }

        //public virtual async Task<EntityEntry<T>> AddAsync(T entity)
        //{
        //    var dbset = await DbSet.AddAsync(entity);
        //    return dbset;
        //}
        public virtual async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }





        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Updates the given entity. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ///
        /// <returns>   A T. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual T Update(T entity)
        {
            DbSet.Attach(entity);
            //_dbContext.Set<T>().AddOrUpdate(entity);            
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }
        public T Update(T item, params string[] changedPropertyNames)
        {
            _dbContext.Set<T>().Attach(item);
            foreach (var propertyName in changedPropertyNames)
            {
                // If we can't find the property, this line will throw an exception, 
                //which is good as we want to know about it
                _dbContext.Entry(item).Property(propertyName).IsModified = true;
            }
            return item;
        }
        public T Update(T entity, Expression<Func<T, object>>[] properties)
        {
            EntityEntry<T> entry = _dbContext.Entry(entity);
            entry.State = EntityState.Unchanged;
            foreach (var property in properties)
            {
                string propertyName = "";
                Expression bodyExpression = property.Body;
                if (bodyExpression.NodeType == ExpressionType.Convert && bodyExpression is UnaryExpression)
                {
                    Expression operand = ((UnaryExpression)property.Body).Operand;
                    propertyName = ((MemberExpression)operand).Member.Name;
                }
                //else
                //{
                //    propertyName = ExpressionHelper.GetExpressionText(property);
                //}
                entry.Property(propertyName).IsModified = true;
            }

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            _dbContext.SaveChanges();
            return entity;
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Deletes the given where. </summary>
        ///
        /// <param name="entity">   The entity to delete. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }


        public virtual void UpdateCrossTable(List<T> selectedObj, Expression<Func<T, bool>> ExistingRecords, string entityName)
        {

            List<int> selectedIds = selectedObj != null ? selectedObj.Select(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s))).ToList() : new List<int>();

            List<T> existingDBRecords = ((IQueryable)_dbContext.GetType().GetProperty(entityName).GetValue(_dbContext)).OfType<T>().Where(ExistingRecords).ToList();

            List<T> newEntitiesList = selectedObj != null ? selectedObj.Where(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s)) == 0).Select(s => s).ToList() : new List<T>();
            //Update the current entity
            for (int i = 0; i < selectedIds.Count; i++)
            {
                if (selectedIds[i] == 0)
                {
                    continue;
                }

                var entity = DbSet.Find(selectedIds[i]);// get the current entity in database
                var newObj = selectedObj.FirstOrDefault(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s)) == selectedIds[i]);

                if (entity != null && selectedIds[i] != 0)
                {
                    var createdByProp = entity.GetType().GetProperty("CreatedBy");
                    if (createdByProp != null)
                    {
                        var userCreatedId = createdByProp.GetValue(entity);
                        newObj.GetType().GetProperty("CreatedBy").SetValue(newObj, userCreatedId);
                    }

                    var createdateProp = entity.GetType().GetProperty("CreateDate");
                    if (createdateProp != null)
                    {
                        var userCreatedate = createdateProp.GetValue(entity);
                        newObj.GetType().GetProperty("CreateDate").SetValue(newObj, userCreatedate);
                    }


                    _dbContext.Entry(entity).CurrentValues.SetValues(newObj);
                }
            }

            // Delete the not existing entity in selected list from database
            var deleted = existingDBRecords.Where(x => !selectedIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x)))).ToList();
            foreach (T obj in deleted)
            {
                DbSet.Remove(obj);
            }

            // Add new entity in databsae
            for (int i = 0; i < newEntitiesList.Count; i++)
            {
                DbSet.Add(newEntitiesList[i]);
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Updates the cross table. </summary>
        ///
        /// <param name="selectedObj">      The many-to-many relation object. </param>
        /// <param name="ExistingRecords">  Where condition that will be used to get existing records
        ///                                                                    for cross table. </param>
        /// <param name="entityName">       Name of the entity as it exists in dbContext. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void UpdateCrossTable(List<T> selectedObj, Expression<Func<T, bool>> ExistingRecords, string entityName, bool isSoftDelete = false, int? currentUser = null, DateTime? currentDate = null)
        {
            currentDate ??= DateTime.UtcNow;

            List<int> selectedIds = selectedObj != null ? selectedObj.Select(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s))).ToList() : new List<int>();

            List<T> existingDBRecords = ((IQueryable)_dbContext.GetType().GetProperty(entityName).GetValue(_dbContext)).OfType<T>().Where(ExistingRecords).ToList();

            List<T> newEntitiesList = selectedObj != null ? selectedObj.Where(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s)) == 0).Select(s => s).ToList() : new List<T>();
            //Update the current entity
            for (int i = 0; i < selectedIds.Count; i++)
            {
                if (selectedIds[i] == 0)
                {
                    continue;
                }

                var entity = DbSet.Find(selectedIds[i]);// get the current entity in database
                var newObj = selectedObj.FirstOrDefault(s => Convert.ToInt32(s.GetType().GetProperty("Id").GetValue(s)) == selectedIds[i]);

                if (entity != null && selectedIds[i] != 0)
                {
                    var createdByProp = entity.GetType().GetProperty("CreatedBy");
                    if (createdByProp != null)
                    {
                        var userCreatedId = createdByProp.GetValue(entity);
                        newObj.GetType().GetProperty("CreatedBy").SetValue(newObj, userCreatedId);
                    }

                    var createdateProp = entity.GetType().GetProperty("CreateDate");
                    if (createdateProp != null)
                    {
                        var userCreatedate = createdateProp.GetValue(entity);
                        newObj.GetType().GetProperty("CreateDate").SetValue(newObj, userCreatedate);
                    }

                    //for tracking entity
                    var modifiedDateProp = entity.GetType().GetProperty("ModifiedDate");
                    if (modifiedDateProp != null)
                    {
                        newObj.GetType().GetProperty("ModifiedDate").SetValue(newObj, currentDate);
                    }
                    var modifiedByProp = entity.GetType().GetProperty("ModifiedBy");
                    if (modifiedByProp != null)
                    {
                        newObj.GetType().GetProperty("ModifiedBy").SetValue(newObj, currentUser);
                    }
                    _dbContext.Entry(entity).CurrentValues.SetValues(newObj);
                }
            }

            // Delete the not existing entity in selected list from database
            var deleted = existingDBRecords.Where(x => !selectedIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x)))).ToList();
            foreach (T obj in deleted)
            {
                //Soft Delete with Tracking ModifiedDate and ModifiedBy
                if (isSoftDelete)
                {

                    var isDeletedProp = obj.GetType().GetProperty("IsDeleted");
                    if (isDeletedProp != null)
                    {
                        obj.IsDeleted = true;
                        obj.GetType().GetProperty("IsDeleted").SetValue(obj, true);

                        var modifiedDateProp = obj.GetType().GetProperty("ModifiedDate");
                        if (modifiedDateProp != null)
                        {
                            obj.GetType().GetProperty("ModifiedDate").SetValue(obj, currentDate);
                        }
                        var modifiedByProp = obj.GetType().GetProperty("ModifiedBy");
                        if (modifiedByProp != null)
                        {
                            obj.GetType().GetProperty("ModifiedBy").SetValue(obj, currentUser);
                        }
                    }
                }
                else
                {
                    DbSet.Remove(obj);

                }
            }

            // Add new entity in databsae
            for (int i = 0; i < newEntitiesList.Count; i++)
            {
                var createdByProp = newEntitiesList[i].GetType().GetProperty("CreatedBy");
                if (createdByProp != null)
                {
                    //may produce bug here if current user is null
                    newEntitiesList[i].GetType().GetProperty("CreatedBy").SetValue(newEntitiesList[i], currentUser);
                }

                var createdateProp = newEntitiesList[i].GetType().GetProperty("CreateDate");
                if (createdateProp != null)
                {
                    newEntitiesList[i].GetType().GetProperty("CreateDate").SetValue(newEntitiesList[i], currentDate);
                }
                DbSet.Add(newEntitiesList[i]);
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Deletes the given where. </summary>
        ///
        /// <param name="where">    The where to get. </param>
        ///-------------------------------------------------------------------------------------------------

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = DbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
            {
                DbSet.Remove(obj);
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets by identifier. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The by identifier. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets by identifier. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The by identifier. </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual T GetById(long id)
        {
            return DbSet.Find(id);
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await DbSet.FindAsync(id);
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all. </summary>
        ///
        /// <returns>   all. </returns>
        ///-------------------------------------------------------------------------------------------------

        //public virtual IQueryable<T> GetAll()
        //{
        //    return DbSet;
        //}
        public virtual IQueryable<T> GetAll(bool disableFilter = false)
        {
            if (disableFilter)
                return DisableFilter(nameof(DynamicFilters.IsActive));
            return DbSet;
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where);
        }

        public virtual IQueryable<T> WhereIf(Expression<Func<T, bool>> predicate, bool isValid, bool disableFilter = false)
        {
            return disableFilter ? DisableFilter(nameof(DynamicFilters.IsActive)) : DbSet.WhereIf(predicate, isValid);
        }

        public virtual IQueryable<T> EqualJsonBy(string PropretyName, string PropertyValue, JsonKeyEnum JsonKey)
        {
            return DbSet.EqualJsonBy(PropretyName, PropertyValue, JsonKey);
        }

        public virtual IQueryable<T> EqualJsonBy(List<EqualModel> equalsList)
        {
            return DbSet.EqualJsonBy(equalsList);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public virtual Dictionary<string, string> GetEntitySchema()
        {
            var dictionary = new Dictionary<string, string>();

            var entityType = DbContext.Model.FindEntityType(typeof(T));
            var schema = entityType.GetSchema() ?? entityType.GetViewSchema();
            var table = entityType.GetTableName() ?? entityType.GetViewName();

            dictionary.Add(schema, table);

            return dictionary;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all lists in this collection. </summary>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process all lists in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual IEnumerable<T> GetAllList(bool disableFilter = false)
        {
            return GetAll(disableFilter).ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAllListAsync(bool disableFilter = false)
        {
            return await GetAll(disableFilter).ToListAsync();
        }

        public virtual int Count(bool disableFilter = false)
        {
            return DbSet.Count();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the manies in this collection. </summary>
        ///
        /// <param name="where">    The where to get. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the manies in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where).AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(where).ToListAsync();
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a t using the given where. </summary>
        ///
        /// <param name="where">    The where to get. </param>
        ///
        /// <returns>   A T. </returns>
        ///-------------------------------------------------------------------------------------------------

        public T Get(Expression<Func<T, bool>> where)
        {
            return DbSet.FirstOrDefault<T>(where);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.FirstOrDefaultAsync<T>(where);
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enumerates all include in this collection. </summary>
        ///
        /// <param name="includeProperties">    A variable-length parameters list containing include
        ///                                     properties. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process all include in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public IEnumerable<T> AllInclude(params Expression<Func<T, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties).AsEnumerable();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Finds the includes in this collection. </summary>
        ///
        /// <param name="predicte"> The predicte. </param>
        ///
        /// <param name="predicate">            The predicate. </param>
        /// <param name="includeProperties">    A variable-length parameters list containing include
        ///                                                                      properties. </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the includes in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public IEnumerable<T> FindByInclude(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            return query.Where(predicate).AsEnumerable();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Paged result. </summary>
        ///
        /// <typeparam name="TKey"> Type of the key. </typeparam>
        /// <param name="take">                 The take. </param>
        /// <param name="skip">                 The skip. </param>
        /// <param name="keySelector">          The key selector. </param>
        /// <param name="sortDirection">        (Optional) The sort direction. </param>
        /// <param name="includeProperties">    A variable-length parameters list containing include
        ///                                     properties. </param>
        ///
        /// <returns>   A ListPaged&lt;T&gt; </returns> 
        ///-------------------------------------------------------------------------------------------------

        public ListPaged<T> PagedResult<TKey>(int take,
                                              int skip,
                                              Expression<Func<T, TKey>> keySelector,
                                              string sortDirection = nameof(SortingDirection.ASC),
                                              params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            var count = query.Count();
            ListPaged<T> list = new ListPaged<T>
            {
                ResultCount = count,
                Result = sortDirection == nameof(SortingDirection.DESC) ? query.OrderByDescending(keySelector).Skip(skip).Take(take).ToList()
                                                 : query.OrderBy(keySelector).Skip(skip).Take(take).ToList()
            };
            return list;
        }

        public ListPaged<T> PagedResult<TKey>(int take,
                                              int skip,
                                              Expression<Func<T, TKey>> keySelector,
                                              string sortDirection = nameof(SortingDirection.ASC),
                                              bool disableFilter = false,
                                              params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties, disableFilter);
            var count = query.Count();

            ListPaged<T> list = new ListPaged<T>
            {
                ResultCount = count,
                Result = sortDirection == nameof(SortingDirection.DESC) ? query.OrderByDescending(keySelector).Skip(skip).Take(take).ToList()
                                                 : query.OrderBy(keySelector).Skip(skip).Take(take).ToList()
            };
            return list;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Paged result. </summary>
        ///
        /// <typeparam name="TKey"> Type of the key you want to sort by. </typeparam>
        /// <param name="take">                 The take. </param>
        /// <param name="skip">                 The skip. </param>
        /// <param name="keySelector">          The key selector. </param>
        /// <param name="predicate">            The predicate. </param>
        /// <param name="sortDirection">        (Optional) The sort direction. </param>
        /// <param name="includeProperties">    A variable-length parameters list containing include
        ///                                     properties. </param>
        ///
        /// <returns>   A ListPaged&lt;T&gt; </returns>
        ///-------------------------------------------------------------------------------------------------

        public ListPaged<T> PagedResult<TKey>(int take, int skip, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, string sortDirection = nameof(SortingDirection.ASC), bool disableFilter = false, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties, disableFilter);
            var count = query.Where(predicate).Count();
            ListPaged<T> list = new ListPaged<T>
            {
                ResultCount = count,
                Result = sortDirection == nameof(SortingDirection.DESC) ? query.Where(predicate).OrderByDescending(keySelector).Skip(skip).Take(take).ToList()
                                                 : query.Where(predicate).OrderBy(keySelector).Skip(skip).Take(take).ToList()
            };
            return list;
        }
        public async Task<ListPaged<T>> PagedResultAsync<TKey>(int take, int skip, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, string sortDirection = nameof(SortingDirection.ASC), params Expression<Func<T, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            var count = await query.Where(predicate).CountAsync();
            ListPaged<T> list = new ListPaged<T>
            {
                ResultCount = count,
                Result = sortDirection == nameof(SortingDirection.DESC) ? await query.Where(predicate).OrderByDescending(keySelector).Skip(skip).Take(take).ToListAsync()
                                                 : await query.Where(predicate).OrderBy(keySelector).Skip(skip).Take(take).ToListAsync()
            };
            return list;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all including. </summary>
        ///
        /// <param name="includeProperties">    A variable-length parameters list containing include
        ///                                     properties. </param>
        ///
        /// <returns>   all including. </returns>
        ///-------------------------------------------------------------------------------------------------

        public IQueryable<T> GetAllIncluding(Expression<Func<T, object>>[] includeProperties, bool disableFilter = false)
        {
            IQueryable<T> queryable = disableFilter ? DisableFilter(nameof(DynamicFilters.IsActive)) : DbSet.AsQueryable();
            var x = includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
            return x;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Execute sql statement. </summary>
        ///
        /// <typeparam name="typ">  The type of return. </typeparam>
        /// <param name="queryStatment">    The Statement . </param>
        /// <param name="parameters">       The Parameter which is included in statement. </param>
        ///
        /// <returns>   return the "type" which you pass it. </returns>
        ///-------------------------------------------------------------------------------------------------

        public List<typ> ExecuteSQLQuery<typ>(string queryStatment, Hashtable parameters) where typ : class
        {
            var sqlParam = new List<object>();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (DictionaryEntry entry in parameters)
                {
                    SqlParameter param = new SqlParameter()
                    {
                        ParameterName = entry.Key.ToString(),

                        Value = entry.Value == null ? DBNull.Value : entry.Value,
                    };

                    sqlParam.Add(param);
                }
            }

            return DbContext.Set<typ>().FromSqlRaw(queryStatment, sqlParam.ToArray()).ToList();
            //return DbContext.Database.SqlQuery<typ>(queryStatment, sqlParam.ToArray()).ToList();
        }

        public void ExecuteSQLQuery(string queryStatment, Hashtable parameters)
        {
            var sqlParam = new List<object>();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (DictionaryEntry entry in parameters)
                {
                    SqlParameter param = new SqlParameter()
                    {
                        ParameterName = entry.Key.ToString(),
                        Value = entry.Value
                    };
                    sqlParam.Add(param);
                }
            }

            DbContext.Database.ExecuteSqlRaw(queryStatment, sqlParam.ToArray());
        }

        public void RefreshEntity(T entity)
        {
            _dbContext.Entry<T>(entity).Reload();
        }

        public void RefreshEntity(T entity, params Expression<Func<T, object>>[] navigationProperties)
        {
            RefreshEntity(entity);
            foreach (var navigationProp in navigationProperties)
            {
                _dbContext.Entry<T>(entity).Reference(navigationProp).Load();
            }
        }

        public async Task RefreshEntityReferencesAsync(T entity)
        {
            var references = _dbContext.Entry<T>(entity).References;

            foreach (var reference in references)
            {
                await reference.LoadAsync();
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Execute stored procedure statement and can return multiResult. </summary>
        ///
        /// <param name="storeProcedureName">    The name of stored procedure . </param>
        /// <param name="parameters">       The collection of Parameter for stored procedure. </param>
        /// <param name="lstDtoType">       The list of type for each result which is returned from stored procedure. </param>
        /// <returns>   return the list of IEnumerable. </returns>
        ///-------------------------------------------------------------------------------------------------

        public List<IEnumerable> ExecuteStoredProcedure(string storeProcedureName, Hashtable parameters, List<Type> lstDtoType)
        {
            var results = DbContext.MultipleResults(storeProcedureName).Execute(lstDtoType, parameters);
            return results;
        }
    }

    public static class DynamicFiltersExtensions
    {

    }
}

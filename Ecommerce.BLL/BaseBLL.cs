using AutoMapper;
using Ecommerce.BLL.DBQueriesDto;
using Ecommerce.BLL.FilterBuilder;
using Ecommerce.Core.Helpers.JsonLanguages;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ecommerce.BLL
{
    public class FilterValue
    {
        public string Value { get; set; }
        public Operation Operator { get; set; } = Operation.Contains;
        public FilterStatementConnector Connector { get; set; } = FilterStatementConnector.And;
    }
    public abstract class BaseBLL
    {

        private readonly IMapper _mapper;
        public BaseBLL(IMapper mapper)
        {
            _mapper = mapper;
        }


        //internal virtual ResponseBuilder<T> InitializeResponse<T>( )
        //{
        //    var responseBuilder = new ResponseBuilder<T>();

        //    return responseBuilder;
        //}
        // protected virtual ITResponse<T> CreateResponse<T>( )
        // {
        //     var responseBuilder = new ResponseBuilder<T>();

        //     return responseBuilder.Build();
        // }
        // protected virtual ITResponse<T> CreateDxResponse<T>( T data, MessageCodes? messageCode = null,
        //string message = "", List<ValidationFailure> inputValidations = null, List<TErrorField> businessValidations = null, Exception exception = null )
        // {
        //     var output = new ResponseBuilder<T>().Build();// new TResponse<T>();
        //     var responseBuilder = new ResponseBuilder<T>();// new TResponse<T>();
        //     responseBuilder
        //         .WithData(data)
        //         .WithErrors(businessValidations)
        //         .WithException(exception);

        //     return responseBuilder.Build();
        // }




        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Entity has references. </summary>
        ///
        /// <param name="id">               The identifier. </param>
        /// <param name="schemaName">       Name of the schema. </param>
        /// <param name="parentTableName">  Name of the parent table. </param>
        /// <param name="repository">       The entity repository. </param>
        /// <param name="excludedTables">   (Optional) The excluded tables comma seperated and must be
        ///                                 quoted with square brackets ex.:[SchemaName].[TableName],
        ///                                 [SchemaName].[TableName]. </param>
        ///
        /// <returns>   A CheckDto. </returns>
        ///-------------------------------------------------------------------------------------------------

        protected virtual CheckDto EntityHasReferences<T>(long id, string schemaName, string parentTableName, IRepository<T> repository, string excludedTables = "") where T : class
        {
            Hashtable hashParam = new Hashtable
            {
                ["RefId"] = id,
                ["SchemaName"] = schemaName,
                ["ParentTableName"] = parentTableName,
                ["ExcludedTables"] = excludedTables
            };
            var result = repository.ExecuteSQLQuery<CheckDto>("EXEC CheckIfDependenciesExist @RefId, @SchemaName, @ParentTableName, @ExcludedTables", hashParam).FirstOrDefault();

            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Entity has references. </summary>
        ///
        /// <param name="id">               The identifier. </param>
        /// <param name="repository">       The entity repository. </param>
        /// <param name="excludedTables">   (Optional) The excluded tables comma seperated and must be
        ///                                 quoted with square brackets ex.:[SchemaName].[TableName],
        ///                                 [SchemaName].[TableName]. </param>
        ///
        /// <returns>   A CheckDto. </returns>
        ///-------------------------------------------------------------------------------------------------
        protected virtual CheckDto EntityHasReferences<T>(long id, IRepository<T> repository, string excludedTables = "") where T : class
        {
            var dicOfSchemaAndTable = repository.GetEntitySchema();
            Hashtable hashParam = new Hashtable
            {
                ["RefId"] = id,
                ["SchemaName"] = dicOfSchemaAndTable.FirstOrDefault().Key,
                ["ParentTableName"] = dicOfSchemaAndTable.FirstOrDefault().Value,
                ["ExcludedTables"] = excludedTables
            };
            List<Type> types = new List<Type>();
            types.Add(new CheckDto().GetType());
            types.Add(new DTO.Lookups.GetCountryOutputDto().GetType());
            var proc = repository.ExecuteStoredProcedure("CheckIfDependenciesExist", hashParam, types).FirstOrDefault();

            var result = proc.Cast<CheckDto>().FirstOrDefault();
            // var result = repository.ExecuteSQLQuery<CheckDto>("EXEC CheckIfDependenciesExist @RefId, @SchemaName, @ParentTableName, @ExcludedTables", hashParam).FirstOrDefault();

            return result;
        }


        protected JsonLanguageModel GetJsonLanguageModelOrNull(string jsonString)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    return JsonConvert.DeserializeObject<JsonLanguageModel>(jsonString);

                }

                else
                    return null;
            }

            catch (Exception ex)
            {
                return null;
            }
        }




        //protected virtual PagedResultDto<T1> GetPagedList<T1, T2, TKey>( FilteredResultRequestDto pagedDto,
        //                                                             IRepository<T2> repository,
        //                                                             Expression<Func<T2, TKey>> orderExpression,
        //                                                             Expression<Func<T2, bool>> searchExpression,
        //                                                             string sortDirection = nameof(SortingDirection.ASC),
        //                                                             bool disableFilter = false,

        //                                                             params Expression<Func<T2, object>> [] includeProperties ) where T1 : class where T2 : class
        //{
        //    Dictionary<string, FilterValue> filters = null;
        //    if (pagedDto.Filter != null)
        //        filters = JsonConvert.DeserializeObject<Dictionary<string, FilterValue>>(pagedDto.Filter);

        //    //List<string> excludedColums = new List<string>() { "Taxes" };
        //    //if (excludedColums != null & excludedColums.Count() > 0)
        //    //{
        //    //    foreach (var column in excludedColums)
        //    //    {
        //    //        var taxFilter = filters.FirstOrDefault(t => t.Key.Contains(column));
        //    //        if (filters.Any(t => t.Key.Contains(column)))
        //    //            filters.Remove(taxFilter.Key);
        //    //    }

        //    //}
        //    PagedResultDto<T1> result = new PagedResultDto<T1>();

        //    try
        //    {


        //        var filterBuilder = new FilterBuilder.Generic.Filter<T2>();
        //        //01- Get All Case - Without Search terms & No Filters
        //        if (searchExpression == null && (filters == null || filters.Count == 0))
        //        {
        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, sortDirection, disableFilter, includeProperties);
        //            // x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //02- Get All filtered - With Search term & With providing Filters
        //        else if (searchExpression != null && filters?.Count > 0)
        //        {
        //            foreach (var item in filters)
        //            {
        //                if (!string.IsNullOrWhiteSpace(item.Key) && item.Key != "undefined" && item.Value != null)
        //                    filterBuilder.By(item.Key.Replace("-R2-", ""), item.Value.Operator, item.Value.Value);

        //            }

        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression().And(searchExpression), sortDirection, disableFilter, includeProperties);
        //            //x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //03- Get All filtered - Without Search term but With providing Filters
        //        else if (searchExpression == null && filters.Count > 0)
        //        {
        //            foreach (var item in filters)
        //            {
        //                if (!string.IsNullOrWhiteSpace(item.Key) && item.Key != "undefined" && item.Value != null)
        //                    //if (!string.IsNullOrWhiteSpace(item.Value.Value))
        //                    filterBuilder.By(item.Key.Replace("-R2-", ""), item.Value.Operator, item.Value.Value);
        //            }

        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression(), sortDirection, disableFilter, includeProperties);
        //            //x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //04- Get Specific entity - With Search term but Without providing any Filters
        //        else
        //        {
        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, searchExpression, sortDirection, disableFilter, includeProperties); //x => x.Id.ToString(), queryExpression
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return result;

        //    }
        //}

        protected virtual PagedResultDto<T1> GetPagedList<T1, T2, TKey>(FilteredResultRequestDto pagedDto,
                                                              IRepository<T2> repository,
                                                              Expression<Func<T2, TKey>> orderExpression,
                                                              Expression<Func<T2, bool>> searchExpression,
                                                              string sortDirection = nameof(SortingDirection.ASC),
                                                              bool disableFilter = false,
                                                              List<string> excluededColumns = null,
                                                              params Expression<Func<T2, object>>[] includeProperties
                                                               ) where T1 : class where T2 : class
        {
            PagedResultDto<T1> result = new PagedResultDto<T1>();

            try
            {
                Dictionary<string, FilterValue> filters = null;
                if (pagedDto.Filter != null)
                    filters = JsonConvert.DeserializeObject<Dictionary<string, FilterValue>>(pagedDto.Filter);

                #region Exclude Filters not exist in Entity
                if (excluededColumns != null && excluededColumns.Any() && filters != null && filters.Any())
                {
                    foreach (var column in excluededColumns)
                    {
                        var excludedFilter = filters.FirstOrDefault(t => t.Key.Contains(column));
                        if (filters.Any(t => t.Key.Contains(column)))
                            filters.Remove(excludedFilter.Key);
                    }
                }
                #endregion

                var filterBuilder = new FilterBuilder.Generic.Filter<T2>();
                //01- Get All Case - Without Search terms & No Filters
                if (searchExpression == null && (filters == null || filters.Count == 0))
                {
                    var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, sortDirection, disableFilter, includeProperties);
                    // x => x.Id.ToString()
                    result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items["Lang"] = pagedDto.Lang);
                    result.TotalCount = entityList.ResultCount;
                }
                //02- Get All filtered - With Search term & With providing Filters
                else if (searchExpression != null && filters?.Count > 0)
                {
                    foreach (var item in filters)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Key) && item.Key != "undefined" && item.Value != null)
                            filterBuilder.By(item.Key.Replace("-R2-", ""), item.Value.Operator, item.Value.Value, item.Value.Connector);
                    }

                    var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression().And(searchExpression), sortDirection, disableFilter, includeProperties);
                    //x => x.Id.ToString()
                    result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items["Lang"] = pagedDto.Lang);
                    result.TotalCount = entityList.ResultCount;
                }
                //03- Get All filtered - Without Search term but With providing Filters
                else if (searchExpression == null && filters.Count > 0)
                {
                    foreach (var item in filters)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Key) && item.Key != "undefined" && item.Value != null)
                            filterBuilder.By(item.Key.Replace("-R2-", ""), item.Value.Operator, item.Value.Value, item.Value.Connector);
                    }
                    var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression(), sortDirection, disableFilter, includeProperties);
                    //x => x.Id.ToString()
                    result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items["Lang"] = pagedDto.Lang);
                    result.TotalCount = entityList.ResultCount;
                }
                //04- Get Specific entity - With Search term but Without providing any Filters
                else
                {
                    var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, searchExpression, sortDirection, disableFilter, includeProperties); //x => x.Id.ToString(), queryExpression
                    result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items["Lang"] = pagedDto.Lang);
                    result.TotalCount = entityList.ResultCount;
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;

            }
        }

        protected static Expression<Func<T, object>> GetOrderExpressionNested<T>(string? sortBy = null) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = GetNestedPropertyAccessExpression(parameter, typeof(T), sortBy ?? "Id");

            // Convert the property access to object to handle null values gracefully
            var propertyAccessConverted = Expression.Convert(propertyAccess, typeof(object));
            var orderExpression = Expression.Lambda<Func<T, object>>(propertyAccessConverted, parameter);
            return orderExpression;
        }

        private static Expression GetNestedPropertyAccessExpression(Expression parameter, Type type, string propertyPath)
        {
            string[] properties = propertyPath.Split('.');
            Expression propertyAccess = parameter;

            foreach (string property in properties)
            {
                if (property.EndsWith("()"))
                {
                    // Handle method calls like FirstOrDefault()
                    string methodName = property.Substring(0, property.IndexOf("()"));
                    MethodInfo methodInfo = typeof(Enumerable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 1)
                        .MakeGenericMethod(type.GenericTypeArguments[0]);
                    propertyAccess = Expression.Call(methodInfo, propertyAccess);
                    type = type.GenericTypeArguments[0];
                }
                else
                {
                    PropertyInfo propertyInfo = type.GetProperty(property) ?? throw new ArgumentException($"Property '{property}' does not exist on type '{type.Name}'");
                    propertyAccess = Expression.Property(propertyAccess, propertyInfo);
                    type = propertyInfo.PropertyType;
                }
            }

            return propertyAccess;
        }

        protected static Expression<Func<T, object>> GetOrderExpression<T>(string? sortBy = null) where T : class
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "Id"; // Default to "Id" if no sortBy is specified
            }

            var parameter = Expression.Parameter(typeof(T), "x");

            // Split the sortBy by '.' to handle nested properties
            var propertyNames = sortBy.Split('.');

            Expression propertyAccess = parameter;
            Type currentType = typeof(T);

            foreach (var propertyName in propertyNames)
            {
                if (propertyName.Contains("["))
                {
                    // Handle cases like Products[Name], where Products is a collection
                    var collectionName = propertyName.Substring(0, propertyName.IndexOf('['));
                    var itemPropertyName = propertyName.Substring(propertyName.IndexOf('[') + 1).TrimEnd(']');

                    // Get the collection property
                    var collectionProperty = currentType.GetProperty(collectionName);
                    if (collectionProperty == null)
                    {
                        throw new ArgumentException($"Property {collectionName} not found on {currentType.Name}");
                    }

                    // Get the item type of the collection (e.g., Product)
                    var itemType = collectionProperty.PropertyType.GetGenericArguments()[0];

                    // Build expression for the item property
                    var itemParameter = Expression.Parameter(itemType, "item");
                    var itemProperty = Expression.Property(itemParameter, itemPropertyName);

                    // Select the first item in the collection for sorting purposes
                    var anyMethod = typeof(Enumerable).GetMethods()
                        .First(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1)
                        .MakeGenericMethod(itemType);

                    var firstItemInCollection = Expression.Call(anyMethod, Expression.Property(propertyAccess, collectionProperty));
                    propertyAccess = Expression.Property(firstItemInCollection, itemPropertyName);

                    currentType = itemType;
                }
                else
                {
                    // Handle simple property access (including nested properties)
                    var property = currentType.GetProperty(propertyName);
                    if (property == null)
                    {
                        throw new ArgumentException($"Property {propertyName} not found on {currentType.Name}");
                    }

                    propertyAccess = Expression.Property(propertyAccess, property);
                    currentType = property.PropertyType;
                }
            }

            // If the property is of value type, convert it to object to handle nulls
            if (currentType.IsValueType)
            {
                propertyAccess = Expression.Convert(propertyAccess, typeof(object));
            }

            return Expression.Lambda<Func<T, object>>(propertyAccess, parameter);
        }

        //protected static Expression<Func<T, object>> GetOrderExpression<T>(string? sortBy = null) where T : class
        //{
        //    var property = typeof(T).GetProperty(sortBy ?? "Id");
        //    if (property == null)
        //    {
        //        property = typeof(T).GetProperty("Id");
        //    }

        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var propertyAccess = Expression.Property(parameter, property);

        //    // Check if the property is of a value type (e.g., int, decimal, etc.)
        //    if (property.PropertyType.IsValueType)
        //    {
        //        // If it's a value type, convert it to its nullable counterpart to handle null values
        //        var convertedProperty = Expression.Convert(propertyAccess, typeof(object));
        //        var nullableProperty = Expression.Convert(convertedProperty, typeof(object));
        //        var orderExpression = Expression.Lambda<Func<T, object>>(nullableProperty, parameter);
        //        return orderExpression;
        //    }
        //    else
        //    {
        //        // If it's not a value type, directly convert it to object
        //        var propertyAccessConverted = Expression.Convert(propertyAccess, typeof(object));
        //        var orderExpression = Expression.Lambda<Func<T, object>>(propertyAccessConverted, parameter);
        //        return orderExpression;
        //    }
        //}
        protected static Func<IQueryable<T>, IOrderedQueryable<T>> GetOrderExpressionQuerabl<T>(string? sortBy = null, SortingDirection sortingDirection = SortingDirection.ASC) where T : class
        {
            var property = typeof(T).GetProperty(sortBy ?? "Id");
            if (property == null)
            {
                property = typeof(T).GetProperty("Id");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);

            var orderExpression = Expression.Lambda(propertyAccess, parameter);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(orderExpression.Body, typeof(object)), parameter);

            return (IQueryable<T> source) => sortingDirection == SortingDirection.DESC ? source.OrderByDescending(lambda) : source.OrderBy(lambda);
        }


        protected string getFileContentType(string format) => format switch
        {
            "pdf" => "application/pdf",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "rtf" => "",
            "mht" => "",
            "html" => "text/html",
            "txt" => "text/plain",
            "csv" => "text/plain",
            "png" => "image/png",
            "jpg" => "image/jpg",
        };
        #region Comment
        /////-------------------------------------------------------------------------------------------------
        ///// <summary>   Gets paged list&lt;OutputDto&gt; </summary>
        /////
        ///// <typeparam name="T1">   Output Dto. </typeparam>
        ///// <typeparam name="T2">   Entity. </typeparam>
        ///// <typeparam name="TKey"> Type of the key. </typeparam>
        ///// <param name="pagedDto">         The paged dto of type FilteredResultRequestDto. </param>
        ///// <param name="repository">       The entity repository. </param>
        ///// <param name="orderExpression">  The column to order by in lambda expression. </param>
        ///// <param name="searchExpression"> The columns to search by in lambda expression. </param>
        ///// <param name="sortDirection">    (Optional) The sort direction. </param>
        /////
        ///// <returns>   The paged list&lt;OutputDto&gt; </returns>
        /////-------------------------------------------------------------------------------------------------
        //protected virtual PagedResultDto<T1> GetPagedList<T1, T2, TKey>( FilteredResultRequestDto pagedDto,
        //                                                                IRepository<T2> repository,
        //                                                                Expression<Func<T2, TKey>> orderExpression,
        //                                                                Expression<Func<T2, bool>> searchExpression,
        //                                                                string sortDirection = nameof(SortingDirection.ASC) ) where T1 : class where T2 : class
        //{
        //    PagedResultDto<T1> result = new PagedResultDto<T1>();

        //    try
        //    {

        //        Dictionary<string, FilterValue> filters = null;
        //        if (!string.IsNullOrWhiteSpace(pagedDto.Filter))
        //            filters = JsonConvert.DeserializeObject<Dictionary<string, FilterValue>>(pagedDto.Filter);



        //        var filterBuilder = new FilterBuilder.Generic.Filter<T2>();
        //        //01- Get All Case - Without Search terms & No Filters
        //        if (searchExpression == null && (filters == null || filters.Count == 0))
        //        {
        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, sortDirection); // x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //02- Get All filtered - With Search term & With providing Filters
        //        else if (searchExpression != null && !string.IsNullOrWhiteSpace(pagedDto.Filter) && filters?.Count > 0)
        //        {
        //            foreach (var item in filters)
        //            {
        //                filterBuilder.By(item.Key, item.Value.Operator, item.Value.Value);
        //            }

        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression().And(searchExpression), sortDirection);//x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //03- Get All filtered - Without Search term but With providing Filters
        //        else if (searchExpression == null && !string.IsNullOrWhiteSpace(pagedDto.Filter) && filters.Count > 0)
        //        {
        //            foreach (var item in filters)
        //            {
        //                if (!string.IsNullOrWhiteSpace(item.Value.Value))
        //                    filterBuilder.By(item.Key, item.Value.Operator, item.Value.Value);
        //            }

        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, filterBuilder.BuildExpression(), sortDirection); //x => x.Id.ToString()
        //            result.Items = _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        //04- Get Specific entity - With Search term but Without providing any Filters
        //        else
        //        {
        //            var entityList = repository.PagedResult(pagedDto.MaxResultCount, pagedDto.SkipCount, orderExpression, searchExpression, sortDirection); //x => x.Id.ToString(), queryExpression
        //            result.Items = !string.IsNullOrWhiteSpace(pagedDto.Lang) ?
        //                           _mapper.Map<List<T1>>(entityList.Result, opts => opts.Items ["Lang"] = pagedDto.Lang) :
        //                          _mapper.Map<List<T1>>(entityList.Result);
        //            result.TotalCount = entityList.ResultCount;
        //        }
        //        return result;
        //    }
        //    //catch serialization issues or any raised exception
        //    catch (Exception ex)
        //    {
        //        return result;
        //    }
        //}
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   search in Entity for Json Values exists (Defaul,ar) </summary>
        ///
        /// <typeparam name="T">   Entity Type. </typeparam>
        /// <param name="repository">       The entity repository. </param>
        /// <param name="searchExpression"> The columns to search by in lambda expression. </param>
        /// <param name="jsonString">     the json string with can be deserialize to type JsonLanguageModel with(Default,ar). </param>
        /// <param name="excludedId">    (Optional) Id field to be excluded from search (in create = 0 ,in update = current entityId ) </param>
        /// <param name="disableFilter">    (Optional) Disable IsActive DynamicFilter  </param>

        ///
        /// <returns>   The paged list&lt;OutputDto&gt; </returns>
        ///-------------------------------------------------------------------------------------------------
        //protected bool IsJsonValueExist<T>( IRepository<T> repository, Expression<Func<T, bool>> searchExpression, string jsonString,string fieldName ,bool disableFilter = true, int? excludedId = null ) where T : class
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(jsonString))
        //        {
        //            var name = JsonConvert.DeserializeObject<JsonLanguageModel>(jsonString);
        //            PropertyInfo _nameProperty = typeof(T).GetProperty(fieldName);
        //            var exists = repository.Where(z=> SearchExact(repository, name.Default, i => Json.Value("Name", "$.default")));

        //                                 //(Json.Value("Name", "$.default") == name.Default));
        //            //var exists = repository.WhereIf(searchExpression, excludedId.HasValue, disableFilter).Any(s =>
        //            //                     (Json.Value(jsonString, "$.default") == name.Default || Json.Value("s.Name", "$.Default") == name.Default)
        //            //                  || (Json.Value(jsonString, "$.ar") == name.Ar || Json.Value("s.Name", "$.Ar") == name.Ar));
        //            return exists;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //public IQueryable<T> SearchExact<T>( IRepository<T> repository, string keyword,
        //                           Expression<Func<T, string>> getNameExpression ) where T : class
        //{
        //    var param = System.Linq.Expressions.Expression.Parameter(typeof(T), "s");
        //    return repository.Where(
        //                System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
        //                  System.Linq.Expressions.Expression.Equal(
        //                    System.Linq.Expressions.Expression.Invoke(
        //                      System.Linq.Expressions.Expression.Constant(getNameExpression),
        //                      param),
        //                    System.Linq.Expressions.Expression.Constant(keyword)//, param
        //                  )));
        //}

        //protected bool IsJsonValueExist<T>( IRepository<T> repository, Expression<Func<T, bool>> searchExpression, Expression<Func<T, bool>> fieldExpression, string jsonString, bool disableFilter = true, int? excludedId = null ) where T : class
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(jsonString))
        //        {
        //            var name = JsonConvert.DeserializeObject<JsonLanguageModel>(jsonString);
        //            var exists = repository.WhereIf(searchExpression, excludedId.HasValue, disableFilter).Where(s =>
        //            Json.Value(fieldExpression + "", "$.default") == name.Default).Any();
        //            //  Json.GetExpressionValue<T>(fieldExpression, "$.default", name.Default));
        //            //   (Json.Value("s.Name", "$.default") == name.Default || Json.Value("s.Name", "$.Default") == name.Default)
        //            //|| (Json.Value("s.Name", "$.ar") == name.Ar || Json.Value("s.Name", "$.Ar") == name.Ar));
        //            //var exists = repository.WhereIf(searchExpression, excludedId.HasValue, disableFilter).Any(s =>
        //            //                     (Json.Value(jsonString, "$.default") == name.Default || Json.Value("s.Name", "$.Default") == name.Default)
        //            //                  || (Json.Value(jsonString, "$.ar") == name.Ar || Json.Value("s.Name", "$.Ar") == name.Ar));
        //            return exists;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //protected bool IsNameAlreadyExist<T>( IRepository<T> repository,JsonLanguageModel name, int? Id ) where T : class
        //{
        //    return repository.WhereIf(pl => pl.Id != Id, Id.HasValue).Any(s => Json.Value(s.Name, "$.default") == name.Default || Json.Value(s.Name, "$.ar") == name.Ar);
        //}
        //       [
        //"createDate",
        //">=",
        //"2022-03-27T10:48:00.000"  
        //],  
        //"and",  
        //[
        //"createDate",
        //"<",
        //"2022-03-27T10:49:00.000"  
        //]
        //       public class ListWithDuplicates : List<KeyValuePair<string, FilterValue>>
        //       {
        //           public void Add( string key, FilterValue value )
        //           {
        //               var element = new KeyValuePair<string, FilterValue>(key, value);
        //               this.Add(element);
        //           }

        //       }



        #endregion
    }
}

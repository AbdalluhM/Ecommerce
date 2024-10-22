using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ecommerce.BLL.Customers.Reviews.Customers;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Customers.WishlistApplication;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Customer.Wishlist.WihlistApplicationValidator;

namespace Ecommerce.BLL.Customers.Wishlist
{
    public class WishlistApplicationBLL : BaseBLL, IWishlistApplicationBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<WishListApplication> _wishListAplicationRepository;
        IRepository<Application> _applicationRepository;
        ICustomerReviewBLL _customerReviewBLL;


        #endregion

        #region Constructor
        public WishlistApplicationBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<WishListApplication> WihlistAplicationRepository, IRepository<Application> applicationRepository, ICustomerReviewBLL customerReviewBLL) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _wishListAplicationRepository = WihlistAplicationRepository;
            _applicationRepository = applicationRepository;
            _customerReviewBLL = customerReviewBLL;
        }

        #endregion

        #region Action
        //
        // Summary:
        //     create new WihlistApplication.
        //
        // Parameters:
        //     CreateWishlistApplicationInputDto:
        //          inputDto.
        // Returns:
        //     GetWishlistApplicationOutputDto:
        //           A Response with errors if find or WihlistApplication if insert successfull.
        public IResponse<GetWishlistApplicationOutputDto> Create(CreateWishlistApplicationInputDto inputDto)
        {
            var output = new Response<GetWishlistApplicationOutputDto>();

            try
            {
                //Input Validation Validations
                var validator = new CreateWihlistApplicationInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {

                    return output.CreateResponse(validator.Errors);
                }
                //Business Validation
                // 1- Check if Application in list wishlist
                var wishlistAddOn = _wishListAplicationRepository.GetAll()
                    .FirstOrDefault(x => x.ApplicationId == inputDto.ApplicationId && x.CustomerId == inputDto.CustomerId);
                if (wishlistAddOn != null)
                {
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(Application));
                }
                // 2- Check if Entity Exists
                var entityE = _applicationRepository.GetAll().FirstOrDefault(x => x.Id == inputDto.ApplicationId);
                if (entityE == null)
                    output.AppendError(MessageCodes.NotFound, nameof(Application));

                if (!output.IsSuccess)
                   return output.CreateResponse();

                var mappedInput = _mapper.Map<WishListApplication>(inputDto);
               
                var entity = _wishListAplicationRepository.Add(mappedInput);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetWishlistApplicationOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        //
        // Summary:
        //     Delete AddOn from wishlist list .
        //
        // Parameters:
        //     DeleteWishlistApplicationInputDto:
        //         inputDto.
        // Returns:
        //     A Response with errors if find or true when deleted success.
        public IResponse<bool> Delete(DeleteWishlistApplicationInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = new DeleteWihlistApplicationInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _wishListAplicationRepository.Where(x => x.ApplicationId == inputDto.ApplicationId && x.CustomerId==inputDto.CustomerId).FirstOrDefault();//WishlistApplicationRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(WishListApplication));
                _wishListAplicationRepository.Delete(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        //
        // Summary:
        //     Get List all WishlistApplication.
        //
        // Parameters:
        //     int:
        //         customerId.
        // Returns:
        //     GetWihlistAddOnOutputDto:
        //           A Response with all WishlistApplication.
        public async Task<IResponse<List<GetWishlistApplicationOutputDto>>> GetAllAsync(WishlistApplicationSearchInputDto inputDto)
        {
            var output = new Response<List<GetWishlistApplicationOutputDto>>();
            try
            {
                var query =await _wishListAplicationRepository
                    .WhereIf(x => x.Application.Name.Contains(inputDto.SearchItems), !string.IsNullOrEmpty(inputDto.SearchItems))
                    .Where(x=>x.Application != null && x.CustomerId == inputDto.CustomerId)
                    .ToListAsync();

                var result = _mapper.Map<List<GetWishlistApplicationOutputDto>>(query);
                result.ForEach(async x =>
                {
                    x.Application.Rate = _customerReviewBLL.GetRateAsync(x.ApplicationId, includeStarPercentage: true).GetAwaiter().GetResult();
                    
                    
                });
               ;
                result.OrderBy(x => x.Application?.Price?.NetPrice);
                return output.CreateResponse(result);
                
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        ///// <summary>
        ///// Get Paged List of only Active and not Deleted records
        ///// </summary>
        ///// <param name="pagedDto"></param>
        ///// <returns></returns>
        public async Task<IResponse<PagedResultDto<GetWishlistApplicationOutputDto>>> GetPagedListAsync(CustomerFilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetWishlistApplicationOutputDto>>();

            var result = GetPagedList<GetWishlistApplicationOutputDto, WishListApplication, int>(
                pagedDto: pagedDto,
                repository: _wishListAplicationRepository,
                orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection,
              searchExpression: x => x.CustomerId == pagedDto.CustomerId
                && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || !string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.Application.Name.Contains(pagedDto.SearchTerm)
                      || x.Application.Title.Contains(pagedDto.SearchTerm)
                      || x.Application.ShortDescription.Contains(pagedDto.SearchTerm))));
            result.Items.ToList().ForEach(x =>
            {
                x.Application.Rate =  _customerReviewBLL.GetRateAsync(x.ApplicationId, includeStarPercentage: true).GetAwaiter().GetResult();
            });
            result.Items.OrderBy(x => x.Application?.Price?.NetPrice);

            return output.CreateResponse(result);
        }
        #endregion



    }
}

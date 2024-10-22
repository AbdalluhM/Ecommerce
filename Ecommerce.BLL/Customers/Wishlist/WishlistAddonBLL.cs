using AutoMapper;
using Ecommerce.BLL.Applications;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Applications;
using Ecommerce.DTO.Customers.Wishlist;
using Ecommerce.DTO.Files;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Customer.Wishlist.WihlistAddOnValidator;

namespace Ecommerce.BLL.Customers.Wishlist
{
    public class WishlistAddonBLL:BaseBLL,IWishlistAddonBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<WishListAddOn> _wishListAddOnRepository;
        IRepository<AddOn> _addOnRepository;

        #endregion

        #region Constructor
        public WishlistAddonBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<WishListAddOn> WishListAddOnRepository ,IRepository<AddOn> addOnRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _wishListAddOnRepository = WishListAddOnRepository;
            _addOnRepository = addOnRepository;
        }

        #endregion

        #region Action
        //
        // Summary:
        //     create new WihlistAddOn.
        //
        // Parameters:
        //     CreateWishlistAddOnInputDto:
        //          inputDto.
        // Returns:
        //     GetWihlistAddOnOutputDto:
        //            A Response with errors if find or WihlistAddOn if insert successfull.
        public IResponse<GetWihlistAddOnOutputDto> Create(CreateWishlistAddOnInputDto inputDto)
        {
            var output = new Response<GetWihlistAddOnOutputDto>();

            try
            {
                //Input Validation Validations
                var validator = new CreateWihlistAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {

                    return output.CreateResponse(validator.Errors);
                }

                //Business Validation
                // 1- Check if AddOn in list wishlist
                var wishlistAddOn = _wishListAddOnRepository .GetAll()
                    .FirstOrDefault(x => x.AddOnId == inputDto.AddOnId && x.CustomerId == inputDto.CustomerId);
                if (wishlistAddOn != null)
                {
                    return output.CreateResponse(MessageCodes.AlreadyExists, nameof(AddOn));
                }
                var entityE = _addOnRepository.GetAll().FirstOrDefault(x => x.Id == inputDto.AddOnId);

                // 2- Check if Entity Exists
                if (entityE == null)
                    output.AppendError(MessageCodes.NotFound, nameof(AddOn));

                if (!output.IsSuccess)
                   return output.CreateResponse();

                var mappedInput = _mapper.Map<WishListAddOn>(inputDto);
                var entity = _wishListAddOnRepository.Add(mappedInput);

                _unitOfWork.Commit();

                var result = _mapper.Map<GetWihlistAddOnOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);

            }
        }
        //
        // Summary:
        //     Delete Application from wishlist list .
        //
        // Parameters:
        //     DeleteWishlistAddOnInputDto:
        //         inputDto.
        // Returns:
        //     A Response with errors if find or true when deleted success.
        public IResponse<bool> Delete(DeleteWishlistAddOnInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = new DeleteWihlistAddOnInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _wishListAddOnRepository.Where(x => x.AddOnId == inputDto.AddOnId && x.CustomerId==inputDto.CustomerId).FirstOrDefault();//WishlistAddOnRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(WishListAddOn));
                _wishListAddOnRepository.Delete(entity);
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
        //     Get List all wishlistAddOn.
        //
        // Parameters:
        //     int:
        //         customerId.
        // Returns:
        //     GetWihlistAddOnOutputDto:
        //           A Response with all wishlistAddon.
        public async Task<IResponse<List<GetWihlistAddOnOutputDto>>> GetAllAsync(WishlistAddOnSearchInputDto inputDto)
        {
            var output = new Response<List<GetWihlistAddOnOutputDto>>();
            try
            {
                var query = _wishListAddOnRepository
                     .WhereIf(x => x.AddOn.Name.Contains(inputDto.SearchItems), !string.IsNullOrEmpty(inputDto.SearchItems))
                     .Where(x => x.CustomerId == inputDto.CustomerId).ToList();
                var result = _mapper.Map<List<GetWihlistAddOnOutputDto>>(query);
                result.OrderBy(x => x.AddOn?.Price?.NetPrice);

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
        ///

       public async Task<IResponse<PagedResultDto<GetWihlistAddOnOutputDto>>> GetPagedListAsync(CustomerFilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetWihlistAddOnOutputDto>>();

            var result = GetPagedList<GetWihlistAddOnOutputDto, WishListAddOn, int>(
                pagedDto: pagedDto,
                repository: _wishListAddOnRepository, 
                orderExpression: x => x.Id,
                sortDirection: pagedDto.SortingDirection,
                searchExpression: x => x.CustomerId == pagedDto.CustomerId 
                && (string.IsNullOrWhiteSpace(pagedDto.SearchTerm)
                      || !string.IsNullOrWhiteSpace(pagedDto.SearchTerm) &&
                         (x.AddOn.Name.Contains(pagedDto.SearchTerm)
                      || x.AddOn.Title.Contains(pagedDto.SearchTerm)
                      || x.AddOn.ShortDescription.Contains(pagedDto.SearchTerm))));

            result.Items.OrderBy(x => x.AddOn?.Price?.NetPrice);

            return output.CreateResponse(result);
        }
        #endregion
    }
}

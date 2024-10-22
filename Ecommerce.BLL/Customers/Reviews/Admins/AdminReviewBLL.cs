using AutoMapper;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Reviews;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Review.Admins;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.BLL.Validation.Customer.Review.CustomerReviewValidator;

namespace Ecommerce.BLL.Customers.Reviews.Admins
{
    public class AdminReviewBLL : BaseBLL, IAdminReviewBLL
    {
        #region Fields

        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<CustomerReview> _adminReviewRepository;
        IRepository<Application> _applicationRepository;
        #endregion

        #region Constructor
        public AdminReviewBLL(IMapper mapper, IUnitOfWork unitOfWork, IRepository<CustomerReview> customerReviewRepository, IRepository<Application> applicationRepository) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _adminReviewRepository = customerReviewRepository;
            _applicationRepository = applicationRepository;
        }
        #endregion

        #region Action
        //
        // Summary:
        //     Get pagedlist of customer review .
        //
        // Parameters:
        //     FilteredResultRequestDto:
        //        pagedDto.
        // Returns:
        //     A Response with all customerReview that pending by filter.
        public async Task<IResponse<PagedResultDto<GetCustomerReviewOutputDto>>> GetPagedListAsync(FilteredResultRequestDto pagedDto)
        {
            var output = new Response<PagedResultDto<GetCustomerReviewOutputDto>>();

            var result = GetPagedList<GetCustomerReviewOutputDto, CustomerReview, int>(
                pagedDto: pagedDto,
                repository: _adminReviewRepository,
                orderExpression: x => x.Id,
                searchExpression: x => x.StatusId == (int)ReviewStatusEnum.Pending,
                sortDirection: pagedDto.SortingDirection,
                      disableFilter: true);
            return output.CreateResponse(result);
        }
        //
        // Summary:
        //     submit review.
        //
        // Parameters:
        //     SubmitCustomerReviewInputDto:
        //          inputDto.
        // Returns:
        //     GetCustomerReviewOutputDto:
        //            A Response with errors if find or Entity i need submited.
        public async Task<IResponse<GetCustomerReviewOutputDto>> SubmitAsync(SubmitCustomerReviewInputDto inputDto)
        {
            var output = new Response<GetCustomerReviewOutputDto>();

            try
            {
                //Input Validation
                var validator = new SubmitCustomerReviewInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }


                //Business Validations

                var entity = _adminReviewRepository.DisableFilter(nameof(DynamicFilters.IsActive))
                    .FirstOrDefault(x => x.Id == inputDto.Id);//_CustomerReviewRepository.GetById(inputDto.Id);

                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerReview));
                //Business
                // Update status review
                inputDto.StatusId = (int)ReviewStatusEnum.Confirmed;
                // to do updated by 
                entity = _mapper.Map(inputDto, entity);
                //update Entity
                entity = _adminReviewRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetCustomerReviewOutputDto>(entity);
                return output.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        //
        // Summary:
        //     Delete customer review.
        //
        // Parameters:
        //     DeleteCustomerReviewInputDto:
        //         inputDto.
        // Returns:
        //     A Response with errors if find or true if customerReview deleted successful.
        public IResponse<bool> Delete(DeleteCustomerReviewInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation
                var validator = new DeleteCustomerReviewInputDtoValidator().Validate(inputDto);
                if (!validator.IsValid)
                {
                    return output.CreateResponse(validator.Errors);
                }

                var entity = _adminReviewRepository.DisableFilter(nameof(DynamicFilters.IsActive)).FirstOrDefault(x => x.Id == inputDto.Id);//CustomerReviewRepository.GetById(inputDto.Id);
                //Business Validation
                // 1- Check if Entity Exists
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerReview));
                _adminReviewRepository.Delete(entity);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        #endregion

    }
}

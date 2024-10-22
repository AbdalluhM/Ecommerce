using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.Customer.Review.Customers;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Reviews;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Review.Customers;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Roles.RoleDto;

namespace Ecommerce.BLL.Customers.Reviews.Customers
{
    public class CustomerReviewBLL : BaseBLL, ICustomerReviewBLL
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CustomerReview> _CustomerReviewRepository;

        public CustomerReviewBLL(IMapper mapper,
                                 IUnitOfWork unitOfWork,
                                 IRepository<CustomerReview> customerReviewRepository)
                : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _CustomerReviewRepository = customerReviewRepository;
        }

        public async Task<IResponse<int>> MakeNewReviewAsync(NewReviewDto newReviewDto)
        {
            var response = new Response<int>();

            try
            {
                var validation = await new NewReviewDtoValidator().ValidateAsync(newReviewDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var isUserHasReviewBefore = await _CustomerReviewRepository
                                                        .AnyAsync(r => r.CustomerId == newReviewDto.CustomerId &&
                                                                       r.ApplicationId == newReviewDto.ApplicationId);

                if (isUserHasReviewBefore)
                {
                    response.CreateResponse(MessageCodes.AlreadyExists, nameof(CustomerReview));
                    return response;
                }

                var newReview = _mapper.Map<CustomerReview>(newReviewDto);

                await _CustomerReviewRepository.AddAsync(newReview);

                _unitOfWork.Commit();

                response.CreateResponse(newReview.Id);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
                return response;
            }

            return response;
        }

        public async Task<IResponse<int>> UpdateReviewAsync(EditReviewDto editReviewDto)
        {
            var response = new Response<int>();

            try
            {
                var validation = await new EditReviewDtoValidator().ValidateAsync(editReviewDto);

                if (!validation.IsValid)
                {
                    response.CreateResponse(validation.Errors);
                    return response;
                }

                var oldReview = await _CustomerReviewRepository.GetAsync(r => r.Id == editReviewDto.Id);

                if (oldReview is null)
                {
                    response.CreateResponse(MessageCodes.NotFound, nameof(CustomerReview));
                    return response;
                }

                // delete old review.
                _CustomerReviewRepository.Delete(oldReview);

                // make new review.
                var newReview = _mapper.Map<CustomerReview>(editReviewDto);
                newReview.ApplicationId = oldReview.ApplicationId;
                newReview.CustomerId = oldReview.CustomerId;

                await _CustomerReviewRepository.AddAsync(newReview);

                _unitOfWork.Commit();

                response.CreateResponse(newReview.Id);
            }
            catch (Exception ex)
            {
                response.CreateResponse(ex);
                return response;
            }

            return response;
        }

        public async Task<RateDto> GetRateAsync(int applicationId, bool includeStarPercentage = false)
        {
            var review = await _CustomerReviewRepository.GetManyAsync(r => r.Application != null
                                                                        && r.ApplicationId == applicationId
                                                                        && r.StatusId == (int)ReviewStatusEnum.Confirmed);

            if (review is null || !review.Any())
            {
                return null;
            }

            var reviewDto = new RateDto();

            var rates = review.Sum(r => r.Rate);
            var totalCount = review.Count();

            var averageRate = rates / totalCount;

            reviewDto.AverageRate = averageRate;
            reviewDto.Count = totalCount;

            // stars percentage.
            if (includeStarPercentage)
            {
                reviewDto.Stars = review.GroupBy(r => Math.Ceiling(r.Rate))
                                        .Select(r => new ReviewStarDto
                                        {
                                            Star = (byte)r.Key,
                                            Percentage = Convert.ToDecimal(r.Count()) / Convert.ToDecimal(totalCount),
                                        });
            }

            return reviewDto;
        }

        public async Task<IResponse<CustomerReviewDto>> GetCustomerReviewAsync(int applicationId, int customerId)
        {
            var response = new Response<CustomerReviewDto>();

            var review = await _CustomerReviewRepository.GetAsync(r => r.ApplicationId == applicationId && r.CustomerId == customerId);

            var reviewDto = _mapper.Map<CustomerReviewDto>(review);

            return response.CreateResponse(reviewDto);
        }

        public IResponse<PagedResultDto<ReviewDto>> GetApplicationReviewsPagedList(FilteredResultRequestDto filterDto, int appId, int customerId)
        {
            var response = new Response<PagedResultDto<ReviewDto>>();

            var priceLevelsDto = GetPagedList<ReviewDto, CustomerReview, int>(pagedDto: filterDto,
                                                                              _CustomerReviewRepository,
                                                                              orderExpression: r => r.Id,
                                                                              searchExpression: r => r.ApplicationId == appId &&
                                                                                                     r.CustomerId != customerId &&
                                                                                                     r.StatusId == (int)ReviewStatusEnum.Confirmed,
                                                                              sortDirection: filterDto.SortingDirection,
                                                                              disableFilter: true);

            return response.CreateResponse(priceLevelsDto);
        }
    }
}
using AutoMapper;

using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Paging;
using Ecommerce.Repositroy.Base;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Ecommerce.DTO.Customers.HomePage.HomePageDto;
using Version = Ecommerce.Core.Entities.Version;

namespace Ecommerce.BLL.Customers.HomePage
{
    public class HomePageBLL : BaseBLL, IHomePageBLL
    {
        #region Fields
        IMapper _mapper;
        IUnitOfWork _unitOfWork;
        IRepository<Application> _applicationRepository;
        IRepository<Tag> _tagRepository;
        IRepository<Version> _versionRepository;
        IRepository<ApplicationTag> _applicationTagRepository;
        IRepository<VersionSubscription> _versionSubScriptionRepository;

        #endregion
        #region Constructor
        public HomePageBLL(IMapper mapper,
                                        IUnitOfWork unitOfWork,
                                        IRepository<Application> applicationRepository,
                                        IRepository<Tag> tagRepository,
                                        IRepository<ApplicationTag> applicationTagRepository,
                                        IRepository<VersionSubscription> versionSubScriptionRepository,
                                        IRepository<Core.Entities.Version> versionRepository)
                                        : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _applicationRepository = applicationRepository;
            _applicationTagRepository = applicationTagRepository;
            _tagRepository = tagRepository;
            _versionSubScriptionRepository = versionSubScriptionRepository;
            _versionRepository = versionRepository;
        }
        #endregion
        public async Task<IResponse<PagedResultDto<GetCustomerAppTagsOrHighlightedOutputDto>>> GetCustomerAppTagsoRHighlighted( FilterCustomerAppTagsInputDto pagedDto )
        {
            var output = new Response<PagedResultDto<GetCustomerAppTagsOrHighlightedOutputDto>>();
            try
            {
                var customerAppIds = _versionSubScriptionRepository
                    .Where(x => x.CustomerSubscription.CustomerId == pagedDto.CustomerId && !x.CustomerSubscription.IsAddOn
                    && x.VersionPrice != null && x.VersionPrice.Version != null && x.VersionPrice.Version.Application != null)
                    .Select(vs => vs.VersionPrice.Version.ApplicationId).Distinct().ToList();
                var tags = _applicationTagRepository.Where(x => customerAppIds.Contains(x.ApplicationId) && x.Tag != null).Select(x => x.TagId).ToList();

                //var notpurchasedApplications = _applicationRepository.Where(x => !customerAppIds.Contains(x.Id) && x.Versions.Any(v => v.Application != null && v.IsHighlightedVersion)).ToList();
                var result = GetPagedList<GetCustomerAppTagsOrHighlightedOutputDto, Application, int>(
                 pagedDto: pagedDto,
                 repository: _applicationRepository,
                 orderExpression: x => x.Id,
                 sortDirection: pagedDto.SortingDirection,
                 searchExpression: x =>x.Versions != null && x.Versions.Any(vp => vp.VersionPrices != null
                 && vp.VersionPrices.Any(c=>c.CountryCurrency.CountryId == pagedDto.CountryId))&& !customerAppIds.Contains(x.Id) && 
                 x.ApplicationTags.Any(at => tags.Contains(at.TagId)));

                if (result == null || (result != null && result.Items.Count == 0))
                    result = GetPagedList<GetCustomerAppTagsOrHighlightedOutputDto, Application, int>(
                    pagedDto: pagedDto,
                    repository: _applicationRepository,
                    orderExpression: x => x.Id,
                    sortDirection: pagedDto.SortingDirection,
                    searchExpression: x => x.Versions.Any(x => x.VersionPrices.Count(c => c.CountryCurrency.CountryId == pagedDto.CountryId) > 0)
                    && !customerAppIds.Contains(x.Id)  && x.Versions.Any(v => v.Application != null && v.IsHighlightedVersion));


                return output.CreateResponse(result);

            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }
        }
    }
}

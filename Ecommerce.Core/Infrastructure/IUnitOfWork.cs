using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Ecommerce.Core.Infrastructure
{
    /// <summary>   Interface for unit of work. </summary>
    public interface IUnitOfWork
    {
        /// <summary>   Commits this object. </summary>
        void Commit();

        Task<int> CommitAsync();
    }
}

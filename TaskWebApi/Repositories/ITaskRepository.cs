using TaskWebApi.Models;

namespace TaskWebApi.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Customer>> GetCustomersWithOnlyTvNoDslAsync();
        Task<IEnumerable<Customer>> GetCustomersWithOnlyDslNoTvAsync();
        Task<IEnumerable<(int CustomerIdTv, int CustomerIdDsl, DateTime StartDate)>> GetLinkedCustomersAsync();
        Task<IEnumerable<(TvProduct, TvProduct)>> GetOverlappingTvProductsAsync();
    }
}

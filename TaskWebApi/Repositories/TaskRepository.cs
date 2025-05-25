using Microsoft.EntityFrameworkCore;
using System.Data;
using TaskWebApi.Data;
using TaskWebApi.Models;


namespace TaskWebApi.Repositories
{
    public class TaskRepository:ITaskRepository
    {
        private readonly HysTestTaskDbContext _context;

        public TaskRepository(HysTestTaskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithOnlyTvNoDslAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Customers
                .Where(c =>
                    _context.TvProducts.Any(tv =>
                        tv.CustomerId == c.Id &&
                        tv.StartDate < today &&
                        (tv.EndDate == null || tv.EndDate > today)
                    ) &&
                    !_context.DslProducts.Any(dsl =>
                        dsl.CustomerId == c.Id &&
                        dsl.StartDate < today &&
                        (dsl.EndDate == null || dsl.EndDate > today)
                    )
                )
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithOnlyDslNoTvAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Customers
                .Where(c =>
                    _context.DslProducts.Any(dsl =>
                        dsl.CustomerId == c.Id &&
                        dsl.StartDate < today &&
                        (dsl.EndDate == null || dsl.EndDate > today)
                    ) &&
                    !_context.TvProducts.Any(tv =>
                        tv.CustomerId == c.Id &&
                        tv.StartDate < today &&
                        (tv.EndDate == null || tv.EndDate > today)
                    )
                )
                .Distinct()
                .ToListAsync();
        }

        // …

        public async Task<IEnumerable<(int CustomerIdTv, int CustomerIdDsl, DateTime StartDate)>>
     GetLinkedCustomersAsync()
        {
            // 1) Выполняем тот же самый cross-join + Any + Union…Max(), но через ToListAsync()
            var raw = await (
                from c1 in _context.Customers
                from c2 in _context.Customers
                where c1.Id != c2.Id
                      && c1.Email == c2.Email
                      && c1.Address == c2.Address
                      && _context.TvProducts.Any(tv => tv.CustomerId == c1.Id)
                      && _context.DslProducts.Any(dsl => dsl.CustomerId == c2.Id)
                select new
                {
                    CustomerIdTv = c1.Id,
                    CustomerIdDsl = c2.Id,
                    // Берём все даты старта из TV-продуктов c1 и DSL-продуктов c2 и считаем Max()
                    StartDate = _context.TvProducts
                                        .Where(tv => tv.CustomerId == c1.Id)
                                        .Select(tv => tv.StartDate)                // DateOnly?
                                        .Union(
                                           _context.DslProducts
                                               .Where(dsl => dsl.CustomerId == c2.Id)
                                               .Select(dsl => dsl.StartDate)
                                        )
                                        .Max()                                     // DateOnly?
                })
                .Distinct()
                .ToListAsync();

            // 2) Переводим DateOnly? в DateTime и проектируем в кортежи
            var result = raw
                .Select(x =>
                {
                    // если у вас StartDate — DateOnly? (nullable), то:
                    var dto = x.StartDate;
                    var dt = dto.HasValue
                                ? dto.Value.ToDateTime(TimeOnly.MinValue)
                                : DateTime.MinValue;
                    return (x.CustomerIdTv, x.CustomerIdDsl, dt);
                });

            return result;
        }







        public async Task<IEnumerable<(TvProduct, TvProduct)>> GetOverlappingTvProductsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var activeTvs = await _context.TvProducts
                .Where(p => p.StartDate < today && (p.EndDate == null || p.EndDate > today))
                .ToListAsync();

            var result = new List<(TvProduct, TvProduct)>();

            for (int i = 0; i < activeTvs.Count; i++)
            {
                for (int j = i + 1; j < activeTvs.Count; j++)
                {
                    var a = activeTvs[i];
                    var b = activeTvs[j];

                    if (a.CustomerId != b.CustomerId)
                        continue;

                    var aEnd = a.EndDate ?? DateOnly.FromDateTime(DateTime.MaxValue);
                    var bEnd = b.EndDate ?? DateOnly.FromDateTime(DateTime.MaxValue);

                    if (a.StartDate <= bEnd && b.StartDate <= aEnd)
                    {
                        result.Add((a, b));
                    }
                }
            }

            return result;
        }



    }
}

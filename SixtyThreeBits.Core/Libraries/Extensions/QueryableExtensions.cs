using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static class QueryableExtensions
    {
        #region Methods
        public static async Task<ReadOnlyCollection<T>> ToReadOnlyListAsync<T>(this IQueryable<T> source)
        {
            return (await source.ToListAsync()).AsReadOnly();
        }
        #endregion
    }
}
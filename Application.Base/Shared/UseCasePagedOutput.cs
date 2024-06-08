using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public class UseCasePagedOutput<T> : UseCaseOutputBase
    {
        public UseCasePagedOutput(int page, int itemsPerPage, IEnumerable<T> data, int? totalRecords = null)
            : base(true)
        {
            PageNo = page;
            PageSize = itemsPerPage;
            Data = data;
            Records = data.Count();
            TotalCount = totalRecords ?? -1;
        }
        public UseCasePagedOutput() { }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int Records { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get { return TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1); } }
        public IEnumerable<T> Data { get; set; }

        public static UseCasePagedOutput<T> ToPagedOutput(int page, int itemsPerPage, IEnumerable<T> source)
        {
            page = page < 1 ? 0 : page - 1;
            UseCasePagedOutput<T> pagedResult = new UseCasePagedOutput<T>(page, itemsPerPage, source);

            var totalRecords = source.Count();

            if (itemsPerPage <= 0)
            {
                pagedResult.Data = source.ToList();
            }
            else
            {
                pagedResult.Data = source.Skip(itemsPerPage * page)
                    .Take(itemsPerPage)
                    .ToList();
            }

            pagedResult.Records = pagedResult.Data.Count();
            pagedResult.TotalCount = totalRecords;

            return pagedResult;
        }
    }
}

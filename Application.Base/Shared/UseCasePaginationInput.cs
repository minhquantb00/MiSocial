using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public class UseCasePaginationInput
    {
        public UseCasePaginationInput()
        {
            PageNo = 1;
            PageSize = 25;
            SortBy = new List<string>();
            SortDesc = new List<bool>();
        }
        public string Keywords { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public List<string> SortBy { get; set; }
        public List<bool> SortDesc { get; set; }
        public bool MustSort { get; set; }
        public bool MultiSort { get; set; }
    }
}

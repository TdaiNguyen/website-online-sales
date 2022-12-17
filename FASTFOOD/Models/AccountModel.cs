using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using PagedList;
namespace FASTFOOD.Models
{
    public class AccountModel
    {
        private readonly fastfoodEntities context = null;

        public AccountModel()
        {
            context = new fastfoodEntities();
        }

        public bool GetPermission(String Email)
        {
            var sql = from s in context.TAIKHOANs where s.EMAIL == Email select s.AUTH;
            int? res = sql.FirstOrDefault();
            if (res == 1) { return true; }
            else { return false; }
        }

        public bool Login(string Email, string Password)
        {
            object[] sqlParas = {
                new SqlParameter("@EMAIL", Email),
                new SqlParameter("@PASSWORD", Password),
            };

            var res = context.Database.SqlQuery<bool>("Sp_Account_Login @EMAIL, @PASSWORD", sqlParas).SingleOrDefault();
            return res;
        }


         public IEnumerable<MONAN> ListAllPage(int page, int rowLimit)
        {
            return context.MONANs.OrderByDescending(x => x.MSMONAN).ToPagedList(page, rowLimit);
        }

}
}
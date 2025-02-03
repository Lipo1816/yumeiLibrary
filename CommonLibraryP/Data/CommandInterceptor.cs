using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.Data
{
    public class CommandInterceptor: DbCommandInterceptor
    {
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            var a = result.CommandText;
            return result;
        }
    }
}

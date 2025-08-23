using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace innerservice.BLs.Interfaces
{
    public interface IOutterServiceBL
    {
        Guid ExecuteCall();
    }
}
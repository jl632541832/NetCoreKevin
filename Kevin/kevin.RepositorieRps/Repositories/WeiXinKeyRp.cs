﻿using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{
    public class WeiXinKeyRp : Repository<TWeiXinKey, Guid>, IWeiXinKeyRp
    {
        public WeiXinKeyRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}

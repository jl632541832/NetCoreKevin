﻿using kevin.Domain.Kevin;
using Kevin.EntityFrameworkCore._.Data;

namespace kevin.RepositorieRps.Repositories
{

    public class SignRp : Repository<TSign, Guid>, ISignRp
    {
        public SignRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}

using Mapster;
using Salla7ly.Application.Features.Authentication.Command.Contracts;
using Salla7ly.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Application.Features.Authentication.Command.Mapping
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CraftmanSignUpCommand, ApplicationUser>()
            .Map(dest => dest.DateOfBirth, src => DateOnly.Parse(src.DateOfBirth));
        }
    }
}

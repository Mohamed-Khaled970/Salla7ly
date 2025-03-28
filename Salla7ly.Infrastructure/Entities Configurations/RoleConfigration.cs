using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Salla7ly.Application.Common.Consts;
using Salla7ly.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salla7ly.Infrastructure.Entities_Configurations
{
    public class RoleConfigration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData([
                new ApplicationRole
                {
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    NormalizedName = DefaultRoles.Admin.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.UserRoleId,
                    Name = DefaultRoles.User,
                    NormalizedName = DefaultRoles.User.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.UserRoleConcurrencyStamp,
                    IsDefault = true
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.CraftsmanRoleId,
                    Name = DefaultRoles.Craftsman,
                    NormalizedName = DefaultRoles.Craftsman.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.CraftsmanRoleConcurrencyStamp
                }
            ]);
        }
    }
}

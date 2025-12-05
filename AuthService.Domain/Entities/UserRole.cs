using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}

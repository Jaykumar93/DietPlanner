using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblRole
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();
}

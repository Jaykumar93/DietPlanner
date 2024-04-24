using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblUserDetail
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateOnly ModifiedDate { get; set; }

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();
}

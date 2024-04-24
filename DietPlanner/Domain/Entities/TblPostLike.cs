using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblPostLike
{
    public Guid LikeId { get; set; }

    public Guid ProfileId { get; set; }

    public Guid PostId { get; set; }

    public DateTime LikeDateTime { get; set; }

    public virtual TblUserPost Post { get; set; } = null!;

    public virtual TblProfileDetail Profile { get; set; } = null!;
}

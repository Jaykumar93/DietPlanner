using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblPostComment
{
    public Guid CommentId { get; set; }

    public Guid ProfileId { get; set; }

    public Guid PostId { get; set; }

    public string CommentDetail { get; set; } = null!;

    public DateTime CommentDateTime { get; set; }

    public virtual TblUserPost Post { get; set; } = null!;

    public virtual TblProfileDetail Profile { get; set; } = null!;
}

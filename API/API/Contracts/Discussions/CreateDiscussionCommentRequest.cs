using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Discussions;

public class CreateDiscussionCommentRequest
{
    [Description("Текст комментария к идее.")]
    [Required]
    [MaxLength(4000)]
    public string CommentBody { get; set; } = string.Empty;

    [Description("Id родительского комментария, если это ответ. Для обычного комментария передавай null.")]
    public Guid? ParentCommentId { get; set; }
}

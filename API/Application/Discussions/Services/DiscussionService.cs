using Application.Abstractions.Discussions;
using Application.Abstractions.Persistence;
using Application.Discussions.Models;

namespace Application.Discussions.Services;

public class DiscussionService(IUnitOfWork unitOfWork) : IDiscussionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<GetDiscussionIdeasResult> GetIdeasAsync(GetDiscussionIdeasQuery query, CancellationToken cancellationToken = default)
    {
        return _unitOfWork.Projects.GetDiscussionIdeasAsync(query, cancellationToken);
    }
}

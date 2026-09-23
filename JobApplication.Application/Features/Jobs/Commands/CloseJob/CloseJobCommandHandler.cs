using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommandHandler : IRequestHandler<CloseJobCommand>
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly ICurrentUser _currentUser;

        public CloseJobCommandHandler(IRepository<Job> jobRepository, ICurrentUser currentUser)
        {
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public async Task Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can close jobs.");

            var job = _jobRepository.Get().FirstOrDefault(j => j.Id == request.Id)
                ?? throw new NotFoundException("Job not found.");

            if (job.RecruiterId != recruiterId)
            {
                throw new ForbiddenException("You can only close your own jobs.");
            }

            job.Close(recruiterId);
            await _jobRepository.SaveChangesAsync();
        }
    }
}

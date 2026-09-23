using JobApplication.Application.Exceptions;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, int>
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly ICurrentUser _currentUser;

        public CreateJobCommandHandler(IRepository<Job> jobRepository, ICurrentUser currentUser)
        {
            _jobRepository = jobRepository;
            _currentUser = currentUser;
        }

        public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var recruiterId = _currentUser.RecruiterId
                ?? throw new ForbiddenException("Only recruiters can create jobs.");

            var job = new Job()
            {
                Title = request.Title,
                Description = request.Description,
                IsActive = true,
                RecruiterId = recruiterId
            };

            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            return job.Id;
        }
    }
}

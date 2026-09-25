using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseExpiredJobs
{
    public class CloseExpiredJobsCommandHandler : IRequestHandler<CloseExpiredJobsCommand, int>
    {
        private const int MaxOpenDays = 30;

        private readonly IRepository<Job> _jobRepository;

        public CloseExpiredJobsCommandHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> Handle(CloseExpiredJobsCommand request, CancellationToken cancellationToken)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-MaxOpenDays);

            var expiredJobs = _jobRepository.Get()
                .Where(j => j.IsActive && j.CreatedAt <= cutoffDate)
                .ToList();

            if (expiredJobs.Count == 0)
            {
                return 0;
            }

            foreach (var job in expiredJobs)
            {
                job.CloseAutomatically();
                _jobRepository.Update(job);
            }

            await _jobRepository.SaveChangesAsync();

            return expiredJobs.Count;
        }
    }
}


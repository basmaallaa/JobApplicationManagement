using JobApplication.Application.Features.Jobs.Commands.CloseExpiredJobs;
using MediatR;

namespace JobApplication.API.Jobs
{
    public class CloseExpiredJobsRecurringJob
    {
        public const string JobId = "close-expired-jobs";

        private readonly ISender _sender;
        private readonly ILogger<CloseExpiredJobsRecurringJob> _logger;

        public CloseExpiredJobsRecurringJob(ISender sender, ILogger<CloseExpiredJobsRecurringJob> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var closedCount = await _sender.Send(new CloseExpiredJobsCommand(), cancellationToken);

            _logger.LogInformation(
                "Recurring job '{JobId}' closed {ClosedCount} job(s) open for more than 30 days.",
                JobId,
                closedCount);
        }
    }
}

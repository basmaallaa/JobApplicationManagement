using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseExpiredJobs
{
    public class CloseExpiredJobsCommand : IRequest<int>
    {
    }
}

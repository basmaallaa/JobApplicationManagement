using JobApplication.Domain.Exceptions;

namespace JobApplication.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description  { get; set; }
        public bool IsActive { get; set; }
        public int? RecruiterId { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int? ClosedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        // A job is closed when IsActive is false.
        public void Close(int recruiterId)
        {
            if (!IsActive)
            {
                throw new DomainException("Job is already closed.");
            }

            IsActive = false;
            ClosedAt = DateTime.UtcNow;
            ClosedBy = recruiterId;
        }

        public void CloseAutomatically()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            ClosedAt = DateTime.UtcNow;
            ClosedBy = null;
        }
    }
}

using CateringBackend.Domain.Data;
using CateringBackend.Domain.Entities.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CateringBackend.Users.Producer.Commands
{
    public class AnswerComplaintCommand
    {
        public string Compliant_answer{ get; set; }
    }

    public class AnswerComplaintWithIdCommand : AnswerComplaintCommand,
        IRequest<(bool complaintExists, bool complaintAnswered, string errorMessage)>
    {
        public Guid ComplaintId { get; set; }
        public AnswerComplaintWithIdCommand(AnswerComplaintCommand command, Guid complaintId)
        {
            Compliant_answer = command.Compliant_answer;
            ComplaintId = complaintId;
        }
    }


    public class AnswerComplaintWithIdCommandHandler : IRequestHandler<AnswerComplaintWithIdCommand,
        (bool complaintExists, bool complaintAnswered, string errorMessage)>
    {
        private readonly CateringDbContext _dbContext;
        public AnswerComplaintWithIdCommandHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<(bool complaintExists, bool complaintAnswered, string errorMessage)> 
            Handle(AnswerComplaintWithIdCommand request, CancellationToken cancellationToken)
        {
            var complaint = _dbContext.Complaints.FirstOrDefault(c => c.Id == request.ComplaintId);

            if (complaint == default)
                return (complaintExists: false, complaintAnswered: false, 
                    errorMessage: $"Complaint o Id {request.ComplaintId} nie istnieje");

            if (complaint.Status == ComplaintStatus.Closed)
                return (complaintExists: true, complaintAnswered: false,
                    errorMessage: "Complaint ma już odpowiedź");

            complaint.Answer = request.Compliant_answer;
            complaint.Status = ComplaintStatus.Closed;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return (complaintExists: true, complaintAnswered: true,
                errorMessage: string.Empty); 
        }
    }
}

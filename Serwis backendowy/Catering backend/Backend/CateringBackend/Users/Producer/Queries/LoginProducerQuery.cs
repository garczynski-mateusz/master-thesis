using System.Threading;
using System.Threading.Tasks;
using CateringBackend.AuthUtilities;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CateringBackend.Users.Producer.Queries
{
    public record LoginProducerQuery : LoginQuery, IRequest<string>;

    public class LoginProducerQueryHandler : IRequestHandler<LoginProducerQuery, string>
    {
        private readonly CateringDbContext _dbContext;

        public LoginProducerQueryHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Handle(LoginProducerQuery request, CancellationToken cancellationToken)
        {
            var producer = await _dbContext.Producers.FirstOrDefaultAsync(
                x => x.Email == request.Email && x.Password == PasswordManager.Encrypt(request.Password), 
                cancellationToken);

            if (producer == default)
                return null;

            return JwtTokenUtilities.GetAuthenticationToken(producer.Id, producer.Email, UserRole.producer);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using CateringBackend.AuthUtilities;
using CateringBackend.Domain.Data;
using CateringBackend.Domain.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CateringBackend.Users.Deliverer.Queries
{
    public record LoginDelivererQuery : LoginQuery, IRequest<string>;

    public class LoginDelivererQueryHandler : IRequestHandler<LoginDelivererQuery, string>
    {
        private readonly CateringDbContext _dbContext;

        public LoginDelivererQueryHandler(CateringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> Handle(LoginDelivererQuery request, CancellationToken cancellationToken)
        {
            var deliverer = await _dbContext.Deliverers.FirstOrDefaultAsync(
                x => x.Email == request.Email && x.Password == PasswordManager.Encrypt(request.Password), 
                cancellationToken);

            if (deliverer == default)
                return null;

            return JwtTokenUtilities.GetAuthenticationToken(deliverer.Id, deliverer.Email, UserRole.deliverer);
        }
    }
}
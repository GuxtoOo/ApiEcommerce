using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiEcommerce.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly Infrastructure.Persistence.Repositories.IOrderRepository _repo;

        public DeleteOrderHandler(Infrastructure.Persistence.Repositories.IOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(request.Id);
            if (order is null) return false;

            await _repo.DeleteAsync(order);
            return true;
        }
    }
}

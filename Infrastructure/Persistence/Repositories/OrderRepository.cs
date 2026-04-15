using ApiEcommerce.Domain.Entities;
using ApiEcommerce.Domain.Enums;
using ApiEcommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Infrastructure.Persistence.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order o); Task<List<Order>> GetAllAsync(OrderStatus? s);
    Task<Order?> GetByIdAsync(int id); Task UpdateAsync(Order o); Task DeleteAsync(Order o);
}
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _ctx; public OrderRepository(AppDbContext c) => _ctx = c;
    public async Task AddAsync(Order o) { _ctx.Orders.Add(o); await _ctx.SaveChangesAsync(); }
    public async Task<List<Order>> GetAllAsync(OrderStatus? s) { var q = _ctx.Orders.AsQueryable(); if (s.HasValue) q = q.Where(x => x.Status == s); return await q.ToListAsync(); }
    public Task<Order?> GetByIdAsync(int id) => _ctx.Orders.FindAsync(id).AsTask();
    public async Task UpdateAsync(Order o) { _ctx.Orders.Update(o); await _ctx.SaveChangesAsync(); }
    public async Task DeleteAsync(Order o) { _ctx.Orders.Remove(o); await _ctx.SaveChangesAsync(); }
}

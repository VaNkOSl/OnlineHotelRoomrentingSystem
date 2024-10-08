﻿namespace OnlineHotelRoomrentingSystem.Data.Data.Common;

public interface IRepository
{
    IQueryable<T> All<T>() where T : class;
    IQueryable<T> AllReadOnly<T>() where T : class;
    Task AddAsync<T>(T entity) where T : class;
    Task<int> SaveChangesAsync();
    Task<T?> GetByIdAsync<T>(object id) where T : class;
    Task DeleteAsync<T>(object id) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteRange<T>(IEnumerable<T> entities) where T : class;
}

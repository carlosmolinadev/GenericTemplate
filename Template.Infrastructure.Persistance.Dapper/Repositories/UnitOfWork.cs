﻿using Npgsql;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Template.Core.Application.Contracts.Persistence;

namespace Template.Infrastructure.Persistance.Dapper.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbConnection _connection;
        private DbTransaction _transaction;
        private bool _disposed = false;

        public UnitOfWork(DbConnection connection)
        {
            _connection = connection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public ICustomerRepository GetCustomerRepository()
        {
            return new CustomerRepository(_connection);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_connection);
        }

        public async Task Save()
        {
            await CommitTransaction();
        }

        private async Task CommitTransaction()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                Dispose();
                _transaction = await _connection.BeginTransactionAsync();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
            }

            _disposed = true;
            

        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
        //public void Dispose()
        //{
        //    _transaction?.DisposeAsync();
        //}

    }
}

////Sync
//public class UnitOfWork : IUnitOfWork, IDisposable
//{
//    private readonly DbConnection _connection;
//    private DbTransaction _transaction;

//    public UnitOfWork(NpgsqlConnection connection)
//    {
//        _connection = new NpgsqlConnection(connection.ConnectionString);
//        _connection.Open();
//    }

//    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
//    {
//        return new Repository<TEntity>(_connection);
//    }

//    public async Task Save()
//    {
//        CommitTransaction();
//    }

//    private void CommitTransaction()
//    {
//        try
//        {
//            _transaction = _connection.BeginTransaction();
//            _transaction.Commit();
//        }
//        catch (Exception ex)
//        {
//            _transaction.Rollback();
//            throw;
//        }
//        finally
//        {
//            Dispose();
//            _transaction = _connection.BeginTransaction();
//        }
//    }

//    public void Dispose()
//    {
//        _transaction?.Dispose();
//    }



////Await
//private readonly DbConnection _connection;
//private DbTransaction _transaction;

//public UnitOfWork(NpgsqlConnection connection)
//{
//    _connection = new NpgsqlConnection(connection.ConnectionString);
//    _connection.Open();
//}

//public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
//{
//    return new Repository<TEntity>(_connection);
//}

//public async Task Save()
//{
//    await CommitTransaction();
//}

//private async Task CommitTransaction()
//{
//    try
//    {
//        _transaction = await _connection.BeginTransactionAsync();
//        await _transaction.CommitAsync();
//    }
//    catch (Exception ex)
//    {
//        await _transaction.RollbackAsync();
//        throw;
//    }
//    finally
//    {
//        Dispose();
//        _transaction = await _connection.BeginTransactionAsync();
//    }
//}

//public void Dispose()
//{
//    _transaction?.DisposeAsync();
//}




//public class UnitOfWork : IUnitOfWork, IDisposable
//{
//    private readonly DbConnection _connection;
//    private DbTransaction _transaction;
//    private bool _disposed = false;

//    public UnitOfWork(NpgsqlConnection connection)
//    {
//        _connection = connection;
//        //_connection.Open();
//        BeginTransaction();
//    }

//    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
//    {
//        return new Repository<TEntity>(_connection);
//    }

//    public void Save()
//    {
//        try
//        {
//            CommitTransaction();
//        }
//        catch (Exception ex)
//        {
//            RollbackTransaction();
//            throw;
//        }
//        finally
//        {
//            Dispose();
//            BeginTransaction();
//        }
//    }

//    private void BeginTransaction()
//    {
//        if (_transaction != null)
//        {
//            return;
//        }

//        _connection.Open();
//        _transaction = _connection.BeginTransaction();
//    }

//    private void CommitTransaction()
//    {
//        try
//        {
//            _transaction.CommitAsync();
//        }
//        catch (Exception ex)
//        {
//            RollbackTransaction();
//            throw;
//        }
//        finally
//        {
//            if (_transaction != null)
//            {
//                _transaction.Dispose();
//                _transaction = null;
//            }
//        }
//    }

//    private void RollbackTransaction()
//    {
//        try
//        {
//            _transaction.Rollback();
//        }
//        finally
//        {
//            if (_transaction != null)
//            {
//                _transaction.Dispose();
//                _transaction = null;
//            }
//        }
//    }

//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (_disposed)
//        {
//            return;
//        }

//        if (disposing)
//        {
//            if (_transaction != null)
//            {
//                _transaction.Dispose();
//                _transaction = null;
//            }

//            _connection.Dispose();
//        }

//        _disposed = true;
//    }

//    ~UnitOfWork()
//    {
//        Dispose(false);
//    }
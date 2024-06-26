﻿using System.Diagnostics.CodeAnalysis;
using Itau.Transfer.Infrastructure.Context;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;

namespace Itau.Transfer.Infrastructure.Repositories;
[ExcludeFromCodeCoverage]
public class RepositoryBase : IRepositoryBase
{
    protected readonly AppDbContext _context;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbconnectionFactory;

    public DbInitializer(IDbConnectionFactory _connectionFactory)
    {
        _dbconnectionFactory = _connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbconnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("""
            create table if not exists movies (
            id UUID primary key,
            slug TEXT not null, 
            title TEXT not null,
            yearofrelease integer not null);
        """);

        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx
            on movies
            using btree(slug);
        """);

        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx
            on movies
            using btree(slug);
        """);

        await connection.ExecuteAsync("""
            create table if not exists genres (
            movieId UUID references movies (Id),
            name TEXT not null);
        """);
    }

}

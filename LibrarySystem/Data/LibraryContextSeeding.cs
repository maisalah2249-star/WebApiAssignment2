using System.Text.Json;
using LibrarySystem.Entities;

namespace LibrarySystem.Data;

public static class LibraryContextSeeding
{
    private static readonly string SeedingPath =
        Path.Combine(AppContext.BaseDirectory, "Data", "Seeding");

    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        // Order matters: Books depend on Authors
        if (!dbContext.Authors.Any())
        {
            var authors = await ReadAsync<Author>("Authors.json");
            if (authors is not null)
            {
                await dbContext.Authors.AddRangeAsync(authors);
                await dbContext.SaveChangesAsync();
            }
        }

        if (!dbContext.Books.Any())
        {
            var books = await ReadAsync<Book>("Books.json");
            if (books is not null)
            {
                await dbContext.Books.AddRangeAsync(books);
                await dbContext.SaveChangesAsync();
            }
        }

        if (!dbContext.Borrowers.Any())
        {
            var borrowers = await ReadAsync<Borrower>("Borrowers.json");
            if (borrowers is not null)
            {
                await dbContext.Borrowers.AddRangeAsync(borrowers);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static async Task<List<T>?> ReadAsync<T>(string fileName)
    {
        var path = Path.Combine(SeedingPath, fileName);
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json);
    }
}
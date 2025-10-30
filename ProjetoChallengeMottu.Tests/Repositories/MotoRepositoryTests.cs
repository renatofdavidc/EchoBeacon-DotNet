using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Filters;
using ProjetoChallengeMottu.Models;
using ProjetoChallengeMottu.Repositories;

namespace ProjetoChallengeMottu.Tests.Repositories;

public class MotoRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Add_GetById_Update_Delete_Works()
    {
        using var ctx = CreateContext();
        var repo = new MotoRepository(ctx);

        var m = new Moto { IdMoto = 1, Placa = "ABC1234", Chassi = "CHS-001", Problema = "Ruído", CustoManutencao = 100 };

        var created = await repo.AddAsync(m);
        Assert.Equal(1, created.IdMoto);

        var fetched = await repo.GetByIdAsync(1);
        Assert.NotNull(fetched);
        Assert.Equal("ABC1234", fetched!.Placa);

        var updated = await repo.UpdateAsync(1, new Moto { Placa = "DEF5678", Chassi = "CHS-001", Problema = "Pneu", CustoManutencao = 200 });
        Assert.NotNull(updated);
        Assert.Equal("DEF5678", updated!.Placa);
        Assert.Equal(200, updated.CustoManutencao);

        var ok = await repo.DeleteAsync(1);
        Assert.True(ok);
        Assert.Null(await repo.GetByIdAsync(1));
    }

    [Fact]
    public async Task GetAll_Paging_And_Filters_Work()
    {
        using var ctx = CreateContext();
        var repo = new MotoRepository(ctx);

        // Seed
        for (int i = 1; i <= 25; i++)
        {
            ctx.Motos.Add(new Moto
            {
                IdMoto = i,
                Placa = $"PL{i:0000}",
                Chassi = $"CHS-{i:0000}",
                Problema = i % 2 == 0 ? "Ruído" : "Pneu",
                CustoManutencao = i
            });
        }
        await ctx.SaveChangesAsync();

        var page1 = await repo.GetAllAsync(new MotoFilter { Page = 1, Size = 10 });
        Assert.Equal(10, page1.Items.Count());
        Assert.Equal(25, page1.TotalItems);

        var filtroProblema = await repo.GetAllAsync(new MotoFilter { Problema = "ruÍdo", Page = 1, Size = 50 });
        Assert.All(filtroProblema.Items, m => Assert.Equal("Ruído", m.Problema));

        var filtroCusto = await repo.GetAllAsync(new MotoFilter { CustoManutencaoMin = 5, CustoManutencaoMax = 10, Page = 1, Size = 50 });
        Assert.All(filtroCusto.Items, m => Assert.InRange(m.CustoManutencao ?? 0, 5, 10));
    }
}

using System.Dynamic;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    //EndpointName constant for GetGame
    const string GetGame = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        //Instead of using App, we can use the variable group in the endpoints
        var group = app.MapGroup("/games");

        //GET /games
        group.MapGet("/", async (GameStoreContext dbContext)
        => await dbContext.Games.Include(game => game.Genre).Select(game => new GameSummaryDto(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            game.ReleaseDate
        )).AsNoTracking().
        ToListAsync()
        );

        //GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(
                new GameDetailsDto(
                    game.Id,
                    game.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate
                )
            );

        })
        .WithName(GetGame);

        //POST /games
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            //Creating an object of dbmodel
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            //Returns a task
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(GetGame, new { id = gameDto.Id }, gameDto);
        });

        //PUT /games/ 1
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            games.FindIndex(game => game.Id == id);

            //Handling non-existing game by its index 
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();


            return Results.NoContent();

        });

        //DELETE /games /1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            dbContext.Games.Remove(game);

            await dbContext.SaveChangesAsync();

            return Results.NoContent();


        });
    }

}

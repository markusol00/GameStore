using GameStore.Api.Dtos;
using Microsoft.VisualBasic;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    //EndpointName constant for GetGame
    const string GetGame = "GetGame";



    private static readonly List<GameDto> games = [
       new (1,
    "Steet fighter II",
    "Fighting",
    19.99M,
    new DateOnly(1992,7, 15)),
    new (2,
    "Grand Theft Auto III",
    "Action",
    19.99M,
    new DateOnly(2001, 6, 15)),
    new (3,
    "FIFA 20",
    "RPG",
    59.99M,
    new DateOnly(2019,9, 15)),

];

    public static void MapGamesEndpoints(this WebApplication app)
    {
        //Instead of using App, we can use the variable group in the endpoints
        var group = app.MapGroup("/games");

        //GET /games
        group.MapGet("/", () => games);

        //GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            var game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok();

        })
        .WithName(GetGame);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame) =>
        {
            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            games.Add(game);

            return Results.CreatedAtRoute(GetGame, new { id = game.Id }, game);
        });

        //PUT /games/ 1
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            //Handling non-existing game by its index 
            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();

        });

        //DELETE /games /1
        group.MapDelete("/{id}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);

            return Results.NoContent();


        });
    }

}

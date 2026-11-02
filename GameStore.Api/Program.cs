using GameStore.Api.Dtos;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<GameDto> games = [
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
//GET /games
app.MapGet("/games", () => games);

//GET /games/1
app.MapGet("/games/{id}", (int id) => games.Find(game => game.Id == id))
.WithName("GetGame");

//POST /games
app.MapPost("/games", (CreateGameDto newGame) =>
{
    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );
    games.Add(game);

    return Results.CreatedAtRoute("GetGame", new { id = game.Id }, game);
});

app.Run();

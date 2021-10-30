package com.example

class GamesManager {
    private var spareGameId: GameId = 0
    private val gamesList = ArrayList<Game>()

    fun createNewGame(player1Id: UserId, player2Id: UserId): Game {
        val gameId = spareGameId++
        val game = Game(gameId, player1Id, player2Id, GameProperties(2, Speed.NORM))
        gamesList.add(game)

        return game
    }

    fun gameById(gameId: GameId) = gamesList.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = gamesList.find { (it.player1Id == userId) or (it.player2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove) {
        val game = gameById(gameId)
        game?.makeMove(move)
    }
}
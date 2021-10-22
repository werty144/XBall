package com.example

class GamesManager {
    private var spareGameId: GameId = 0
    private val gamesList = ArrayList<Game>()

    fun createNewGame(player1Id: UserId, player2Id: UserId): Game {
        val gameId = spareGameId++
        val game = Game(gameId, player1Id, player2Id, 0)
        gamesList.add(game)

        return game
    }

    fun gameById(gameId: GameId) = gamesList.first {it.gameId == gameId}

    fun gameSateById(gameId: GameId) = gameById(gameId).state

    fun getGamesForUser(userId: UserId): List<Game> = gamesList.filter { (it.player1Id == userId) or (it.player2Id == userId) }

    fun setState(gameId: GameId, newState: Int) {
        gamesList.first {it.gameId == gameId}.state = newState
    }

    fun makeMove(gameId: GameId, move: String) {
        val game = gameById(gameId)
        if (move == "inc") {
            game.state++
        } else {
            game.state--
        }
    }
}
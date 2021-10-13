package com.example

class GamesManager {
    private var spareGameId: GameId = 0
    private val gamesList = ArrayList<Game>()

    fun createNewGame(player1Id: UserId, player2Id: UserId): Game {
        val gameId = spareGameId++
        val game = Game(gameId, player1Id, player2Id)
        gamesList.add(game)

        return game
    }

    fun gameById(gameId: GameId) = gamesList.first {it.id == gameId}

    fun gameSateById(gameId: GameId) = gameById(gameId).state

    fun getGamesForUser(userId: UserId): List<Game> = gamesList.filter { (it.player1Id == userId) or (it.player2Id == userId) }

    fun setState(gameId: GameId, newState: Int) {
        gamesList.first {it.id == gameId}.state = newState
    }
}
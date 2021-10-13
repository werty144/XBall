package com.example

class GamesManager {
    private var spareId: GameId = 0
    val gamesList = ArrayList<Game>()

    fun createNewGame(): Game {
        val gameId = spareId++
        val game = Game(gameId)
        gamesList.add(game)

        return game
    }

    fun gameById(id: GameId) = gamesList.first {it.id == id}
}
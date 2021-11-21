package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.delay
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.encodeToJsonElement

class GamesManager {
    private var spareGameId: GameId = 0
    private val gamesList = ArrayList<Game>()
    private val updateTime = 10L

    fun createNewGame(player1Id: UserId, player2Id: UserId): Game {
        val gameId = spareGameId++
        val game = Game(gameId, player1Id, player2Id, GameProperties(2, Speed.NORM))
        gamesList.add(game)

        return game
    }

    fun gameById(gameId: GameId) = gamesList.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = gamesList.find { (it.player1Id == userId) or (it.player2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }

    suspend fun runGame(game: Game, firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(updateTime)
            game.nextState(updateTime)
            val message = Json.encodeToString(APIRequest("gameState", Json.encodeToJsonElement(game.state)))
            firstPlayerConnection.session.send(message)
            secondPlayerConnection.session.send(message)
        }
    }
}
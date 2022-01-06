package com.example.infrastructure

import com.example.game.Game
import com.example.game.GameId
import com.example.routing.APIMove
import com.example.routing.APIRequest
import com.example.routing.Connection
import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.delay
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonElement
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.encodeToJsonElement

class GamesManager {
    private var spareGameId: GameId = 0
    private val gamesList = ArrayList<Game>()
    private val updateTime = 10L

    fun createNewGame(invite: Invite): Game {
        val gameId = spareGameId++
        val game = Game(gameId, invite.inviterId, invite.invitedId, invite.gameProperties, updateTime)
        gamesList.add(game)

        return game
    }

    fun gameById(gameId: GameId) = gamesList.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = gamesList.find { (it.user1Id == userId) or (it.user2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }

    suspend fun runGame(game: Game, firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(updateTime)
            game.nextState()
            val message = Json.encodeToString(JsonObject(mapOf(
                "path" to Json.encodeToJsonElement("game"),
                "body" to JsonObject(mapOf(
                    "state" to Json.encodeToJsonElement(game.state),
                    "score" to Json.encodeToJsonElement(game.score),
                    "time" to Json.encodeToJsonElement(game.timer.time)
                )
            ))))
            firstPlayerConnection.session.send(message)
            secondPlayerConnection.session.send(message)
        }
    }
}
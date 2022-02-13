package com.example.infrastructure

import com.example.game.Game
import com.example.game.GameId
import com.example.game.GameStatus
import com.example.routing.APIMove
import com.example.routing.Connection
import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.withContext
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.encodeToJsonElement

class GamesManager {
    private var spareGameId: GameId = 0
    val games = ArrayList<Game>()
    private val updateTime = 10L

    fun createNewGame(invite: Invite): Game {
        val game = Game(spareGameId++, invite.inviterId, invite.invitedId, invite.gameProperties, updateTime)
        games.add(game)

        return game
    }

    fun gameById(gameId: GameId) = games.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = games.find { (it.user1Id == userId) or (it.user2Id == userId) }

    fun userHasGames(userId: UserId): Boolean = games.any { (it.user1Id == userId) or (it.user2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }

    suspend fun runGame(game: Game, firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(updateTime)
            game.nextState()
            val message = Json.encodeToString(
                JsonObject(
                    mapOf(
                        "path" to Json.encodeToJsonElement("game"),
                        "body" to JsonObject(
                            mapOf(
                                "state" to Json.encodeToJsonElement(game.state),
                                "score" to Json.encodeToJsonElement(game.score.toString()),
                                "time" to Json.encodeToJsonElement(game.timer.time),
                                "status" to Json.encodeToJsonElement(game.getStatus())
                            )
                        )
                    )
                )
            )
            firstPlayerConnection.session.send(message)
            secondPlayerConnection.session.send(message)

            if (game.getStatus() == GameStatus.ENDED) {
                games.remove(game)
                return
            }
        }
    }
}
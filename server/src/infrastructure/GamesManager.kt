package com.example.infrastructure

import com.example.game.Game
import com.example.game.GameId
import com.example.game.GameStatus
import com.example.routing.APIMove
import com.example.routing.Connection
import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.withContext
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.encodeToJsonElement
import java.util.Collections

class GamesManager {
    private var spareGameId: GameId = 0
    val games: MutableSet<Game> = Collections.synchronizedSet(LinkedHashSet())
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

    suspend fun runGame(game: Game, firstUserId: UserId, secondUserId: UserId, connections: Set<Connection>) {
        var firstPlayerConnection = connections.firstOrNull { (it.userId == firstUserId) and it.session.isActive}
        var secondPlayerConnection = connections.firstOrNull { (it.userId == secondUserId) and it.session.isActive }
        println(firstPlayerConnection)
        println(secondPlayerConnection)
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

            if ((firstPlayerConnection != null) && firstPlayerConnection.session.isActive) {
                firstPlayerConnection.session.send(message)
            } else {
                firstPlayerConnection = connections.firstOrNull { (it.userId == firstUserId) and it.session.isActive}
            }

            if ((secondPlayerConnection != null) && secondPlayerConnection.session.isActive) {
                secondPlayerConnection.session.send(message)
            } else {
                secondPlayerConnection = connections.firstOrNull { (it.userId == secondUserId) and it.session.isActive}
            }

            if (game.getStatus() == GameStatus.ENDED) {
                games.remove(game)
                return
            }
        }
    }
}
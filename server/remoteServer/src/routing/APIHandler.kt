package com.xballserver.remoteserver.routing

import com.xballserver.remoteserver.infrastructure.*
import io.ktor.websocket.*
import kotlinx.coroutines.coroutineScope
import kotlinx.serialization.json.*
import kotlinx.serialization.DeserializationStrategy


fun <T>tryJsonParse(serializer: DeserializationStrategy<T>, input: Any): T? {
    return try {
        when (input) {
            is String -> Json.decodeFromString(serializer, input)
            is JsonElement -> Json.decodeFromJsonElement(serializer, input)
            else -> null
        }
    } catch (e: Exception) {
        null
    }
}

class APIHandler(
    private val gamesManager: GamesManager,
    private val gameStartManager: GameStartManager
) {
    suspend fun handle(path: String, userId: UserId, body: JsonElement) {
        when (path) {
            "lobbyReady" -> {
                val apiLobby = tryJsonParse(APILobby.serializer(), body) ?: return
                val lobby = apiLobby.toLobby()
                gameStartManager.handleLobbyReadyRequest(lobby, userId)
            }
            "gameReady" -> {
                gameStartManager.handleGameReadyRequest(userId)
            }
            "makeMove" -> {
                val move = tryJsonParse(APIMove.serializer(), body) ?: return
                val gameId = gamesManager.getGameForUser(userId)?.gameId ?: return
                gamesManager.makeMove(gameId, move, userId)
            }
        }
    }

}

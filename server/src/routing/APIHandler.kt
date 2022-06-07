package com.example.routing

import com.example.infrastructure.*
import io.ktor.http.cio.websocket.*
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
    private val lobbyManager: LobbyManager,
    private val connections: Set<Connection>
) {
    fun handle(frame: Frame, connection: Connection) {
        when (frame) {
            is Frame.Text -> {
                val userId = connection.userId
                val requestString = frame.readText()
                val request = tryJsonParse(APIRequest.serializer(), requestString) ?: return

                when (request.path) {
                    "lobbyReady" -> {
                        val requestBody = tryJsonParse(APILobby.serializer(), request.body) ?: return
                        lobbyManager.lobbyReady(
                            requestBody.lobbyID,
                            userId,
                            requestBody.nMembers,
                            requestBody.gameProperties)
                    }
                    "makeMove" -> {
                        val requestBody = tryJsonParse(APIMakeMove.serializer(), request.body) ?: return
                        val gameId = gamesManager.getGameForUser(userId)?.gameId ?: return

                        val move: APIMove = tryJsonParse(APIMove.serializer(), requestBody.move) ?: return
                        gamesManager.makeMove(gameId, move, userId)
                    }
                }
            }
        }
    }
}

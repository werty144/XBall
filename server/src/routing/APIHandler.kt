package com.example.routing

import com.example.infrastructure.Coupler
import com.example.infrastructure.GamesManager
import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.*
import kotlinx.coroutines.launch
import kotlinx.serialization.DeserializationStrategy
import java.util.logging.Logger


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

class APIHandler(private val application: Application) {

    private val gamesManager = GamesManager()
    private val coupler = Coupler()

    suspend fun handle(frame: Frame, connection: Connection, connections: Set<Connection>) {
        when (frame) {
            is Frame.Text -> {
                val userId = connection.id
                val requestString = frame.readText()
                val request = tryJsonParse(APIRequest.serializer(), requestString) ?: return

                when (request.path) {
                    "invite" -> {
                        val invite = tryJsonParse(APIInvite.serializer(), request.body) ?: return
                        val newInvite = coupler.formNewInvite(userId, invite)
                        if  (newInvite != null) connections.find { it.id == invite.invitedId }?.session?.send(
                            Json.encodeToString(APIRequest("invite", Json.encodeToJsonElement(newInvite)))
                        )
                    }
                    "acceptInvite" -> {
                        val requestBody = tryJsonParse(APIAcceptInvite.serializer(), request.body) ?: return
                        val inviteId = requestBody.inviteId
                        val invite = coupler.getInviteById(inviteId)
                        val game = gamesManager.createNewGame(invite)
                        val firstPlayerConnection = connections.first { it.id == invite.inviterId }
                        val secondPlayerConnection = connections.first { it.id == invite.invitedId }
                        application.launch {gamesManager.runGame(game, firstPlayerConnection, secondPlayerConnection)}
                    }
                    "makeMove" -> {
                        val requestBody = tryJsonParse(APIMakeMove.serializer(), request.body) ?: return
                        val gameId = gamesManager.getGameForUser(userId)?.gameId

                        val move: APIMove = tryJsonParse(APIMove.serializer(), requestBody.move) ?: return
                        if (gameId != null) gamesManager.makeMove(gameId, move, userId)
                    }
                }
            }
        }
    }
}

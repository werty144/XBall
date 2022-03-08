package com.example.routing

import com.example.infrastructure.InvitesManager
import com.example.infrastructure.GamesManager
import com.example.infrastructure.UserId
import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.Dispatchers
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.*
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
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
    private val invitesManager: InvitesManager,
    private val connections: Set<Connection>
) {
    suspend fun handle(frame: Frame, connection: Connection) {
        when (frame) {
            is Frame.Text -> {
                val userId = connection.userId
                val requestString = frame.readText()
                val request = tryJsonParse(APIRequest.serializer(), requestString) ?: return

                when (request.path) {
                    "invite" -> {
                        if (gamesManager.userHasGames(userId)) return
                        val invite = tryJsonParse(APIInvite.serializer(), request.body) ?: return
                        val newInvite = invitesManager.formNewInvite(userId, invite) ?: return
                        connections.find { it.userId == invite.invitedId }?.session?.send(
                            Json.encodeToString(APIRequest("invite", Json.encodeToJsonElement(newInvite)))
                        )
                    }
                    "acceptInvite" -> {
                        if (gamesManager.userHasGames(userId)) return
                        val requestBody = tryJsonParse(APIAcceptInvite.serializer(), request.body) ?: return
                        val inviteId = requestBody.inviteId
                        val invite = invitesManager.getInviteById(inviteId) ?: return
                        val game = gamesManager.createNewGame(invite)
                        gamesManager.launchGame(game, invite.inviterId, invite.invitedId, connections)
                        invitesManager.removeInviteById(inviteId)
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

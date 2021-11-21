package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.*
import kotlinx.coroutines.launch


class APIHandler(private val coroutineScope: CoroutineScope) {

    private val gamesManager = GamesManager()
    private val coupler = Coupler()

    suspend fun handle(frame: Frame, connection: Connection, connections: Set<Connection>) {
        when (frame) {
            is Frame.Text -> {
                val userId = connection.id
                val requestString = frame.readText()
                val request: APIRequest = Json.decodeFromString(APIRequest.serializer(), requestString)
                val requestPath = request.path

                when (requestPath) {
                    "invite" -> {
                        val requestBody = Json.decodeFromJsonElement(APIInvite.serializer(), request.body)
                        val invitedId = requestBody.invitedId
                        val newInvite = coupler.formNewInvite(userId, invitedId)
                        if  (newInvite != null) connections.find { it.id == invitedId }?.session?.send(
                            Json.encodeToString(APIRequest("invite", Json.encodeToJsonElement(newInvite)))
                        )
                    }
                    "acceptInvite" -> {
                        val requestBody = Json.decodeFromJsonElement(APIAcceptInvite.serializer(), request.body)
                        val inviteId = requestBody.inviteId
                        val invite = coupler.getInviteById(inviteId)
                        val game = gamesManager.createNewGame(invite.inviterId, invite.invitedId)
                        val firstPlayerConnection = connections.first { it.id == invite.inviterId }
                        val secondPlayerConnection = connections.first { it.id == invite.invitedId }
                        coroutineScope.launch {gamesManager.runGame(game, firstPlayerConnection, secondPlayerConnection)}
                    }
                    "makeMove" -> {
                        val requestBody = Json.decodeFromJsonElement(APIMakeMove.serializer(), request.body)
                        val gameId = gamesManager.getGameForUser(userId)?.gameId

                        val move: APIMove = Json.decodeFromJsonElement(APIMove.serializer(), requestBody.move)
                        if (gameId != null) gamesManager.makeMove(gameId, move, userId)
                    }
                }
            }
        }
    }
}

package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.*
import kotlinx.coroutines.launch



object BodySerializer: JsonContentPolymorphicSerializer<RequestBody>(RequestBody::class) {
    override fun selectDeserializer(element: JsonElement) = when {
        "invitedId" in element.jsonObject -> APIInvite.serializer()
        "inviteId" in element.jsonObject -> APIAcceptInvite.serializer()
        "move" in element.jsonObject -> APIMakeMove.serializer()
        else -> throw Exception("Unknown body")
    }
}

class APIHandler(private val coroutineScope: CoroutineScope) {

    private val gamesManager = GamesManager()
    private val coupler = Coupler()

    suspend fun handle(frame: Frame, connection: Connection, connections: Set<Connection>) {
        when (frame) {
            is Frame.Text -> {
                val userId = connection.id
                val requestString = frame.readText()
                val request: APIRequest = Json.decodeFromString(requestString)
                val requestPath = request.path
                val requestBody: RequestBody = Json.decodeFromJsonElement(BodySerializer, request.body)

                when (requestPath) {
                    "invite" -> {
                        val invitedId = (requestBody as APIInvite).invitedId
                        val newInvite = coupler.formNewInvite(userId, invitedId)
                        if  (newInvite != null) connections.find { it.id == invitedId }?.session?.send(
                            Json.encodeToString(APIRequest("invite", Json.encodeToJsonElement(newInvite)))
                        )
                    }
                    "acceptInvite" -> {
                        val inviteId = (requestBody as APIAcceptInvite).inviteId
                        val invite = coupler.getInviteById(inviteId)
                        val game = gamesManager.createNewGame(invite.inviterId, invite.invitedId)
                        val firstPlayerConnection = connections.first { it.id == invite.inviterId }
                        val secondPlayerConnection = connections.first { it.id == invite.invitedId }
                        coroutineScope.launch {game.run(firstPlayerConnection, secondPlayerConnection)}
                    }
                    "makeMove" -> {
                        val gameId = (requestBody as APIMakeMove).gameId
                        val move = requestBody.move
                        gamesManager.makeMove(gameId, Move())
                    }
                }
            }
        }
    }
}

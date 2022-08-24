package com.xballserver.remoteserver.routing

import com.xballserver.remoteserver.game.GameProperties
import com.xballserver.remoteserver.infrastructure.Lobby
import com.xballserver.remoteserver.infrastructure.LobbyID
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonElement


@Serializable
data class APIRequest(val path: String, val body: JsonElement)

// Using String for userID since ULong serialization is not supported
@Serializable
data class APIRequestUser(val path: String, val userID: String, val body: JsonElement)

@Serializable
data class APILobby(val id: LobbyID, val maxCapacity: Int, val gameProperties: GameProperties) {
    fun toLobby(): Lobby = Lobby(
        id,
        maxCapacity,
        gameProperties
    )
}

@Serializable
data class APIMakeMove(val move: JsonElement)

@Serializable
data class APIMove(val playerId: Int, val action: String, val actionData: JsonElement)



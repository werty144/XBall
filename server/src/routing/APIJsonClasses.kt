package com.example.routing

import com.example.game.GameProperties
import com.example.infrastructure.LobbyID
import com.example.infrastructure.UserId
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonElement


@Serializable
data class APIRequest(val path: String, val body: JsonElement)

@Serializable
data class APILobby(val lobbyID: LobbyID, val gameProperties: GameProperties)

@Serializable
data class APIMakeMove(val move: JsonElement)

@Serializable
data class APIMove(val playerId: Int, val action: String, val actionData: JsonElement)


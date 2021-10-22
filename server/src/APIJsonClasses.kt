package com.example

import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonElement

@Serializable
abstract class RequestBody
@Serializable
data class APIRequest(val path: String, val body: JsonElement)

@Serializable
data class APIInvite(val invitedId: UserId): RequestBody()
@Serializable
data class APIAcceptInvite(val inviteId: InviteId): RequestBody()
@Serializable
data class APIMakeMove(val gameId: GameId, val move: String): RequestBody()
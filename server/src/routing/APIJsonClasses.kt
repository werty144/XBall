package com.example.routing

import com.example.game.Speed
import com.example.infrastructure.InviteId
import com.example.infrastructure.UserId
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.JsonElement


@Serializable
data class APIRequest(val path: String, val body: JsonElement)

@Serializable
data class APIInvite(val invitedId: UserId, val speed: Speed, val playersNumber: Int)
@Serializable
data class APIAcceptInvite(val inviteId: InviteId)
@Serializable
data class APIMakeMove(val move: JsonElement)


@Serializable
data class APIMove(val playerId: Int, val action: String, val actionData: JsonElement)


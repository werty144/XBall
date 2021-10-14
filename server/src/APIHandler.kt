package com.example

import kotlinx.serialization.json.Json
import io.ktor.http.cio.websocket.*
import kotlinx.serialization.Serializable
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.json.JsonContentPolymorphicSerializer
import kotlinx.serialization.json.JsonElement
import kotlinx.serialization.json.jsonObject

@Serializable
abstract class RequestBody
@Serializable
data class APIRequest(val path: String, val body: JsonElement)
@Serializable
data class APIInvite(val invitedId: UserId): RequestBody()
@Serializable
data class APIAcceptInvite(val inviteId: InviteId): RequestBody()

@Serializable
data class TestRequest(val a: Int, val b: String): RequestBody()

object BodySerializer: JsonContentPolymorphicSerializer<RequestBody>(RequestBody::class) {
    override fun selectDeserializer(content: JsonElement) = when {
        "invitedId" in content.jsonObject -> APIInvite.serializer()
        "inviteId" in content.jsonObject -> APIAcceptInvite.serializer()
        else -> APIInvite.serializer()
    }
}

class APIHandler {
    suspend fun handle(frame: Frame, connection: Connection, connections: Set<Connection>) {
        when (frame) {
            is Frame.Text -> {
                val requestString = frame.readText()
                val request: APIRequest = Json.decodeFromString(requestString)
                val requestPath = request.path
                val requestBody: RequestBody = Json.decodeFromJsonElement(BodySerializer, request.body)

                when (requestPath) {
                    "invite" -> {
                        connection.session.send((requestBody as APIInvite).invitedId.toString())
                    }
                }
            }
        }
    }
}

fun gameInvite(inviterId: UserId, invitedId: UserId, coupler: Coupler) {
    coupler.formNewInvite(inviterId, invitedId)
}
package com.example.infrastructure

import com.example.routing.Connection
import com.example.routing.Connections
import io.ktor.http.cio.websocket.*

class AuthenticationManager {
    private val connectionIdToUserId = mutableMapOf<Int, UserId>()

    fun userIdByToken(token: String): UserId {
        return token.removeSuffix("_salt").toInt()
    }

    fun validate_token(token: String): Boolean {
        return token.endsWith("_salt")
    }

    fun mapConnectionToUser(connectionId: Int, userId: UserId) {
        connectionIdToUserId[connectionId] = userId
    }

    fun getUserIdByConnectionId(connectionId: Int) = connectionIdToUserId[connectionId]!!

    fun processFirstMessage(
        frame: Frame,
        thisConnection: Connection
    ): Boolean {
        thisConnection.firstMessageReceived = true
        return if (frame !is Frame.Text || !validate_token(frame.readText())) {
            false
        } else {
            mapConnectionToUser(
                thisConnection.id,
                userIdByToken(frame.readText())
            )
            true
        }

    }
}

@kotlinx.serialization.Serializable
data class UserCredentials(val id: String)
package com.example.infrastructure

import com.example.routing.Connection
import io.ktor.http.cio.websocket.*

class AuthenticationManager {
    fun userIdByToken(token: String): UserId {
        return token.removeSuffix("_salt").toInt()
    }

    fun validateToken(token: String): Boolean {
        return token.endsWith("_salt")
    }

    fun validateFirstMessage(frame: Frame) = frame is Frame.Text && validateToken(frame.readText())
}

@kotlinx.serialization.Serializable
data class UserCredentials(val id: String)
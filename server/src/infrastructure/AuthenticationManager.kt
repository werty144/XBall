package com.example.infrastructure

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
}
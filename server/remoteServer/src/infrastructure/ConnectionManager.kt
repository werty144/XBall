package com.xballserver.remoteserver.infrastructure

import com.xballserver.remoteserver.routing.Connection
import com.xballserver.remoteserver.routing.Connections
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.util.*
import kotlin.collections.LinkedHashSet

open class ConnectionManager {
    val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())

    fun addConnection(connection: Connection) {
        connections.add(connection)
    }

    open suspend fun sendMessage(userId: UserId, message: String) {
        val userConnection = connections.find { it.userId == userId }
        if (isActiveConnection(userConnection)) {
            userConnection!!.session.send(message)
        }
    }

    fun isActiveConnection(connection: Connection?): Boolean {
        return (connection != null) && (connection.session.isActive)
    }

    fun clean() {
        connections.removeIf { !isActiveConnection(it) }
    }
}

class LocalConnectionManager: ConnectionManager() {
    override suspend fun sendMessage(userId: UserId, message: String) {
        val connectionToHost = connections.find { it.userId == 0UL } ?: return
        connectionToHost.session.send("${userId}\n" + message)
    }
}
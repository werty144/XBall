package com.example.routing

import com.example.infrastructure.InvitesManager
import com.example.infrastructure.GamesManager
import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import io.ktor.routing.*
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.util.concurrent.atomic.AtomicInteger

typealias Connections = MutableSet<Connection>

fun Application.configureRouting(gamesManager: GamesManager, invitesManager: InvitesManager, connections: Connections) {

    val apiHandler = APIHandler(this, gamesManager, invitesManager, connections)

    routing {
        webSocket("/") {
            val thisConnection = Connection(this)
            connections += thisConnection
            thisConnection.session.isActive
            send("${thisConnection.id}")
            for (frame in incoming) {
                apiHandler.handle(frame, thisConnection)
            }
        }
    }
}

class Connection(val session: DefaultWebSocketSession) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()

    override fun toString(): String = "Connection(id=$id, active=${session.isActive})"
}
package com.example.routing

import com.example.infrastructure.*
import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.util.concurrent.atomic.AtomicInteger

typealias Connections = MutableSet<Connection>

fun Application.configureRouting(gamesManager: GamesManager,
                                 invitesManager: InvitesManager,
                                 authenticationManager: AuthenticationManager,
                                 connections: Connections) {

    val apiHandler = APIHandler(this, gamesManager, invitesManager, connections)

    routing {
        post("/auth") {
            val credentials = call.receive<UserCredentials>()
            val superSecretToken = credentials.id + "_salt"
            call.respond(superSecretToken.toByteArray())
        }
        webSocket("/") {
            val thisConnection = Connection(this)
            connections += thisConnection
            for (frame in incoming) {
                if (!thisConnection.firstMessageReceived) {
                    if (!authenticationManager.processFirstMessage(frame, thisConnection)) {
                        close()
                        connections.remove(thisConnection)
                    }
                } else {
                    apiHandler.handle(frame, authenticationManager.getUserIdByConnectionId(thisConnection.id))
                }
            }
        }
    }
}

class Connection(val session: DefaultWebSocketSession) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()
    var firstMessageReceived = false

    override fun toString(): String = "Connection(id=$id, active=${session.isActive})"
}

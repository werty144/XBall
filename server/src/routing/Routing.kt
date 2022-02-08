package com.example.routing

import com.example.infrastructure.InvitesManager
import com.example.infrastructure.GamesManager
import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.util.concurrent.atomic.AtomicInteger

typealias Connections = MutableSet<Connection>

fun Application.configureRouting(gamesManager: GamesManager, invitesManager: InvitesManager, connections: Connections) {

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
            thisConnection.session.isActive
            for (frame in incoming) {
                if (!thisConnection.firstMessageReceived) {
                    if (frame !is Frame.Text || !validate_token(frame.readText())) {
                        close()
                        connections.remove(thisConnection)
                    }
                    thisConnection.firstMessageReceived = true
                } else {
                    apiHandler.handle(frame, thisConnection)
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

fun validate_token(token: String): Boolean {
    return token.endsWith("_salt\n")
}

@kotlinx.serialization.Serializable
data class UserCredentials(val id: String)
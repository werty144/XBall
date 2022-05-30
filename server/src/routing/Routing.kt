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

    val apiHandler = APIHandler(gamesManager, invitesManager, connections)

    routing {
        post("/auth") {
            val credentials = call.receive<UserCredentials>()
            val superSecretToken = credentials.id + "_salt"
            call.respond(superSecretToken.toByteArray())
        }
        webSocket("/") {
            var firstMessageReceived = false
            var thisConnection: Connection? = null
            for (frame in incoming) {
                if (!firstMessageReceived) {
                    firstMessageReceived = true
                    if (!authenticationManager.validateFirstMessage(frame)) {
                        close()
                        return@webSocket
                    } else {
                        val token = (frame as Frame.Text).readText()
                        thisConnection = Connection(this, authenticationManager.userIdByToken(token))
                        connections += thisConnection
                    }
                } else {
                    apiHandler.handle(frame, thisConnection!!)
                }
            }
        }

        get("/stop_games") {
            gamesManager.stopAll()
        }
    }
}

class Connection(val session: DefaultWebSocketSession, val userId: UserId) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()

    override fun toString(): String = "Connection(id=$id, active=${session.isActive})"
}

package com.example.routing

import com.example.infrastructure.*
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.request.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlinx.coroutines.isActive
import java.util.concurrent.atomic.AtomicInteger

typealias Connections = MutableSet<Connection>

fun Application.configureRouting(gamesManager: GamesManager,
                                 lobbyManager: LobbyManager,
                                 authenticationManager: AuthenticationManager,
                                 connections: Connections) {

    val apiHandler = APIHandler(gamesManager, lobbyManager, connections)

    routing {
        post("/auth") {
            val credentials = call.receive<UserCredentials>()
            val steamId = authenticationManager.validateSteamTicket(credentials.ticket) ?: return@post
            val superSecretPassword = authenticationManager.generatePasswordForUser(steamId)
            call.response.status(HttpStatusCode.OK)
            call.respond(superSecretPassword.toByteArray())
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
                        val password = (frame as Frame.Text).readText()
                        thisConnection = Connection(this, authenticationManager.userId(password)!!)
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

        get("/test") {
            call.respond("Poshel nahuy")
        }
    }
}

class Connection(val session: DefaultWebSocketSession, val userId: UserId) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()

    override fun toString(): String = "Connection(userId=$userId, active=${session.isActive})"
}

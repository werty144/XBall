package com.xballserver.localserver

import com.xballserver.remoteserver.game.GameProperties
import com.xballserver.remoteserver.infrastructure.*
import com.xballserver.remoteserver.routing.*
import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlinx.serialization.json.JsonElement


fun Application.configureRouting(gamesManager: GamesManager,
                                 connectionManager: ConnectionManager,
                                 gameStartManager: GameStartManager
)
{
    val apiHandler = APIHandler(gamesManager, gameStartManager)
    routing {
        webSocket("/") {
            connectionManager.addConnection(Connection(this, 0U))
            for (frame in incoming) {
                if (frame is Frame.Text) {
                    val message = frame.readText()
                    val request = tryJsonParse(APIRequestUser.serializer(), message) ?: continue
                    logToFile("Request: $request")
                    val userID = request.userID.toULong()
                    apiHandler.handle(request.path, userID, request.body)
                } else {
                    continue
                }
            }
        }

        get("/test") {
            call.respondText("Privet")
            call.respond(HttpStatusCode.OK)
        }
    }
}



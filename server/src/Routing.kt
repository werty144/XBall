package com.example

import io.ktor.application.*
import io.ktor.http.*
import io.ktor.http.cio.websocket.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*
import io.ktor.websocket.*
import kotlinx.coroutines.launch
import java.util.*
import java.util.concurrent.atomic.AtomicInteger
import kotlin.collections.LinkedHashSet


fun Application.configureRouting() {

    val gamesManager = GamesManager()
    val coupler = Coupler()
    val apiHandler = APIHandler()

    routing {
        val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())
        webSocket("/chat") {
            val thisConnection = Connection(this)
            connections += thisConnection
            send("You've logged in as [${thisConnection.name}]")
            for (frame in incoming) {
                when (frame) {
                    is Frame.Text -> {
                        val receivedText = frame.readText()
                        val textWithUsername = "[${thisConnection.name}]: $receivedText"
                        connections.forEach {
                            it.session.send(textWithUsername)
                        }
                    }
                }
            }
        }

        webSocket("/") {
            val thisConnection = Connection(this)
            connections += thisConnection
            send("You've logged in as [${thisConnection.name}]")
            for (frame in incoming) {
                apiHandler.handle(frame, thisConnection, connections)
            }
        }

        get("/getUserId") {
            call.respondText(coupler.getNewUserId().toString())
        }

//        post("/checkInvites") {
//            val requestParams = call.receiveParameters()
//            val userId = requestParams["user_id"]!!.toInt()
//            val invitesForUser = coupler.getInvitesForUser(userId)
//            call.respond(Gson().toJson(invitesForUser))
//        }

        post("/acceptInvite") {
            val requestParams = call.receiveParameters()
            val userId = requestParams["user_id"]!!.toInt()
            val inviteId = requestParams["invite_id"]!!.toInt()
            val inviterId = coupler.getInviterId(userId, inviteId)

            if (inviterId == null) {
                call.respond(HttpStatusCode.BadRequest)
                return@post
            }

            val game = gamesManager.createNewGame(inviterId, userId)
            launch {game.run()}
            call.respond(HttpStatusCode.OK)
        }

//        post("/checkGames") {
//            val requestParams = call.receiveParameters()
//            val userId = requestParams["user_id"]!!.toInt()
//            val gamesForUser = gamesManager.getGamesForUser(userId)
//            call.respond(Gson().toJson(gamesForUser))
//        }

        post("/getGameState") {
            val requestParams = call.receiveParameters()
            val gameId = requestParams["game_id"]!!.toInt()
            val gameState = gamesManager.gameSateById(gameId)
            call.respondText(gameState.toString())
        }

        post("/setGameState") {
            val requestParams = call.receiveParameters()
            val gameId = requestParams["game_id"]!!.toInt()
            val newState = requestParams["new_state"]!!.toInt()
            gamesManager.setState(gameId, newState)
            call.respond(HttpStatusCode.OK)
        }
    }
}

class Connection(val session: DefaultWebSocketSession) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val name = "user${lastId.getAndIncrement()}"
}
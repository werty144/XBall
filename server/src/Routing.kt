package com.example

import com.google.gson.Gson
import io.ktor.application.*
import io.ktor.http.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*
import kotlinx.coroutines.launch

fun Application.configureRouting() {

    val gamesManager = GamesManager()
    val coupler = Coupler()

    routing {
        get("/getUserId") {
            call.respondText(coupler.getNewUserId().toString())
        }

        post("/gameInvite") {
            val requestParams = call.receiveParameters()
            val inviterId = requestParams["inviter_id"]!!.toInt()
            val invitedId = requestParams["invited_id"]!!.toInt()
            coupler.formNewInvite(inviterId, invitedId)
            call.respond(HttpStatusCode.Created)
        }

        post("/checkInvites") {
            val requestParams = call.receiveParameters()
            val userId = requestParams["user_id"]!!.toInt()
            val invitesForUser = coupler.getInvitesForUser(userId)
            call.respond(Gson().toJson(invitesForUser))
        }

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

        post("/checkGames") {
            val requestParams = call.receiveParameters()
            val userId = requestParams["user_id"]!!.toInt()
            val gamesForUser = gamesManager.getGamesForUser(userId)
            call.respond(Gson().toJson(gamesForUser))
        }

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

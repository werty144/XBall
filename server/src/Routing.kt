package com.example

import io.ktor.application.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*

fun Application.configureRouting() {

    val gamesManager = GamesManager()

    routing {
        get("/newGame") {
            val game = gamesManager.createNewGame()
            call.respondText(game.id.toString() + '\n')
            game.run()
        }

        post("/gameState") {
            val gameId = call.receive<String>().toInt()
            val game = gamesManager.gameById(gameId)
            call.respondText(game.toString() + '\n')
        }
    }
}

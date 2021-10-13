package com.example

import io.ktor.application.*
import io.ktor.request.*
import io.ktor.response.*
import io.ktor.routing.*
import kotlinx.coroutines.launch

fun Application.configureRouting() {

    val gamesManager = GamesManager()

    routing {
        get("/newGame") {
            val game = gamesManager.createNewGame()
            call.respondText(game.id.toString() + '\n')
            launch {game.run()}
        }

        post("/gameState") {
            val gameId = call.receive<String>().toInt()
            val game = gamesManager.gameById(gameId)
            call.respondText(game.toString() + '\n')
        }
    }
}

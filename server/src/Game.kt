package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.delay

typealias GameId = Int

data class Game(val id: GameId, val player1Id: UserId, val player2Id: UserId) {

    var state = 0

    suspend fun run(firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(1000)
            firstPlayerConnection.session.send(toString())
            secondPlayerConnection.session.send(toString())
        }
    }

    override fun toString() = "id: $id, players: ($player1Id, $player2Id), state: $state"
}
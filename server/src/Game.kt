package com.example

import kotlinx.coroutines.delay

typealias GameId = Int

data class Game(val id: GameId, val player1Id: UserId, val player2Id: UserId) {

    var state = 0

    suspend fun run() {
        while (true) {
            delay(1000)
            println(toString())
        }
    }

    override fun toString() = "id: $id, players: ($player1Id, $player2Id), state: $state"
}
package com.example

import kotlinx.coroutines.delay

typealias GameId = Int

data class Game(val id: GameId) {

    var ticks = 0

    suspend fun run() {
        while (true) {
            delay(1000)
            ticks++
            print("Ticked $id\n")
        }
    }

    override fun toString() = "id: $id, ticks: $ticks\n"
}
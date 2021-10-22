package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.delay
import kotlinx.serialization.Serializable
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.encodeToJsonElement

typealias GameId = Int

@Serializable
data class Game(val gameId: GameId, val player1Id: UserId, val player2Id: UserId, var state: Int) {

//    var state = 0

    suspend fun run(firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(10)
            val message = Json.encodeToString(APIRequest("gameState",Json.encodeToJsonElement(this)))
            firstPlayerConnection.session.send(message)
            secondPlayerConnection.session.send(message)
        }
    }

    override fun toString() = "id: $gameId, players: ($player1Id, $player2Id), state: $state"
}
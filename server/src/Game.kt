package com.example

import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.delay
import kotlinx.serialization.Serializable
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.encodeToJsonElement

typealias GameId = Int

@Serializable
data class Game(val gameId: GameId, val player1Id: UserId, val player2Id: UserId, val gameProperties: GameProperties) {
    val state: GameState
    init {
        val p1State = PlayerState(0, 75, 0, 0, UnitVector(1.0F, 0.0F))
        val p1 = Player(0, player1Id, p1State)
        val p2State = PlayerState(300, 75, 0, 0, UnitVector(1.0F, 0.0F))
        val p2 = Player(1, player2Id, p2State)
        val players = listOf(p1, p2)
        val ballState = BallState(150, 75, 0, 0, UnitVector(1.0F, 0.0F))
        state = GameState(players, ballState)
    }

    suspend fun run(firstPlayerConnection: Connection, secondPlayerConnection: Connection) {
        while (true) {
            delay(10)
            nextState()
            val message = Json.encodeToString(APIRequest("gameState",Json.encodeToJsonElement(state)))
            firstPlayerConnection.session.send(message)
            secondPlayerConnection.session.send(message)
        }
    }

    fun makeMove(move: Move){}

    fun nextState() {}
}

class Move

enum class Speed {SLOW, NORM, FAST}

@Serializable
data class GameProperties(
    val playersN: Int,
    val speed: Speed
) {
    val width = 300
    val height = 150
    val playerSpeed = 70
    val ballSpeed = 150
}

@Serializable
data class Point(val x: Int, val y: Int)

@Serializable
class UnitVector{
    val x: Float
    val y: Float
    constructor(x: Float, y: Float) {
        val vectorLength = kotlin.math.sqrt(x * x + y * y)
        this.x = x / vectorLength
        this.y = y / vectorLength
    }
    constructor(p1: Point, p2: Point) {
        val xDiff = kotlin.math.abs(p1.x - p2.x).toFloat()
        val yDiff = kotlin.math.abs(p1.y - p2.y).toFloat()
        val vectorLength = kotlin.math.sqrt(xDiff * xDiff + yDiff * yDiff)
        this.x = xDiff / vectorLength
        this.y = yDiff / vectorLength
    }
}


@Serializable
data class PlayerState(var x: Int, var y: Int, var z: Int = 0, var speed: Int, val direction: UnitVector)

@Serializable
data class Player(val id: Int, val teamUser: UserId, val state: PlayerState)

@Serializable
data class BallState(var x: Int, var y: Int, var z: Int = 0, var speed: Int, val direction: UnitVector)

@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)
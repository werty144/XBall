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
        val p1State = PlayerState(0.0F, 75.0F, 0.0F, 0.0F)
        val p1 = Player(0, player1Id, p1State)
        val p2State = PlayerState(300.0F, 75F, 0F, 0F)
        val p2 = Player(1, player2Id, p2State)
        val players = listOf(p1, p2)
        val ballState = BallState(150F, 75F, 0F, 0F, Vector(1.0F, 0.0F))
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

    fun makeMove(move: APIMove) {
        when (move.action) {
            "movement" -> {
                val destination = Json.decodeFromJsonElement(APIMovementMove.serializer(), move.actionData)
                state.players.find { it.id == move.playerId }?.state?.destination = Point(destination.x, destination.y)
            }
        }
    }

    fun nextState() {
        state.players.forEach {
            if (it.state.destination != null) {
                val destination = it.state.destination!!
                val position = Point(it.state.x, it.state.y)
                val orientation = Vector(position, destination).unit()
                val step = orientation * gameProperties.playerSpeed
                if (distance(destination, position) <= step.length()) {
                    it.state.x = destination.x
                    it.state.y = destination.y
                    it.state.destination = null
                } else {
                    val nextPoint = position + step
                    it.state.x = nextPoint.x
                    it.state.y = nextPoint.y
                }
            }
        }
    }
}

enum class Speed {SLOW, NORM, FAST}

@Serializable
data class GameProperties(
    val playersN: Int,
    val speed: Speed
) {
    val width = 300
    val height = 150
    val playerSpeed = 4
    val ballSpeed = 150
}

@Serializable
data class Point(val x: Float, val y: Float) {
    operator fun plus(vector: Vector): Point {
        return Point(x + vector.x, y + vector.y)
    }
}

fun distance(p1: Point, p2: Point) = Vector(p1, p2).length()

@Serializable
class Vector{
    val x: Float
    val y: Float

    constructor(x: Float, y: Float) { this.x =  x; this.y = y }

    constructor(p1: Point, p2: Point) {
        val xDiff = p2.x - p1.x
        val yDiff = p2.y - p1.y
        this.x = xDiff.toFloat()
        this.y = yDiff.toFloat()
    }

    fun unit(): Vector = Vector(x / length(), y / length())

    operator fun times(n: Int): Vector = Vector(x * n, y * n)

    fun length(): Float = kotlin.math.sqrt(x * x + y * y)

}


@Serializable
data class PlayerState(var x: Float, var y: Float, var z: Float = 0.0F, var speed: Float, var destination: Point? = null)

@Serializable
data class Player(val id: Int, val teamUser: UserId, val state: PlayerState)

@Serializable
data class BallState(var x: Float, var y: Float, var z: Float = 0.0F, var speed: Float, val direction: Vector)

@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)
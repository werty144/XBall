package com.example

import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.Json
import kotlin.math.*

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

    fun makeMove(move: APIMove) {
        when (move.action) {
            "movement" -> {
                val destination = Json.decodeFromJsonElement(APIMovementMove.serializer(), move.actionData)
                state.players.find { it.id == move.playerId }?.state?.destination = Point(destination.x, destination.y)
            }
        }
    }

    fun nextState(updateTime: Long) {
        state.players.forEach {
            it.move(gameProperties, updateTime)
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
    val playerSpeed = 60
    val ballSpeed = 150
    val playerRotationSpeed = 2*PI
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
        this.x = xDiff
        this.y = yDiff
    }

    fun unit(): Vector = Vector(x / length(), y / length())

    operator fun times(n: Float): Vector = Vector(x * n, y * n)

    fun length(): Float = sqrt(x * x + y * y)

    fun angle(): Float = atan2(y, x)

    fun rotated(angle: Float): Vector = Vector(x * cos(angle) - y * sin(angle), x * sin(angle) + y * cos(angle))

    fun orientedAngleWithVector(v: Vector): Float = atan2(x*v.y - y*v.x, x*v.x + y*v.y)

}


@Serializable
data class PlayerState(
    var x: Float,
    var y: Float,
    @Required
    var z: Float = 0F,
    var speed: Float = 0F,
    var destination: Point? = null,
    var orientation: Vector = Vector(1F, 0F)
) {
    @Required
    var rotationAngle: Float = 0F
        get() = orientation.angle()
}

@Serializable
data class Player(val id: Int, val teamUser: UserId, val state: PlayerState) {
    fun move(gameProperties: GameProperties, updateTime: Long) {
        if (state.destination != null) {
            val destination = state.destination!!
            val position = Point(state.x, state.y)
            val orientation = Vector(position, destination).unit()

            val step = orientation * (gameProperties.playerSpeed / (1000F / updateTime))
            if (distance(destination, position) <= step.length()) {
                state.x = destination.x
                state.y = destination.y
                state.destination = null
            } else {
                val nextPoint = position + step
                state.x = nextPoint.x
                state.y = nextPoint.y
            }

            val angleDiff = state.orientation.orientedAngleWithVector(orientation)
            val maxAngle = (gameProperties.playerRotationSpeed / (1000F / updateTime)).toFloat()
            if (abs(angleDiff) <= maxAngle) {
                state.orientation = state.orientation.rotated(angleDiff)
            } else {
                state.orientation = state.orientation.rotated(sign(angleDiff) * maxAngle)
            }
        }
    }
}

@Serializable
data class BallState(var x: Float, var y: Float, var z: Float = 0F, var speed: Float, val direction: Vector)

@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)
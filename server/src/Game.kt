package com.example

import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.Json
import kotlin.math.*

typealias GameId = Int

@Serializable
data class Game(val gameId: GameId, val player1Id: UserId, val player2Id: UserId, val properties: GameProperties) {
    val state: GameState

    init {
        val p1State = PlayerState(0.0F, 75.0F, 0.0F, 0.0F)
        val p1 = Player(0, player1Id, p1State)
        val p2State = PlayerState(300.0F, 75F, 0F, 0F)
        val p2 = Player(1, player2Id, p2State)
        val players = listOf(p1, p2)
        val ballState = BallState(150F, 75F)
        state = GameState(players, ballState)
    }

    fun makeMove(move: APIMove, actorId: UserId) {
        if (!validateMove(move, actorId)) {
            return
        }
        when (move.action) {
            "movement" -> {
                val destination = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
                state.players.find { it.id == move.playerId }?.state?.destination = Point(destination.x, destination.y)
            }
            "grab" -> {
                val player = state.players.find { it.id == move.playerId }!!
                if (distance(player.state.position, state.ballState.position) <= properties.grabRadius) {
                    state.ballState.ownerId = player.id
                    state.ballState.destination = null
                }
            }
            "throw" -> {
                if (state.ballState.ownerId != move.playerId) return

                val destination = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
                state.ballState.ownerId = null
                state.ballState.destination = destination
            }
        }
    }

    fun nextState(updateTime: Long) {
        state.players.forEach {
            it.move(this, updateTime)
        }

        state.ballState.update(this, updateTime)
    }

    fun validateMove(move: APIMove, actorId: UserId): Boolean {
        val player = state.players.find { it.id == move.playerId }
        return if (player == null) {
            false
        } else {
            player.teamUser == actorId
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
    val playerRadius = 5
    val ballRadius = 3
    val grabRadius = 15

    fun pointWithinField(point: Point): Boolean {
        return (0 <= point.x) and (point.x <= width) and (0 <= point.y) and (point.y <= height)
    }

    fun playersIntersectIfPlacedTo(position1: Point, position2: Point): Boolean {
        return distance(position1, position2) < 2 * playerRadius
    }
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

    var position: Point = Point(x, y)
        get() = Point(x, y)
}

@Serializable
data class Player(val id: Int, val teamUser: UserId, val state: PlayerState) {
    fun move(game: Game, updateTime: Long) {
        val gameProperties = game.properties

        if (state.destination != null) {
            val destination = state.destination!!
            val position = Point(state.x, state.y)
            val orientation = Vector(position, destination).unit()

            val step = orientation * (gameProperties.playerSpeed / (1000F / updateTime))
            if (distance(destination, position) <= step.length()) {
                if (canMove(destination, game)) {
                    state.x = destination.x
                    state.y = destination.y
                    state.destination = null
                }
            } else {
                val nextPoint = position + step
                if (canMove(nextPoint, game)) {
                    state.x = nextPoint.x
                    state.y = nextPoint.y
                }
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

    fun canMove(target: Point, game: Game): Boolean {
        return game.properties.pointWithinField(target) and
                game.state.players.all {
                    (it.id == id) or
                            (!game.properties.playersIntersectIfPlacedTo(it.state.position, target))
                }
    }
}

@Serializable
data class BallState(var x: Float, var y: Float) {
    var z: Float = 0F
    var ownerId: Int? = null
    var destination: Point? = null

    var position: Point = Point(0F, 0F)
        get() = Point(x, y)

    fun update(game: Game, updateTime: Long) {
        if (ownerId != null) {
            val player = game.state.players.find { it.id == ownerId }!!
            val position = player.state.position +
                    player.state.orientation.unit() *
                    (game.properties.playerRadius + game.properties.ballRadius).toFloat()
            x = position.x
            y = position.y
        }

        if (destination != null) {
            val destination = destination!!
            if (!game.properties.pointWithinField(destination)) return

            val position = Point(x, y)
            val orientation = Vector(position, destination).unit()

            val step = orientation * (game.properties.ballSpeed / (1000F / updateTime))
            if (distance(destination, position) <= step.length()) {
                    x = destination.x
                    y = destination.y
                    this.destination = null
            } else {
                val nextPoint = position + step
                x = nextPoint.x
                y = nextPoint.y
            }
        }
    }
}

@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)
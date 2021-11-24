package com.example.game

import com.example.routing.APIMove
import com.example.infrastructure.UserId
import kotlinx.serialization.Serializable
import kotlinx.serialization.json.Json
import kotlin.math.*

typealias GameId = Int

@Serializable
data class Game(val gameId: GameId, val player1Id: UserId, val player2Id: UserId, val properties: GameProperties) {
    val state: GameState

    init {
        val p1State = PlayerState(0.0F, 75.0F, 0.0F)
        val p1 = Player(0, player1Id, p1State)
        val p2State = PlayerState(300.0F, 75F, 0F)
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
                val positionTarget = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
                state.players.find { it.id == move.playerId }?.state?.positionTarget = positionTarget
            }
            "orientation" -> {
                val orientationTarget = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
                state.players.find { it.id == move.playerId }?.state?.orientationTarget = orientationTarget
            }
            "grab" -> {
                grabRandom(this, move)
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
            it.nextState(this, updateTime)
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
data class GameState(val players: List<Player>, val ballState: BallState)
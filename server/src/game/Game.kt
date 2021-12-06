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
        val players = ArrayList<Player>()
        var sparePlayersId = 0
        for (i in 1..properties.playersNumber) {
            players.add(Player(sparePlayersId++, player1Id,
                PlayerState(
                    properties.fieldWidth * 1/4,
                    properties.fieldHeight / (properties.playersNumber + 1) * i,
                    orientation = Vector(1F, 0F)
                )
            ))
            players.add(Player(sparePlayersId++, player2Id,
                PlayerState(
                    properties.fieldWidth * 3/4,
                    properties.fieldHeight / (properties.playersNumber + 1) * i,
                    orientation = Vector(-1F, 0F)
                )
            ))
        }
        val ballState = BallState(properties.fieldWidth / 2, properties.fieldHeight / 2)
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
                throwRandom(this, move)
            }
        }
    }

    fun nextState(updateTime: Long) {
        val speedAdjustedUpdateTime = updateTime * properties.speed.slowingCoefficient()
        state.players.forEach {
            it.nextState(this, speedAdjustedUpdateTime)
        }

        state.ballState.update(this, speedAdjustedUpdateTime)
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

enum class Speed {
    SLOW,
    NORMAL,
    FAST;

    fun slowingCoefficient(): Float {
        return when (this) {
            SLOW -> 0.25F
            NORMAL -> 0.5F
            FAST -> 1F
        }
    }
}


@Serializable
data class GameProperties(
    val playersNumber: Int,
    val speed: Speed
) {
    val fieldWidth = 300F
    val fieldHeight = 150F
    val playerSpeed = 60
    val ballSpeed = 150
    val playerRotationSpeed = 2*PI
    val playerRadius = 5
    val ballRadius = 3
    val grabRadius = 15

    fun pointWithinField(point: Point): Boolean {
        return (0 <= point.x) and (point.x <= fieldWidth) and (0 <= point.y) and (point.y <= fieldHeight)
    }

    fun clipPointToField(point: Point): Point =
        Point(
            max(0F, min(point.x, fieldWidth.toFloat())),
            max(0F, min(point.y, fieldHeight.toFloat()))
        )

    fun playersIntersectIfPlacedTo(position1: Point, position2: Point): Boolean {
        return distance(position1, position2) < 2 * playerRadius
    }
}


@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)
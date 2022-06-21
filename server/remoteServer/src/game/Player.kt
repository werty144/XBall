package com.example.game

import com.xballserver.remoteserver.routing.APIMove
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.encodeToJsonElement
import java.sql.Timestamp
import kotlin.math.abs
import kotlin.math.sign

@Serializable
data class PlayerState(
    var x: Double,
    var y: Double,
    @Required
    var z: Double = 0.0,
    @Transient
    var orientation: Vector = Vector(1.0, 0.0)
) {
    @Transient
    var positionTarget: Point? = null

    @Transient
    var orientationTarget: Point? = null

    @Required
    var rotationAngle: Double = 0.0
        get() = orientation.angle()

    @Transient
    var position: Point = Point(x, y)
        get() = Point(x, y)
}

@Serializable
data class Player(val id: Int, val side: Side, val state: PlayerState) {
    @Transient
    var lastTimeBallOwner: Timestamp = Timestamp(System.currentTimeMillis())


    fun nextState(game: Game) {
        manageGrab(game)
        move(game)
        rotate(game)
    }

    fun rotate(game: Game) {
        var direction: Vector? = null
        if ((state.positionTarget != null) and (state.positionTarget != state.position)) {
            direction = Vector(state.position, state.positionTarget!!).unit()
        } else {
            if ((state.orientationTarget != null) and (state.orientationTarget != state.position)) {
                direction = Vector(state.position, state.orientationTarget!!).unit()
            }
        }

        if (direction != null) {
            val angleDiff = state.orientation.orientedAngleWithVector(direction)
            val maxAngle = (game.properties.playerRotationSpeed * game.deltaTime)
            if (abs(angleDiff) <= maxAngle) {
                state.orientation = state.orientation.rotated(angleDiff)
                state.orientationTarget = null
            } else {
                state.orientation = state.orientation.rotated(sign(angleDiff) * maxAngle)
            }
        }
    }

    fun move(game: Game) {
        if ((state.positionTarget != null) and (state.positionTarget != state.position)) {
            val positionTarget = state.positionTarget!!
            val direction = Vector(state.position, positionTarget).unit()

            val step = direction * (game.properties.playerSpeed * game.deltaTime)
            if (distance(positionTarget, state.position) <= step.length()) {
                if (canMove(positionTarget, game)) {
                    state.x = positionTarget.x
                    state.y = positionTarget.y
                    state.positionTarget = null
                }
            } else {
                val nextPoint = state.position + step
                if (canMove(nextPoint, game)) {
                    state.x = nextPoint.x
                    state.y = nextPoint.y
                }
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

    fun manageGrab(game: Game) {
        if (game.state.ballState.ownerId == id) {
            lastTimeBallOwner = Timestamp(System.currentTimeMillis())
            return
        }

        val ballOwner = game.getBallOwner()
        if ((ballOwner != null) && (ballOwner.side == side)) return

        val currentTimeStamp = Timestamp(System.currentTimeMillis())
        if (currentTimeStamp.time - lastTimeBallOwner.time < game.properties.grabCoolDown) return

        grabRandom(game, APIMove(id, "grab", Json.encodeToJsonElement("")))
    }
}
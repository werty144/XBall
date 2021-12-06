package com.example.game

import com.example.infrastructure.UserId
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient
import kotlin.math.abs
import kotlin.math.sign

@Serializable
data class PlayerState(
    var x: Float,
    var y: Float,
    @Required
    var z: Float = 0F,
    @Transient
    var orientation: Vector = Vector(1F, 0F)
) {
    @Transient
    var positionTarget: Point? = null

    @Transient
    var orientationTarget: Point? = null

    @Required
    var rotationAngle: Float = 0F
        get() = orientation.angle()

    @Transient
    var position: Point = Point(x, y)
        get() = Point(x, y)
}

@Serializable
data class Player(val id: Int, val teamUser: UserId, val state: PlayerState) {
    fun nextState(game: Game, updateTime: Float) {
        move(game, updateTime)
        rotate(game, updateTime)
    }

    fun rotate(game: Game, updateTime: Float) {
        var direction: Vector? = null
        if (state.positionTarget != null) {
            direction = Vector(state.position, state.positionTarget!!).unit()
        } else {
            if (state.orientationTarget != null) {
                direction = Vector(state.position, state.orientationTarget!!).unit()
            }
        }

        if (direction != null) {
            val angleDiff = state.orientation.orientedAngleWithVector(direction)
            val maxAngle = (game.properties.playerRotationSpeed / (1000F / updateTime)).toFloat()
            if (abs(angleDiff) <= maxAngle) {
                state.orientation = state.orientation.rotated(angleDiff)
                state.orientationTarget = null
            } else {
                state.orientation = state.orientation.rotated(sign(angleDiff) * maxAngle)
            }
        }
    }

    fun move(game: Game, updateTime: Float) {
        if (state.positionTarget != null) {
            val destination = state.positionTarget!!
            val direction = Vector(state.position, destination).unit()

            val step = direction * (game.properties.playerSpeed / (1000F / updateTime))
            if (distance(destination, state.position) <= step.length()) {
                if (canMove(destination, game)) {
                    state.x = destination.x
                    state.y = destination.y
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
}
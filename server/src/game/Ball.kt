package com.example.game

import io.ktor.sessions.*
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient

@Serializable
data class BallState(var x: Float, var y: Float) {
    @Required
    var z: Float = 0F
    var active = true
    var ownerId: Int? = null
    var destination: Point? = null
    var flyMode: Boolean = false

    @Transient
    var position: Point = Point(0F, 0F)
        get() = Point(x, y)

    @Transient
    var orientation: Vector? = null
        get() = destination?.let {
            if (position == destination) {
                null
            } else {
                Vector(position, it).unit()
            }
        }

    fun update(game: Game) {
        if (ownerId != null) {
            val player = game.state.players.find { it.id == ownerId }!!
            val position = player.state.position +
                    player.state.orientation.unit() *
                    (game.properties.playerRadius + game.properties.ballRadius).toFloat()
            x = position.x
            y = position.y
            return
        }

        if (flyMode and (z != game.properties.flyHeight)) {
            z += kotlin.math.min(game.properties.flyHeight - z, 2F)
            return
        }


        if (destination != null) {
            val destination = destination!!
            val nextPoint = nextPoint(game)

            if (active and intersectTarget(game, nextPoint)) {
                targetAttempt(game)
                return
            }

            if (distance(position, destination) <= distance(position, nextPoint)) {
                x = destination.x
                y = destination.y
                flyMode = false
                this.destination = null
                return
            }

            x = nextPoint.x
            y = nextPoint.y
        } else {
            if (!flyMode) {
                if (z != 0F) {
                    z -= kotlin.math.min(z, 2F)
                } else {
                    flyMode = false
                }
            }
        }
    }

    fun nextPoint(game: Game): Point {
        if (destination == null) throw IllegalStateException("Next step called but no destination")
        if (destination == position) return position

        val orientation = Vector(position, destination!!).unit()
        val step = orientation * (game.properties.ballSpeed / (1000F / game.gameUpdateTime))
        return position + step
    }

    fun intersectTarget(game: Game, nextPoint: Point): Boolean {
        return flyMode and Side.values().any {
            distance(game.properties.targetPoint(it), nextPoint) <
                game.properties.targetRadius + game.properties.ballRadius
        }
    }
}
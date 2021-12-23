package com.example.game

import io.ktor.sessions.*
import kotlinx.serialization.Serializable

@Serializable
data class BallState(var x: Float, var y: Float) {
    var z: Float = 0F
    var active = true
    var ownerId: Int? = null
    var destination: Point? = null

    var position: Point = Point(0F, 0F)
        get() = Point(x, y)

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
                this.destination = null
                return
            }

            x = nextPoint.x
            y = nextPoint.y
        }
    }

    fun nextPoint(game: Game): Point {
        if (destination == null) throw NullPointerException("Next step called but no destination")

        val orientation = Vector(position, destination!!).unit()
        val step = orientation * (game.properties.ballSpeed / (1000F / game.gameUpdateTime))
        return position + step
    }

    fun intersectTarget(game: Game, nextPoint: Point): Boolean {
        return Side.values().any {
            distance(game.properties.targetPoint(it), nextPoint) <
                game.properties.targetRadius + game.properties.ballRadius
        }
    }
}
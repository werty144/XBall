package com.example.game

import io.ktor.sessions.*
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient
import java.util.LinkedList

@Serializable
data class BallState(var x: Double, var y: Double) {
    @Transient
    val minZ = 1.0
    @Required
    var z: Double = minZ
    var active = true
    var ownerId: Int? = null
    @Transient
    var destinations = LinkedList<Point>()
    @Transient
    var flyMode: Boolean = false

    @Transient
    var position: Point = Point(0.0, 0.0)
        get() = Point(x, y)

    @Transient
    var orientation: Vector? = null
        get() = destinations.firstOrNull()?.let {
            if (position == it) {
                null
            } else {
                Vector(position, it).unit()
            }
        }

    fun update(game: Game) {
        if (ownerId != null) {
            val player = game.state.players.find { it.id == ownerId }!!
            var position = player.state.position +
                    player.state.orientation.unit() *
                    (game.properties.playerRadius + game.properties.ballRadius)
            position = game.properties.clipPointToBallBoundaries(position)
            x = position.x
            y = position.y
            return
        }

        val verticalSpeed = game.properties.ballSpeed / (1000F / game.gameUpdateTime)

        if (flyMode and (z < game.properties.flyHeight)) {
            z += kotlin.math.min(game.properties.flyHeight - z, verticalSpeed)
            return
        }

        if (!destinations.isEmpty()) {
            val destination = destinations.first
            val nextPoint = nextPoint(game)

            if (active and intersectTarget(game, nextPoint)) {
                targetAttempt(game)
                return
            }

            if (distance(position, destination) <= distance(position, nextPoint)) {
                x = destination.x
                y = destination.y
                flyMode = false
                destinations.poll()
                return
            }

            x = nextPoint.x
            y = nextPoint.y
        } else {
            if (!flyMode) {
                if (z > minZ) {
                    z -= kotlin.math.min(z - minZ, verticalSpeed)
                } else {
                    flyMode = false
                }
            }
        }
    }

    fun nextPoint(game: Game): Point {
        if (destinations.isEmpty()) throw IllegalStateException("Next step called but no destination")
        if (destinations.first == position) return position

        val orientation = Vector(position, destinations.first).unit()
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
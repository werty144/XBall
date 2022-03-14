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
    private var destinations = LinkedList<Point>()
    @Transient
    private var needsLift = false

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
            z = player.state.z
            return
        }

        val verticalSpeed = game.properties.ballSpeed / (1000F / game.gameUpdateTime)

        if (needsLift) {
            z += kotlin.math.min(game.properties.flyHeight - z, verticalSpeed)
            if (z >= game.properties.flyHeight) {
                needsLift = false
                if (intersectTargetIfPlacedTo(game, position)) targetAttempt(game)
            }
            return
        }

        if (!destinations.isEmpty()) {
            val destination = destinations.first
            val nextPoint = nextPoint(game)

            if (active and intersectTargetIfPlacedTo(game, nextPoint)) {
                targetAttempt(game)
                return
            }

            if (distance(position, destination) <= distance(position, nextPoint)) {
                x = destination.x
                y = destination.y
                destinations.poll()
                return
            }

            x = nextPoint.x
            y = nextPoint.y
        } else {
            if (z > minZ) {
                z -= kotlin.math.min(z - minZ, verticalSpeed)
            }
        }
    }

    fun inAir(): Boolean = z > minZ

    fun moves() = !destinations.isEmpty()

    fun clearDestinations() {
        destinations.clear()
    }

    fun setDestinations(newDestinations: LinkedList<Point>) {
        destinations = newDestinations
    }

    fun startLift() {
        needsLift = true
    }

    fun nextPoint(game: Game): Point {
        if (destinations.isEmpty()) throw IllegalStateException("Next step called but no destination")
        if (destinations.first == position) return position

        val orientation = Vector(position, destinations.first).unit()
        val step = orientation * (game.properties.ballSpeed / (1000F / game.gameUpdateTime))
        return position + step
    }

    fun intersectTargetIfPlacedTo(game: Game, nextPoint: Point): Boolean {
        return (z == game.properties.flyHeight) and
                Side.values().any {
                    distance(game.properties.targetPoint(it), nextPoint) <
                        game.properties.targetRadius + game.properties.ballRadius
                }
    }
}
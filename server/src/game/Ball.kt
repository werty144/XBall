package com.example.game

import io.ktor.sessions.*
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient
import java.util.LinkedList
import kotlin.math.abs
import kotlin.math.ceil
import kotlin.math.min
import kotlin.math.sqrt

@Serializable
data class BallState(var x: Double, var y: Double) {
    @Transient
    val properties = BallProperties()
    @Required
    var z: Double = properties.minZ
    var active = true
    @Required
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

        val verticalSpeed = game.properties.ballSpeed * game.deltaTime

        if (needsLift) {
            z += quadraticDecreasingStep(
                z,
                properties.minZ,
                game.properties.flyHeight,
                properties.verticalMovementWorldTime.toInt()/game.worldUpdateTime.toInt()
            )
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
                stickBallToTarget(game)
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
            if (z > properties.minZ) {
                z -= quadraticDecreasingStep(
                    z,
                    game.properties.flyHeight,
                    properties.minZ,
                    properties.verticalMovementWorldTime.toInt()/game.worldUpdateTime.toInt()
                )
            }
        }
    }

    fun inAir(): Boolean = z > properties.minZ

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
        val step = orientation * (game.properties.ballSpeed * game.deltaTime)
        return position + step
    }

    fun intersectTargetIfPlacedTo(game: Game, nextPoint: Point): Boolean {
        return (z == game.properties.flyHeight) and
                Side.values().any {
                    distance(game.properties.targetPoint(it), nextPoint) <
                        game.properties.targetRadius + game.properties.ballRadius
                }
    }

    fun stickBallToTarget(game: Game) {
        // call only if intersects target on next step
        val nextPoint = nextPoint(game)
        val targetSide = Side.values().find {
            distance(game.properties.targetPoint(it), nextPoint) < game.properties.ballRadius + game.properties.targetRadius
        }!!

        val targetPoint = game.properties.targetPoint(targetSide)
        val targetInteractionCircle = Circle(targetPoint, game.properties.targetRadius + game.properties.ballRadius)
        val ballTrajectoryLine = Line(position, nextPoint)
        val intersectionPoints = intersectionPoints(targetInteractionCircle, ballTrajectoryLine)!!
        val positionIntersectionVector = if (distance(position, intersectionPoints.first) < distance(position, intersectionPoints.second)) {
            Vector(position, intersectionPoints.first)
        } else {
            Vector(position, intersectionPoints.second)
        }
        position + positionIntersectionVector * 0.95
    }
}


class BallProperties {
    val minZ = 1.0
    val verticalMovementWorldTime = 1000L
}


fun linearStep(currentValue: Double, startValue: Double, endValue: Double, totalSteps: Int): Double {
    return min(abs(endValue - currentValue), abs((endValue - startValue) / totalSteps))
}

fun quadraticDecreasingStep(currentValue: Double, startValue: Double, endValue: Double, totalSteps: Int): Double {
    // evaluating parabola coefficients
    val a = -(endValue - startValue) / (totalSteps * totalSteps)
    val b = -2 * a * totalSteps
    val c = startValue

    val currentStep = ceil((-b - sqrt(b*b - 4 * a * (c - currentValue)))/(2*a)).toInt()
    val nextStep = currentStep + 1
    val nextValue = a*nextStep*nextStep + b*nextStep + c
    return min(abs(currentValue - endValue), (currentValue - nextValue))
}
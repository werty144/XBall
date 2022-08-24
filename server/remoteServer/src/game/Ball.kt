package com.example.game

import com.xballserver.remoteserver.game.Game
import com.xballserver.remoteserver.game.Side
import com.xballserver.remoteserver.game.targetAttempt
import kotlinx.serialization.Required
import kotlinx.serialization.Serializable
import kotlinx.serialization.Transient
import java.util.LinkedList
import kotlin.math.*

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
    private var verticalSpeed = 0.0

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

        z += verticalSpeed * game.deltaTime
        z = clip(z, properties.minZ, game.properties.flyHeight)
        if (((z == properties.minZ) && (verticalSpeed < 0)) || (z == game.properties.flyHeight)) {
            verticalSpeed = 0.0
        } else {
            verticalSpeed -= 9.8 * game.deltaTime
            return
        }

        if (!destinations.isEmpty()) {
            val destination = destinations.first
            val nextPoint = nextPoint(game)

            if ((z == game.properties.flyHeight) and intersectTargetIfPlacedTo(game, nextPoint) and active) {
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
            if (z == game.properties.flyHeight) {
                z -= 0.01
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
        verticalSpeed = properties.initialVerticalSpeed
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
    val initialVerticalSpeed = 15.0
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

fun clip(x: Double, lowerBound: Double, upperBound: Double) = min(upperBound, max(lowerBound, x))
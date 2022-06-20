package com.example.game

import com.example.routing.APIMove
import kotlinx.serialization.json.Json
import java.util.*
import kotlin.math.*
import kotlin.random.Random.Default.nextDouble

fun grabRandom(game: Game, move: APIMove) {
    val player = game.state.players.find { it.id == move.playerId }!!

    if (
        (distance(player.state.position, game.state.ballState.position) > game.properties.grabRadius) or
        game.state.ballState.inAir()
    ) return

    val successProbability = grabProbability(game, move)

    if (nextDouble() < successProbability) {
        game.state.ballState.ownerId = player.id
        game.state.ballState.clearDestinations()
    }

}

fun grabProbability(game: Game, move: APIMove): Double {
    val player = game.state.players.find { it.id == move.playerId }!!
    val ballState = game.state.ballState

    var successProbability: Double?
    val ballOwnerId = game.state.ballState.ownerId

    val viewAngle = viewAngle(player, ballState.position)

    successProbability = -(viewAngle / PI) * (viewAngle / PI) + 1
    val playerMoves = player.state.positionTarget != null
    if (playerMoves) {
        successProbability = successProbability.pow(1.3)
    }

    if (ballOwnerId == null) {
        if (ballState.moves()) {
            successProbability = successProbability.pow(1.3)
        }
    } else {
        if (ballOwnerId == player.id) return 1.0

        val ballOwner = game.state.players.find { it.id == ballOwnerId }!!
        if (ballOwner.side == player.side) return successProbability

        val ownerMoves = ballOwner.state.positionTarget != null
        if (ownerMoves) successProbability = successProbability.pow(1F/1.3)

        val facingAngle = viewAngle(ballOwner, player.state.position)

        successProbability *= (PI - facingAngle) / PI
    }

    return successProbability
}

fun throwRandom(game: Game, move: APIMove) {
    if (game.state.ballState.ownerId != move.playerId) return
    val player = game.state.players.find { it.id == move.playerId }!!
    val target = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
    game.state.ballState.ownerId = null
    game.state.ballState.setDestinations(randomDestination(game, target, player))
}

fun randomDestination(game: Game, target: Point, player: Player, sigma_factor: Float = 1F/20): LinkedList<Point> {
    if (game.state.ballState.position == target) return LinkedList<Point>(listOf(target))

    val random = Random()

    val ballPositionTarget = Vector(game.state.ballState.position, target)
    val playerMoves = player.state.positionTarget != null
    var sigma = ballPositionTarget.length() * sigma_factor
    if (playerMoves) sigma *= 1.5F
    val viewAngle = viewAngle(player, target)
    sigma *= 9 * (viewAngle / PI).toFloat().pow(3) + 1
    val dispersion = random.nextGaussian().toFloat() * sigma

    val ballPositionDestination = ballPositionTarget +
            (ballPositionTarget.unit() * dispersion) +
            (ballPositionTarget.orthogonalUnit() * dispersion)

    val throwSegment = Segment(game.state.ballState.position, game.state.ballState.position + ballPositionDestination)

    return throwSegment.getReflectionSequence(game.properties)
}

fun randomLiftDestination(game: Game, target: Point, player: Player): LinkedList<Point> = randomDestination(game, target, player, 0.1f)

fun bendRandom(game: Game, move: APIMove) {
    if (game.state.ballState.ownerId != move.playerId) return
    val player = game.state.players.find { it.id == move.playerId }!!
    val target = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
    game.state.ballState.ownerId = null
    game.state.ballState.startLift()
    game.state.ballState.setDestinations(randomLiftDestination(game, target, player))
}

fun attackRandom(game: Game, move: APIMove) {
    if (game.state.ballState.ownerId != move.playerId) return
    val player = game.state.players.find {it.id == move.playerId}!!
    val target = game.properties.targetPoint(player.side.other())
    game.state.ballState.ownerId = null
    game.state.ballState.startLift()
    game.state.ballState.setDestinations(randomLiftDestination(game, target, player))
}

fun targetAttempt(game: Game) {
    val ballState = game.state.ballState
    val ballNextPoint = ballState.nextPoint(game)
    val targetSide = Side.values().find {
        distance(game.properties.targetPoint(it), ballNextPoint) < game.properties.ballRadius + game.properties.targetRadius
    }!!

    val targetPoint = game.properties.targetPoint(targetSide)
    if (distance(targetPoint, ballState.position) < game.properties.ballRadius + game.properties.targetRadius) {
        game.goal(targetSide)
        return
    }

    val angle = Vector(ballState.position, targetPoint).angleWithVector(ballState.orientation!!)
    if (angle > PI/2) throw IllegalStateException("Can't be...")

    if (Random().nextDouble() < 1 - angle/(PI/2)) {
        game.goal(targetSide)
        return
    } else {
        val reflectionLine = Vector(ballState.position, targetPoint).orthogonalUnit()
        val newDirection = ballState.orientation!!.reflect(reflectionLine).unit()
        val bounceLength = Random().nextDouble() * 50
        val moveSegment = Segment(ballState.position, ballState.position + newDirection * bounceLength)
        ballState.setDestinations(moveSegment.getReflectionSequence(game.properties))
    }
}
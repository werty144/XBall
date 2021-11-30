package com.example.game

import com.example.routing.APIMove
import kotlinx.serialization.json.Json
import kotlin.math.*
import kotlin.random.Random.Default.nextDouble
import java.util.Random

fun grabRandom(game: Game, move: APIMove) {
    val player = game.state.players.find { it.id == move.playerId }!!

    if (distance(player.state.position, game.state.ballState.position) > game.properties.grabRadius) return

    val successProbability = grabProbability(game, move)

    if (nextDouble() < successProbability) {
        game.state.ballState.ownerId = player.id
        game.state.ballState.destination = null
    }

}

fun grabProbability(game: Game, move: APIMove): Double {
    val player = game.state.players.find { it.id == move.playerId }!!
    val ballState = game.state.ballState

    var successProbability: Double?
    val ballOwnerId = game.state.ballState.ownerId

    val angle =
        player.state.orientation.angleWithVector(
            Vector(
                player.state.position,
                ballState.position
            )
        )

    successProbability = -(angle / PI) * (angle / PI) + 1
    val playerMoves = player.state.positionTarget != null
    if (playerMoves) {
        successProbability = successProbability.pow(1.3)
    }

    if (ballOwnerId == null) {
        val ballMoves = ballState.destination != null
        if (ballMoves) {
            successProbability = successProbability.pow(1.3)
        }
    } else {
        if (ballOwnerId == player.id) return 1.0

        val ballOwner = game.state.players.find { it.id == ballOwnerId }!!
        if (ballOwner.teamUser == player.teamUser) return successProbability

        val ownerMoves = ballOwner.state.positionTarget != null
        if (ownerMoves) successProbability = successProbability.pow(1F/1.3)

        val facingAngle = ballOwner.state.orientation.angleWithVector(
            Vector(
                ballOwner.state.position,
                player.state.position
            )
        )

        successProbability = successProbability * (PI - facingAngle) / PI
    }

    return successProbability
}

fun throwRandom(game: Game, move: APIMove) {
    if (game.state.ballState.ownerId != move.playerId) return
    val player = game.state.players.find { it.id == move.playerId }!!

    val target = Json.decodeFromJsonElement(Point.serializer(), move.actionData)
    val random = Random()

    val positionTarget = Vector(player.state.position, target)
    val playerMoves = player.state.positionTarget != null
    var sigma = positionTarget.length()/ 20
    if (playerMoves) sigma *= 1.5F
    val orientationTargetAngle = player.state.orientation.angleWithVector(positionTarget)
    sigma *= 9 * (orientationTargetAngle / PI).toFloat().pow(3) + 1
    val dispersion = random.nextGaussian().toFloat() * sigma

    val positionDestination = positionTarget +
            (positionTarget.unit() * dispersion) +
            (positionTarget.orthogonalUnit() * dispersion)

    val destination = game.properties.clipPointToField(player.state.position + positionDestination)
    game.state.ballState.ownerId = null
    game.state.ballState.destination = destination

}
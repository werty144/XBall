package com.example.game

import com.example.routing.APIMove
import kotlin.math.*
import kotlin.random.Random.Default.nextDouble

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

    val angle = abs(
        player.state.orientation.orientedAngleWithVector(
            Vector(
                player.state.position,
                ballState.position
            )
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

        val facingAngle = abs(ballOwner.state.orientation.orientedAngleWithVector(
            Vector(
                ballOwner.state.position,
                player.state.position
            )
        ))
    }

    return successProbability
}
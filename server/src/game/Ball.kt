package com.example.game

import kotlinx.serialization.Serializable

@Serializable
data class BallState(var x: Float, var y: Float) {
    var z: Float = 0F
    var ownerId: Int? = null
    var destination: Point? = null

    var position: Point = Point(0F, 0F)
        get() = Point(x, y)

    fun update(game: Game, updateTime: Long) {
        if (ownerId != null) {
            val player = game.state.players.find { it.id == ownerId }!!
            val position = player.state.position +
                    player.state.orientation.unit() *
                    (game.properties.playerRadius + game.properties.ballRadius).toFloat()
            x = position.x
            y = position.y
        }

        if (destination != null) {
            val destination = destination!!
            if (!game.properties.pointWithinField(destination)) return

            val position = Point(x, y)
            val orientation = Vector(position, destination).unit()

            val step = orientation * (game.properties.ballSpeed / (1000F / updateTime))
            if (distance(destination, position) <= step.length()) {
                    x = destination.x
                    y = destination.y
                    this.destination = null
            } else {
                val nextPoint = position + step
                x = nextPoint.x
                y = nextPoint.y
            }
        }
    }
}
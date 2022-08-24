package com.xballserver.remoteserver.game

import com.example.game.*
import com.xballserver.remoteserver.routing.APIMove
import com.xballserver.remoteserver.infrastructure.UserId
import com.xballserver.remoteserver.routing.tryJsonParse
import kotlinx.serialization.Serializable
import java.util.LinkedList
import kotlin.math.*

typealias GameId = Int

data class Game(val gameId: GameId, val user1Id: UserId, val user2Id: UserId, val properties: GameProperties, val worldUpdateTime: Long) {
    lateinit var state: GameState
    val sides: Map<UserId, Side>
    val score: MutableMap<Side, Int>
    var gameEnded: Boolean
    val gameUpdateTime: Float
    val deltaTime: Float
    val timer: Timer

    init {
        gameUpdateTime = worldUpdateTime * properties.speed.slowingCoefficient()
        deltaTime = gameUpdateTime / 1000F  // used to multiply speed by
        sides = mapOf(user1Id to Side.LEFT, user2Id to Side.RIGHT)
        score = mutableMapOf(Side.LEFT to 0, Side.RIGHT to 0)
        gameEnded = false
        toInitialState()
        timer = Timer(worldUpdateTime)
        timer.start()
    }

    fun toInitialState() {
        val players = ArrayList<Player>()
        var sparePlayersId = 0
        for (i in 1..properties.playersNumber) {
            players.add(
                Player(sparePlayersId++, sides[user1Id]!!,
                PlayerState(
                    properties.fieldWidth * 1/4,
                    properties.fieldHeight / (properties.playersNumber + 1) * i,
                    orientation = Vector(1.0, 0.0)
                )
            )
            )
            players.add(
                Player(sparePlayersId++, sides[user2Id]!!,
                PlayerState(
                    properties.fieldWidth * 3/4,
                    properties.fieldHeight / (properties.playersNumber + 1) * i,
                    orientation = Vector(-1.0, 0.0)
                )
            )
            )
        }
        val ballState = BallState(properties.fieldWidth / 2, properties.fieldHeight / 2)
        state = GameState(players, ballState)
    }

    fun makeMove(move: APIMove, userId: UserId) {
        if (!validateMove(move, userId)) {
            return
        }
        val player = state.players.find { it.id == move.playerId }!!
        when (move.action) {
            "movement" -> {
                val positionTarget = tryJsonParse(Point.serializer(), move.actionData) ?: return
                player.state.positionTarget = properties.clipPointToField(positionTarget)
            }
            "orientation" -> {
                val orientationTarget = tryJsonParse(Point.serializer(), move.actionData) ?: return
                player.state.orientationTarget = properties.clipPointToField(orientationTarget)
            }
            "grab" -> {
                grabRandom(this, move)
            }
            "throw" -> {
                throwRandom(this, move)
            }
            "bend" -> {
                bendRandom(this, move)
            }
            "attack" -> {
                attackRandom(this, move)
            }
            "stop" -> {
                player.state.positionTarget = null
                player.state.orientationTarget = null
            }
        }
    }

    fun nextState() {
        if (gameEnded) return
        if (timer.needsUnpause) {
            timer.needsUnpause = false
            toInitialState()
            timer.start()
            return
        }

        timer.tick()
        state.players.forEach {
            it.nextState(this)
        }
        state.ballState.update(this)
    }

    fun validateMove(move: APIMove, userId: UserId): Boolean {
        val player = state.players.find { it.id == move.playerId } ?: return false

        return player.side == sides[userId]
    }

    fun goal(side: Side) {
        if (!state.ballState.active) return
        score[side.other()] = score[side.other()]!! + 1
        if (score[side.other()] == properties.winScore) {
            gameEnded = true
        }
        state.ballState.setDestinations(LinkedList<Point>(listOf(properties.targetPoint(side))))
        state.ballState.active = false
        timer.pause()
    }

    fun getStatus(): GameStatus {
        return when (gameEnded) {
            true -> GameStatus.ENDED
            false -> GameStatus.RUNNING
        }
    }

    fun getBallOwner(): Player? = state.players.findLast { it.id == state.ballState.ownerId }
}

enum class Side {
    LEFT,
    RIGHT;

    fun other(): Side {
        return when (this) {
            LEFT -> RIGHT
            RIGHT -> LEFT
        }
    }
}

enum class GameStatus {
    ENDED,
    RUNNING
}

enum class Speed {
    SLOW,
    NORMAL,
    FAST;

    fun slowingCoefficient(): Float {
        return when (this) {
            SLOW -> 0.25F
            NORMAL -> 0.5F
            FAST -> 1F
        }
    }
}


@Serializable
data class GameProperties(
    val playersNumber: Int,
    val speed: Speed
) {
    val fieldWidth = 40.0
    val fieldHeight = 20.0
    val playerSpeed = 6.0
    val ballSpeed = 15.0
    val playerRotationSpeed = 2*PI
    val playerRadius = .35
    val ballRadius = .3
    val grabRadius = 1.5
    val targetXMargin = 0.15
    val targetYMargin = 0.5
    val targetZ = 4.0
    val targetRadius = .4
    val flyHeight = targetZ
    val winScore = 5
    val grabCoolDown = 500
    @kotlinx.serialization.Transient
    val ballBoundaries = run {
        val p1 = Point(ballRadius, ballRadius)
        val p2 = Point(ballRadius, fieldHeight - ballRadius)
        val p3 = Point(fieldWidth - ballRadius, fieldHeight - ballRadius)
        val p4 = Point(fieldWidth - ballRadius, ballRadius)
        listOf(Segment(p1, p2), Segment(p2, p3), Segment(p3, p4), Segment(p4, p1))
    }

    override fun equals(other: Any?): Boolean {
        return if (other is GameProperties) {
            (playersNumber == other.playersNumber) and (speed == other.speed)
        } else {
            false
        }
    }

    fun pointWithinField(point: Point): Boolean {
        return (0 <= point.x) and (point.x <= fieldWidth) and (0 <= point.y) and (point.y <= fieldHeight)
    }

    fun clipToBoundaries(point: Point, margin: Double) =
        Point(
            max(margin, min(point.x, fieldWidth - margin)),
            max(margin, min(point.y, fieldHeight - margin))
        )

    fun clipPointToField(point: Point): Point = clipToBoundaries(point, 0.0)

    fun clipPointToBallBoundaries(point: Point): Point = clipToBoundaries(point, ballRadius)


    fun playersIntersectIfPlacedTo(position1: Point, position2: Point): Boolean {
        return distance(position1, position2) < 2 * playerRadius
    }

    fun targetPoint(side: Side): Point {
        return when (side) {
            Side.LEFT -> Point(fieldWidth * targetXMargin, fieldHeight * targetYMargin)
            Side.RIGHT -> Point(fieldWidth * (1 - targetXMargin), fieldHeight * targetYMargin)
        }

    }

    override fun hashCode(): Int {
        var result = playersNumber
        result = 31 * result + speed.hashCode()
        return result
    }
}


@Serializable
data class GameState(val players: List<Player>, val ballState: BallState)

data class Timer(val worldUpdateTime: Long) {
    var time: Long = 0L
    private var active: Boolean = true
    private var toActivityCountDown: Long? = null
    var needsUnpause: Boolean = false

    fun start() {
        active = true
        toActivityCountDown = null
    }

    fun pause(toActivityTime: Long = 3000L) {
        active = false
        toActivityCountDown = toActivityTime
    }

    fun tick() {
        if (active) {
            time += worldUpdateTime
        } else {
            toActivityCountDown = toActivityCountDown!!.minus(worldUpdateTime)
            if (toActivityCountDown!! <= 0) {
                needsUnpause = true
            }
        }
    }
}
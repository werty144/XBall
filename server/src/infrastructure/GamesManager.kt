package com.example.infrastructure

import com.example.game.Game
import com.example.game.GameId
import com.example.game.GameStatus
import com.example.game.Side
import com.example.routing.APIMove
import com.example.routing.Connection
import com.example.routing.createGameJSONString
import com.example.routing.createPrepareGameJSONString
import io.ktor.http.cio.websocket.*
import kotlinx.coroutines.*
import java.sql.Timestamp
import java.util.Collections

class GamesManager(val connections: Set<Connection>) {
    private var spareGameId: GameId = 0
    val games: MutableSet<Game> = Collections.synchronizedSet(LinkedHashSet())
    val preparedGames: MutableSet<PreparedGame> = Collections.synchronizedSet(LinkedHashSet())
    val preparedGameOutDateTime = 15_000L
    private val updateTime = 5L
    private val gameCoroutineScope: CoroutineScope = CoroutineScope(CoroutineName("Game scope"))

    fun gameById(gameId: GameId) = games.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = games.find { (it.user1Id == userId) or (it.user2Id == userId) }

    fun userHasGames(userId: UserId): Boolean = games.any { (it.user1Id == userId) or (it.user2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }

    suspend fun acceptInvite(invite: Invite) {
        val firstUserId = invite.inviterId
        val secondUserId = invite.invitedId
        val firstUserConnection = connections.firstOrNull { it.userId == firstUserId }
        val secondUserConnection = connections.firstOrNull { it.userId == secondUserId }

        if ((!isActiveConnection(firstUserConnection) or (!isActiveConnection(secondUserConnection)))) {
            return
        }

        val game = Game(spareGameId++, firstUserId, secondUserId, invite.gameProperties, updateTime)

        val firstUserMessage = createPrepareGameJSONString(game, Side.LEFT)
        val secondUserMessage = createPrepareGameJSONString(game, Side.RIGHT)

        firstUserConnection!!.session.send(firstUserMessage)
        secondUserConnection!!.session.send(secondUserMessage)

        preparedGames.add(PreparedGame(game, firstUserId, secondUserId))
    }

    fun userReady(userId: UserId) {
        val preparedGame = preparedGames.firstOrNull {(it.firstUserId == userId) or (it.secondUserId == userId)} ?: return

        preparedGame.ready[userId] = true

        if (preparedGame.ready[preparedGame.firstUserId]!! and preparedGame.ready[preparedGame.secondUserId]!!) {
            preparedGames.remove(preparedGame)
            val game = preparedGame.game
            games.add(game)
            game.toInitialState()
            gameCoroutineScope.launch { runGame(game) }
        }
    }

    suspend fun runGame(game: Game) {
        var firstPlayerConnection = connections.firstOrNull { (it.userId == game.user1Id) and it.session.isActive}
        var secondPlayerConnection = connections.firstOrNull { (it.userId == game.user2Id) and it.session.isActive }
        while (true) {
            delay(updateTime)
            game.nextState()
            val message = createGameJSONString(game)

            if (isActiveConnection(firstPlayerConnection)) {
                firstPlayerConnection!!.session.send(message)
            } else {
                firstPlayerConnection = connections.firstOrNull { (it.userId == game.user1Id) and it.session.isActive}
            }

            if (isActiveConnection(secondPlayerConnection)) {
                secondPlayerConnection!!.session.send(message)
            } else {
                secondPlayerConnection = connections.firstOrNull { (it.userId == game.user2Id) and it.session.isActive}
            }

            if (game.getStatus() == GameStatus.ENDED) {
                games.remove(game)
                return
            }
        }
    }

    fun isActiveConnection(connection: Connection?): Boolean {
        return (connection != null) && (connection.session.isActive)
    }

    fun clean() {
        val currentTimeStamp = Timestamp(System.currentTimeMillis())
        preparedGames.removeIf { currentTimeStamp.time - it.timeStamp.time > preparedGameOutDateTime }
    }
}

data class PreparedGame(val game: Game, val firstUserId: UserId, val secondUserId: UserId) {
    val timeStamp: Timestamp = Timestamp(System.currentTimeMillis())
    val ready = mutableMapOf(firstUserId to false, secondUserId to false)
}
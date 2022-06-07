package com.example.infrastructure

import com.example.game.*
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
    private val updateTime = 5L
    private val gameCoroutineScope: CoroutineScope = CoroutineScope(CoroutineName("Game scope"))
    private val runningGames: MutableSet<Pair<GameId, Job>> = Collections.synchronizedSet(LinkedHashSet())

    fun gameById(gameId: GameId) = games.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = games.find { (it.user1Id == userId) or (it.user2Id == userId) }

    fun userHasGames(userId: UserId): Boolean = games.any { (it.user1Id == userId) or (it.user2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }
    suspend fun startGameFromLobby(lobby: Lobby)
    {
        val userIds = lobby.users.toList()
        if (userIds.map { getGameForUser(it) }.any { it != null }) return

        val userConnections = userIds.map {id ->  connections.firstOrNull { con -> con.userId == id } }
        if (!userConnections.map {isActiveConnection(it)}.all { it }) {
            return
        }

        val game: Game
        when (lobby.nMembers) {
            1 -> {
                game = Game(spareGameId++, userIds[0], userIds[0], lobby.gameProperties, updateTime)
                val firstUserMessage = createPrepareGameJSONString(game, Side.RIGHT)
                userConnections[0]!!.session.send(firstUserMessage)
            }
            2 -> {
                game = Game(spareGameId++, userIds[0], userIds[1], lobby.gameProperties, updateTime)
                val firstUserMessage = createPrepareGameJSONString(game, Side.LEFT)
                val secondUserMessage = createPrepareGameJSONString(game, Side.RIGHT)
                userConnections[0]!!.session.send(firstUserMessage)
                userConnections[1]!!.session.send(secondUserMessage)
            }
            else -> {
                return
            }
        }

        games.add(game)
        game.toInitialState()
        val gameJob = gameCoroutineScope.launch { runGame(game) }
        runningGames.add(Pair(game.gameId, gameJob))
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

    fun stopGame(gameId: GameId) {
        runningGames.find { it.first == gameId }?.second?.cancel()
        games.removeIf { it.gameId == gameId }
    }

    fun stopAll() {
        games.forEach { stopGame(it.gameId) }
    }

    fun isActiveConnection(connection: Connection?): Boolean {
        return (connection != null) && (connection.session.isActive)
    }

    fun clean() {

    }
}
